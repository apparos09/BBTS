using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBTS
{
    // Runs the text-to-speech for the provided speak key.
    public class SpeakOnClick : MonoBehaviour
    {
        // The speak key for the object.
        public string speakKey = "";

        // Set the speak key and call the SpeakText function.
        public void SpeakKey(string newKey)
        {
            speakKey = newKey;
            SpeakText();
        }

        // Call this function to speak the text for the object.
        public void SpeakText()
        {
            // The SDK for TTS has been removed, so the game no longer has any TTS functions.
            // ...
        }

        // This doesn't work if it's part of the UI. Use a button to call SpeakText() instead.
        // OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.
        private void OnMouseDown()
        {
            SpeakText();
        }
    }
}