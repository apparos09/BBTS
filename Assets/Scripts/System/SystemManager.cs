

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBTS
{
    // a class for the system management (formally the LOLManager).
    public class SystemManager : MonoBehaviour
    {
        // the instance of the system manager.
        private static SystemManager instance;

        // The language manager.
        public LanguageManager languageManager;

        // The save system for the game.
        public SaveSystem saveSystem;
        
        // The text-to-speech object.
        public TextToSpeech textToSpeech;

        // The maixmum progress points for the game.
        // Currently, progress is based on the amount of doors cleared in the game.
        const int MAX_PROGRESS = OverworldManager.ROOM_COUNT; // same as room count.

        // private constructor so that only one accessibility object exists.
        private SystemManager()
        {
            // ...
        }

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // This is the instance.
            if (instance == null)
                instance = this;

            // This object should not be destroyed.
            DontDestroyOnLoad(this);

            // If the text-to-speech component is not set, try to get it.
            if (textToSpeech == null)
            {
                // Tries to get the component.
                if(!TryGetComponent<TextToSpeech>(out textToSpeech))
                {
                    // Creates an instance.
                    textToSpeech = TextToSpeech.Instance;
                }
            }

            // If the save system speech component is not set, try to get it.
            if (saveSystem == null)
            {
                // Tries to get a component.
                if (!TryGetComponent<SaveSystem>(out saveSystem))
                {
                    // Creates an instance.
                    saveSystem = SaveSystem.Instance;
                }
            }
        }

        // // Start is called before the first frame update
        // void Start()
        // {
        // 
        // }

        // Returns the instance of the save system.
        public static SystemManager Instance
        {
            get
            {
                // Checks to see if the instance exists. If it doesn't, generate an object.
                if (instance == null)
                {
                    // Makes a new settings object.
                    GameObject go = new GameObject("System Manager (singleton)");

                    // Adds the instance component to the new object.
                    instance = go.AddComponent<SystemManager>();
                }

                // returns the instance.
                return instance;
            }
        }

        // Gets the text from the language file.
        public string GetLanguageText(string key)
        {
            // Checks if the manager is set.
            if (languageManager == null)
                languageManager = LanguageManager.Instance;

            // Set the result.
            string result = languageManager.GetLanguageText(key);

            return result;
        }
    }
}

