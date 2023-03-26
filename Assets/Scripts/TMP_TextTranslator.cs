using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BBTS
{
    // Translates a TMP_Text object's text using the provided key.
    public class TMP_TextTranslator : MonoBehaviour
    {
        // The text object.
        public TMP_Text text;

        // The translation key.
        public string key = "";

        // Marks text if the language file is not loaded.
        public bool markIfFailed = true;

        // Start is called before the first frame update
        void Start()
        {
            // Grabs the text mesh pro component from the object this script is attached to.
            if (text == null)
                text = GetComponent<TextMeshPro>();


            // Translate the text.
            if(text != null)
            {
                Translate();
            }
        }

        // Translate the text.
        public bool Translate()
        {
            // The result variable.
            bool result = false;

            // Checks if the text and the key both exists.
            if (text != null && key != string.Empty)
            {
                // Mark the text.
                result = LanguageManager.Instance.TranslateText(text, key, true);
            }
            else
            {
                result = false;
            }

            return result;
        }

        // Speaks out the provided text.
        public void SpeakText()
        {
            // TTS is no longer available, so this function has nothing.
            // Checks if the TTS is set up, if the TTS is active, and if the key string exists.
            if(GameSettings.Instance.UseTextToSpeech && key != string.Empty)
            {
                // Read out the text.
                SystemManager.Instance.textToSpeech.SpeakText(key);
            }


        }
    }
}