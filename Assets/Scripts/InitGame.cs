

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
            // Creates instances of system, which has a LanguageManager and SaveSystem.
            SystemManager system = SystemManager.Instance;
            
            // LanguageManager lm = LanguageManager.Instance;
            // SaveSystem saveSys = SaveSystem.Instance;
            
            initGame = false;
        }

        // Initializes the game.
        public void Initialize()
        {
            // Game has already been initialized, so don't do anything.
            if (initGame)
                return;

            // Load the English translation.
            LanguageManager lm = LanguageManager.Instance;

            // Gets the save system.
            SaveSystem saveSys = SystemManager.Instance.saveSystem;

            // NOTE: maybe comment this out if you don't want to use it.

            // Checks if the application platform is WebGL or not.
            if(Application.platform == RuntimePlatform.WebGLPlayer) // Is WebGL
            {
                // Don't allow saving, and don't translate text.
                saveSys.allowSaveLoad = false;
                lm.translateText = false;
                lm.changeTextColor = false;
            }
            else // Not WebGL
            {
                // Allow data to be saved.
                saveSys.allowSaveLoad = true;

                // Sets text to be translated.
                lm.translateText = false; // Set to 'true' if testing file reading.
                lm.changeTextColor = false;
            }

            // Default - sets text to be translated.
            // lm.translateText = true;

            // If text should be translated.
            if (lm.translateText)
            {
                lm.LoadEnglish();
                saveSys.RefreshFeedbackString();
            }

            // Default - don't save if running through a web player.
            // saveSys.allowSaveLoad = !(Application.platform == RuntimePlatform.WebGLPlayer);

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
            // Initialize the game on the first update frame.
            Initialize();

            // Load the title scene on the first frame.
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }
    }
}