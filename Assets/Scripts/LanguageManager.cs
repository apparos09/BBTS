using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BBTS
{
    // The language.
    public enum language { none, english }

    // A marker used to mark text that is not loaded from the language file.
    public class LanguageManager : MonoBehaviour
    {   
        // The instance of the language marker.
        private static LanguageManager instance;

        // The file reader for the language manager.
        public FileReader fileReader;

        // The language text.
        // The 'key' is used to determine the line set to said identifier.
        private Dictionary<string, string> langText = new Dictionary<string, string>();

        // The language the game is set to.
        private language setLanguage = language.english;

        // If set to 'true', the text is translated.
        public bool translateText = false;

        // The color used for marking text that wasn't repalced with language file content. 
        [HideInInspector]
        public Color noLoadColor = Color.red;

        // If the text colour should be changed.
        public const bool CHANGE_TEXT_COLOR = true;

        // The constructor
        private LanguageManager()
        {
            // ...
        }

        // Awake is called when the script is loaded.
        private void Awake()
        {
            // Instance saving.
            if (instance == null)
            {
                instance = this;

                // Creates a file reader.
                if(fileReader == null)
                    fileReader = new FileReader();

                // Don't destroy the language manager on load.
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
                return;
            }
        }

        // Returns the instance of the language marker.
        public static LanguageManager Instance
        {
            get
            {
                // Checks to see if the instance exists. If it doesn't, generate an object.
                if (instance == null)
                {
                    // Makes a new settings object.
                    GameObject go = new GameObject("Language Marker (singleton)");

                    // Adds the instance component to the new object.
                    instance = go.AddComponent<LanguageManager>();
                }

                // returns the instance.
                return instance;
            }
        }

        // Gets the set language.
        public language Language
        {
            get { return setLanguage; }
        }

        // Checks if a language is set.
        public bool IsLanguageSet()
        {
            return setLanguage != language.none;
        }

        // Returns 'true' if text should be translated, and if the language is set.
        public bool TranslateAndLanguageSet()
        {
            return translateText && IsLanguageSet();
        }

        // Loads the language.
        private bool LoadLanguage(language langSet)
        {
            // Clear the language text.
            langText.Clear();

            string file = "";
            string filePath = "Assets\\Resources\\Data\\Languages\\";

            // Set the language.
            setLanguage = langSet;

            // Checks which file to load.
            switch (setLanguage)
            {
                case language.english:
                default:
                    file = "language_en.txt";
                    break;
            }

            // Set the file and the file path.
            fileReader.SetFile(file, filePath);
            
            // File doesn't exist, so file can't be loaded.
            if(!fileReader.FileExists())
            {
                setLanguage = language.none;
                return false;
            }

            // Read the file.
            fileReader.ReadFile();

            // Goes through each line.
            foreach(string line in fileReader.lines)
            {
                // Splits the string by tab.
                string[] str = line.Split('\t');

                // Sets the text.
                if (str.Length >= 2)
                    langText.Add(str[0], str[1]);
                else if (str.Length == 1)
                    langText.Add(str[0], string.Empty);


                // NOTE: by default, this adds quotation marks on the end of the string.
                // As such, the trims need to happen after taking the string out.
                // Note that if you intended to have quotes around a message, that would need a workaround.

                // Remove spaces and quotation marks on the end of the string.
                string temp = langText[str[0]];

                // Remove white spaces.
                temp = temp.Trim();

                // Remove quotation marks on the edges.
                temp = temp.Trim('\"');

                // Replace triple-elipses (…) with three periods (...). They can't be displayed for some reason.
                temp = temp.Replace("\uFFFD", "...");

                // Put temp back in lang text.
                langText[str[0]] = temp;
            }

            // Data loaded successfully.
            return true;
        }

        // Loads the English language.
        public bool LoadEnglish()
        {
            return LoadLanguage(language.english);
        }

        // Marks the provided text object.
        public void MarkText(TMP_Text text)
        {
            // If the text color should be changed.
            if(CHANGE_TEXT_COLOR)
                text.color = noLoadColor;
        }

        // Translates the text by 
        public string GetLanguageText(string key)
        {
            // Key not provided.
            if (key == "")
                return "";

            // The resulting string.
            string result = string.Empty;

            // Checks if the key is in the list.
            if (langText.ContainsKey(key))
            {
                // Set the string
                result = langText[key];
            }

            return result;
        }

        // Translates the text using the provided key.
        // If the language file isn't loaded, then the text is marked using the noLoad colour.
        public bool TranslateText(TMP_Text text, string key, bool markIfFailed)
        {
            // The translation result.
            bool result = false;

            // Checks if the key is in the list.
            if (langText.ContainsKey(key) && key != string.Empty)
            {
                // Set the text.
                text.text = GetLanguageText(key);

                // Successful result.
                result = true;
            }
            else
            {
                // Since the game is only in English, this will always return false.
                if (markIfFailed)
                    MarkText(text);

                result = false;
            }

            return result;
        }
    }
}