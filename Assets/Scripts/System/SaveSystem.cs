using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BBTS
{
    [System.Serializable]
    public struct BBTS_Vec3
    {
        public float x;
        public float y;
        public float z;

        // Set components to 0.
        public void SetZero()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        // Convert Vector3 to Vec3
        public static BBTS_Vec3 Vector3ToVec3(Vector3 v)
        {
            BBTS_Vec3 v2;

            v2.x = v.x;
            v2.y = v.y;
            v2.z = v.z;

            return v2;
        }

        // Vec3 to Unity Vector3
        public static Vector3 Vec3ToVector3(BBTS_Vec3 v)
        {
            Vector3 v2;

            v2.x = v.x;
            v2.y = v.y;
            v2.z = v.z;

            return v2;
        }
    }

    // The battle bot training sim data.
    [System.Serializable]
    public class BBTS_GameData
    {
        // Becomes set to 'true' to indicate that there is data to be read.
        public bool valid = false;

        // Marks whether the data is from a completed game or not (game was finished if 'complete' is set to 'true').
        // If the game was completed, start a new game instead.
        public bool complete = false;

        // The player's data.
        public BattleEntitySaveData playerData;

        // The save data for the doors in the game.
        // This also holds the data for each entity.
        // public List<DoorSaveData> doorData;

        // The door save data.
        public DoorSaveData[] doorData = new DoorSaveData[OverworldManager.ROOM_COUNT];

        // Triggers for the tutorial for the game.
        public bool clearedIntro; // Intro tutorial.
        public bool clearedBattle; // Battle tutorial.
        public bool clearedFirstMove; // First move tutorial.
        public bool clearedCritical; // Critical tutorial.
        public bool clearedRecoil; // Recoil tutorial.
        public bool clearedStatChange; // Stat change tutorial.
        public bool clearedBurn; // Burn tutorial.
        public bool clearedParalysis; // Paralysis tutorial.
        public bool clearedFirstBattleDeath; // First battle death tutorial.
        public bool clearedOverworld; // Overworld tutorial.
        public bool clearedTreasure; // Treasure tutorial.
        public bool clearedQuestion; // Question tutorial.
        public bool clearedPhase; // Phase tutorial.
        public bool clearedBoss; // Boss tutorial.
        public bool clearedGameOver; // Game over tutorial.

        // TODO: saving rooms total may not be needed.

        // Results data at the time of the save.
        public int score = 0; // Score
        public int roomsCompleted = 0; // Rooms cleared by the player.

        // The next question round.
        public int questionCountdown = 0;

        // The amount of used questions and the results for said questions.
        public int[] questionsUsed = new int[OverworldManager.ROOM_COUNT];
        public bool[] questionResults = new bool[OverworldManager.ROOM_COUNT];

        // The serializer does NOT like integer arrays or lists for some reason.
        // As such, each one had to be stored as a seperate variable.

        // Not needed since this is a fixed value.
        // public int roomsTotal = 0; // Total rooms cleared.

        public int evolveWaves = 0; // Evolution waves.
        public float gameTime = 0.0F; // Total game time.
        public int turnsPassed = 0; // Total turns.

    }

    // Used to save the game.
    public class SaveSystem : MonoBehaviour
    {
        // The instance of the Battle Manager.
        private static SaveSystem instance;

        // If set to 'true', the game allows the player to save.
        public bool allowSaveLoad = false; // False by default.

        // Becomes 'true' when the save system is initialized.
        private bool initialized = false;

        // The game data.
        // The last game save. This is only for testing purposes.
        public BBTS_GameData lastSave;

        // The data that was loaded.
        public BBTS_GameData loadedData;

        // The file reader.
        public FileReader fileReader = null;

        // The manager for the game.
        public GameplayManager gameManager = null;

        // LOL - AutoSave //
        // Added from the ExampleCookingGame. Used for feedback from autosaves.
        WaitForSeconds feedbackTimer = new WaitForSeconds(2);
        Coroutine feedbackMethod;
        public TMP_Text feedbackText;

        // The string shown when having feedback.
        private string feedbackString = "Saving Data";

        // The string key for the feedback.
        private const string FEEDBACK_STRING_KEY = "sve_msg_savingGame";

        // Private constructor so that only one save system object exists.
        private SaveSystem()
        {
            // ...
        }

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // This is the instance.
            if (instance == null)
            {
                instance = this;

                // // Don't destroy the language manager on load.
                // DontDestroyOnLoad(gameObject);
            }

            // Initializes the save system.
            if (!initialized)
                Initialize();
        }


        // Start is called before the first frame update
        void Start()
        {
            // The language manager.
            LanguageManager lm = LanguageManager.Instance;

            // If the language should be translated.
            if (lm.TranslateAndLanguageSet())
            {
                feedbackString = LanguageManager.Instance.GetLanguageText(FEEDBACK_STRING_KEY);
            }

        }

        // Returns the instance of the save system.
        public static SaveSystem Instance
        {
            get
            {
                // Checks to see if the instance exists. If it doesn't, generate an object.
                if (instance == null)
                {
                    // Makes a new settings object.
                    GameObject go = new GameObject("Save System (singleton)");

                    // Adds the instance component to the new object.
                    instance = go.AddComponent<SaveSystem>();
                }

                // returns the instance.
                return instance;
            }
        }

        // Checks if the save system has been initialized.
        public bool Initialized
        {
            get { return initialized; }
        }

        // Set save and load operations.
        public void Initialize()
        {
            // The result.
            bool result;

            // Creates the file reader with its file path and file.
            fileReader = new FileReader();
            fileReader.filePath = "Assets\\Resources\\Data\\";
            fileReader.file = "save.dat";

            // Checks if the file exists.
            result = fileReader.FileExists();

            // Save system has been initialized.
            initialized = true;
        }

        // Checks if the game manager has been set.
        private bool IsGameManagerSet()
        {
            if (gameManager == null)
                gameManager = FindObjectOfType<GameplayManager>(true);

            // Game manager does not exist.
            if (gameManager == null)
            {
                Debug.LogWarning("The Game Manager couldn't be found.");
                return false;
            }

            return true;
        }

        // Sets the last bit of saved data to the loaded data object.
        public void SetLastSaveAsLoadedData()
        {
            loadedData = lastSave;
        }

        // Clears out the last save and the loaded data object.
        public void ClearLoadedAndLastSaveData()
        {
            lastSave = null;
            loadedData = null;
        }

        // Converts an object to bytes (requires seralizable object) and returns it.
        static public byte[] SerializeObject(object data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, data); // Serialize the data for them emory stream.
            return ms.ToArray();
        }

        // Deserialize the provided object, converting it to an object and returning it.
        static public object DeserializeObject(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            ms.Write(data, 0, data.Length); // Write data.
            ms.Seek(0, 0); // Return to start of data.

            return bf.Deserialize(ms); // return content
        }

        // Saves data.
        public bool SaveGame()
        {
            // The game manager does not exist if false.
            if (!IsGameManagerSet())
            {
                // Tries to find a gameplay manager.
                GameplayManager temp = FindObjectOfType<GameplayManager>();

                // Checks if a gameplay manager exists.
                if (temp != null)
                {
                    // Set gameplay manager.
                    gameManager = temp;
                }
                else
                {
                    Debug.LogWarning("The Game Manager couldn't be found.");
                    return false;
                }
            }

            // Determines if saving wa a success.
            bool success = false;

            // Generates the save data.
            BBTS_GameData savedData = gameManager.GenerateSaveData();

            // Save to a file.
            bool result = SaveToFile(savedData);

            // Stores the most recent save.
            lastSave = savedData;

            // TODO: implement save state.

            return success;
        }

        // Save the information to a file.
        private bool SaveToFile(BBTS_GameData data)
        {
            // Gets the file.
            string file = fileReader.GetFileWithPath();

            // Will generate the file if it doesn't exist.
            // // Checks that the file exists.
            // if (!fileReader.FileExists())
            //     return false;

            // Seralize the data.
            byte[] dataArr = SerializeObject(data);

            // Data did not serialize properly.
            if (dataArr.Length == 0)
                return false;

            // Write to the file.
            File.WriteAllBytes(file, dataArr);

            // Data written successfully.
            return true;
        }

        // Loads a save.
        public bool LoadSave()
        {
            // The result of loading the save data.
            bool success;

            // The file doesn't exist.
            if (!fileReader.FileExists())
            {
                return false;
            }

            // Loads the file.
            loadedData = LoadFromFile();

            // The data has been loaded successfully.
            success = loadedData != null;

            return success;
        }


        // Loads information from a file.
        private BBTS_GameData LoadFromFile()
        {
            // Gets the file.
            string file = fileReader.GetFileWithPath();

            // Checks that the file exists.
            if (!fileReader.FileExists())
                return null;

            // Read from the file.
            byte[] dataArr = File.ReadAllBytes(file);

            // Data did not serialize properly.
            if (dataArr.Length == 0)
                return null;

            // Deseralize the data.
            object data = DeserializeObject(dataArr);

            // Convert to loaded data.
            BBTS_GameData loadData = (BBTS_GameData)(data);

            // Return loaded data.
            return loadData;
        }

        // Called for saving the result.
        private void OnSaveResult(bool success)
        {
            if (!success)
            {
                Debug.LogWarning("Saving not successful");
                return;
            }

            if (feedbackMethod != null)
                StopCoroutine(feedbackMethod);



            // ...Auto Saving Complete
            feedbackMethod = StartCoroutine(Feedback(feedbackString));
        }

        // Feedback while result is saving.
        IEnumerator Feedback(string text)
        {
            // Only updates the text that the feedback text was set.
            if(feedbackText != null)
                feedbackText.text = text;

            yield return feedbackTimer;
            
            // Only updates the content if the feedback text has been set.
            if(feedbackText != null)
                feedbackText.text = string.Empty;
            
            // nullifies the feedback method.
            feedbackMethod = null;
        }

        // Checks if the game has loaded data.
        public bool HasLoadedData()
        {
            // Used to see if the data is available.
            bool result;

            // Checks to see if the data exists.
            if (loadedData != null) // Exists.
            {
                // Checks to see if the data is valid.
                result = loadedData.valid;
            }
            else // No data.
            {
                // Not readable.
                result = false;
            }
                
            // Returns the result.
            return result;
        }

        // Removes the loaded data.
        public void ClearLoadedData()
        {
            loadedData = null;
        }

        // The gameplay manager now checks if there is loadedData. If so, then it will load in the data when the game starts.
        // // Loads a saved game. This returns 'false' if there was no data.
        // public bool LoadGame()
        // {
        //     // No loaded data.
        //     if(loadedData == null)
        //     {
        //         Debug.LogWarning("There is no saved game.");
        //         return false;
        //     }
        // 
        //     // TODO: load the game data.
        // 
        //     return true;
        // }

        // Called to load data from the server.
        private void OnLoadData(BBTS_GameData loadedGameData)
        {
            // Overrides serialized state data or continues with editor serialized values.
            if (loadedGameData != null)
            {
                loadedData = loadedGameData;
            }
            else // No game data found.
            {
                Debug.LogError("No game data found.");
                loadedData = null;
                return;
            }

            // TODO: save data for game loading.
            if(!IsGameManagerSet())
            {
                Debug.LogError("Game gameManager not found.");
                return;
            }

            // TODO: this automatically loads the game if the continue button is pressed.
            // If there is no data to load, the button is gone. 
            // You should move the buttons around to accomidate for this.
            // LoadGame();
        }

        
    }
}