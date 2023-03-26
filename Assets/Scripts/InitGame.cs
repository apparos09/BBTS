

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Initializes the LOL content and then enters the title screen.
// This code was taken from Loader.cs (a file from the LOL template content) and then modified.
namespace RM_BBTS
{
    public class InitGame : MonoBehaviour
    {
        // GAME //
        // Data for the game.
        BBTS_GameData gameData;

        // Becomes 'true' when the game has been initialized.
        public bool initGame = false;

        // The text for the game initialization.
        public TMP_Text initText;

        // LOL //
        // Relative to Assets /StreamingAssets/
        private const string languageJSONFilePath = "language.json";
        private const string questionsJSONFilePath = "questions.json";
        private const string startGameJSONFilePath = "startGame.json";

        // Use to determine when all data is preset to load to next state.
        // This will protect against async request race conditions in webgl.
        LoLDataType _receivedData;

        // This should represent the data you're expecting from the platform.
        // Most games are expecting 2 types of data, Start and Language.
        LoLDataType _expectedData = LoLDataType.START | LoLDataType.LANGUAGE;

        // // LOL - AutoSave //
        // // Added from the ExampleCookingGame. Used for feedback from autosaves.
        // WaitForSeconds feedbackTimer = new WaitForSeconds(2);
        // Coroutine feedbackMethod;
        // public TMP_Text feedbackText;

        [System.Flags]
        enum LoLDataType
        {
            START = 0,
            LANGUAGE = 1 << 0,
            QUESTIONS = 1 << 1
        }

        void Awake()
        {
            // Unity Initialization
            Application.targetFrameRate = 30; // 30 FPS
            Application.runInBackground = false; // Don't run in the background.

            // Use the tutorial by default.
            GameSettings.Instance.UseTutorial = true;

            // LOL Initialization
            // Create the WebGL (or mock) object
#if UNITY_EDITOR
            // ...
#elif UNITY_WEBGL
            // ...
#elif UNITY_IOS || UNITY_ANDROID
            // ...
#endif

            
           // 
            StartCoroutine(_WaitForData());

            // Helper method to hide and show the state buttons as needed.
            // Will call LoadState<T> for you.
            // Helper.StateButtonInitialize<CookingData>(newGameButton, continueButton, OnLoad);

            // TODO: take this out?
            // Shows that the game has been initialized?
            initGame = true;
        }

        // Start is called just before any of the Update methods is called the first time.
        public void Start()
        {
            // Taken out since this probably doesn't need to be translated.
            // // Grabs the language defs.
            // JSONNode defs = SharedState.LanguageDefs;
            // 
            // // Translate the text in case it's shown on screen from the game taking a while to initialize.
            // if (initText != null && defs != null)
            // {
            //     // Do this just in case 
            //     // initText.text = defs["kwd_loading"]; // Loading
            // }
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif
            // LOLSDK.Instance.SaveResultReceived -= OnSaveResult;
        }

        // // On save result.
        // void OnSaveResult(bool success)
        // {
        //     if (!success)
        //     {
        //         Debug.LogWarning("Saving not successful");
        //         return;
        //     }
        // 
        //     if (feedbackMethod != null)
        //         StopCoroutine(feedbackMethod);
        //     // ...Auto Saving Complete
        //     feedbackMethod = StartCoroutine(Feedback("autoSave"));
        // }

        // Waits for the data and then loads the scene.
        IEnumerator _WaitForData()
        {
            yield return new WaitUntil(() => (_receivedData & _expectedData) != 0);
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }

        /// <summary>
        /// This is the setting of your initial state when the scene loads.
        /// The state can be set from your default editor settings or from the
        /// users saved data after a valid save is called.
        /// </summary>
        /// <param name="loadedGameData"></param>
        void OnLoad(BBTS_GameData loadedGameData)
        {
            // Overrides serialized state data or continues with editor serialized values.
            if (loadedGameData != null)
            {
                gameData = loadedGameData;
                LOLManager.Instance.saveSystem.loadedData = loadedGameData;
            }
            else
            {
                return;
            }
               
            // Becomes set to 'true' when the game data has been loaded.
            initGame = true;
        }
        
    }
}