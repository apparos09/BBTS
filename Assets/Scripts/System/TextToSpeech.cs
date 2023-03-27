using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// The namespace should be <YourCompany> <GameName>.
// NOTE: due to the play method changing for LOL versus the Unity Editor you cannot create a universal means of checking when the speak text is done.
namespace BBTS
{
    public class TextToSpeech : MonoBehaviour
    {
        // The instance of the text-to-speech.
        private static TextToSpeech instance;

        // The audio source for the text-to-speech.
        public AudioSource ttsAudioSource;

        // The speak key for stopping the Text-To-Speech
        public const string STOP_SPEAK_KEY = "_stop_";

        // The current key for the spoken text.
        private string currentSpeakKey = "";

        // Constructor
        private TextToSpeech()
        {

        }

        // Awake is called when the script instance is being loaded
        void Awake()
        {
            // Checks for the instance.
            if (instance == null)
            {
                instance = this;
            }

            // if the audio source has not been set, then add an audio source component.
            if (ttsAudioSource == null)
            {
                // Creates a new audio source object.
                // This will be a child of the text-to-speech object so that it can have its own tag.

                // Creates the object, and parents it to this script's object.
                GameObject newObject = new GameObject("Audio Source");
                newObject.transform.parent = gameObject.transform;

                // Gives it the tag.
                newObject.tag = GameSettings.TTS_TAG;

                // Adds the audio source to the new object.
                ttsAudioSource = newObject.AddComponent<AudioSource>();
            }

            // The audio soruce control.
            AudioSourceControl asc = null;

            // Tries to get the audio source control.
            if(!ttsAudioSource.TryGetComponent<AudioSourceControl>(out asc))
            {
                // Adds the audio source control.
                asc = ttsAudioSource.gameObject.AddComponent<AudioSourceControl>();
            }

            // Sets the audio source.
            if (asc.audioSource == null)
                asc.audioSource = ttsAudioSource;

            // Checks for the proper tag on the audio source control object.
            if (!asc.gameObject.CompareTag(GameSettings.TTS_TAG))
                asc.gameObject.tag = GameSettings.TTS_TAG;
            

        }

        // Returns the instance of the text-to-speech object.
        public static TextToSpeech Instance
        {
            get
            {
                // Tries to find the TTS.
                if (instance == null)
                    instance = FindObjectOfType<TextToSpeech>();

                // Checks to see if the instance exists. If it doesn't, generate an object.
                if (instance == null)
                {
                    instance = FindObjectOfType<TextToSpeech>(true);

                    // Generate new instance if an existing instance was not found.
                    if (instance == null)
                    {
                        // Makes a new settings object.
                        GameObject go = new GameObject("Text-to-Speech");

                        // Adds the instance component to the new object.
                        instance = go.AddComponent<TextToSpeech>();
                    }

                }

                // returns the instance.
                return instance;
            }
        }

        // Reads the text based on the provided key.
        public void SpeakText(string key)
        {
            // TTS is no longer available.
        }

        // Cancels the text that's being read by the text-to-speech.
        public void CancelSpeakText()
        {
            ttsAudioSource.Stop();
        }

        // Uses the 'stop speak key' to stop the speka text.
        public void StopSpeakText()
        {
            // Speaks the text.
            SpeakText(STOP_SPEAK_KEY);
        }

        // Gets the current speak key. If no audio is playing, an empty string ("") is returned.
        public string CurrentSpeakKey
        {
            get { return currentSpeakKey; }
        }

        // Checks if the speak text 
        private void Update()
        {
            // TODO: this does NOT work for clearing the key only when the audio is done playing.
            // The Application being paused in the background seems to mess with this.

            // Checks to see if the audio is playing. If it isn't playing, then clear the current key.
            // AudioSource.IsPlaying returns false if the audio is paused.
            // The application does not run in the background.
            // As such, when the window isn't in focus AudioSource.IsPlaying returns false.
            // So this conditional statement checks if the TTS isn't playing, and if the application is.
            if(currentSpeakKey != "" && !ttsAudioSource.isPlaying && Application.isPlaying)
            {
                currentSpeakKey = "";
            }

        }
    }
}
