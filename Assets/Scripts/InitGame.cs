

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Initializes the LOL content and then enters the title screen.
// This code was taken from Loader.cs (a file from the LOL template content) and then modified.
namespace BBTS
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
            SystemManager.Instance.saveSystem.allowSaveLoad = true;
#elif UNITY_WEBGL
            SystemManager.Instance.saveSystem.allowSaves = false;
#elif UNITY_IOS || UNITY_ANDROID
            SystemManager.Instance.saveSystem.allowSaves = false;
#endif


            // Helper method to hide and show the state buttons as needed.
            // Will call LoadState<T> for you.
            // Helper.StateButtonInitialize<CookingData>(newGameButton, continueButton, OnLoad);

            // TODO: take this out?
            // Shows that the game has been initialized?
            
        }

        // Start is called just before any of the Update methods is called the first time.
        public void Start()
        {
            // Gets the language manager.
            LanguageManager lm = LanguageManager.Instance;

            // Sets text to be translated.
            lm.translateText = true;

            // If text should be translated.
            if(lm.translateText)
            {
                lm.LoadEnglish();
            }

            // Gets the save system.
            SaveSystem saveSys = SystemManager.Instance.saveSystem;

            // Don't save if running through a web player.
            saveSys.allowSaveLoad = !(Application.platform == RuntimePlatform.WebGLPlayer);

            // Loads the current save.
            if (saveSys.allowSaveLoad)
                LoadCurrentSave();

            // Game has been initialized.
            initGame = true;
        }

        // Load the current save.
        public void LoadCurrentSave()
        {
            bool success = SaveSystem.Instance.LoadSave();
        }

        // Update is called once per frame
        void Update()
        {
            // Load the title scene on the first frame.
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }
    }
}