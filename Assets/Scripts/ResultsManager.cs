using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace BBTS
{
    // The results manager.
    public class ResultsManager : MonoBehaviour
    {
        // The title text.
        public TMP_Text titleText;

        // The save feedback text for when the game ends.
        public TMP_Text saveFeedbackText;

        [Header("Stats")]

        // The final score.
        public TMP_Text scoreText;

        // The rooms cleared text.
        public TMP_Text roomsClearedText;

        // The total time text.
        public TMP_Text totalTimeText;

        // The total turns text.
        public TMP_Text totalTurnsText;

        // The questions correct text.
        public TMP_Text questionsCorrectText;

        // The questions asked (no repeats) text.
        public TMP_Text questionsAsked;

        // The text for the final level.
        public TMP_Text finalLevelText;

        [Header("Stats/Move Text")]
        // Text for the move title.
        public TMP_Text moveSubtitleText;

        // Move 0 display text.
        public TMP_Text move0Text;

        // Move 1 display text.
        public TMP_Text move1Text;

        // Move 2 display text.
        public TMP_Text move2Text;

        // Move 3 display text.
        public TMP_Text move3Text;

        [Header("Buttons")]
        // The finish button.
        public TMP_Text finishButtonText;

        [Header("Animations")]
        // If transitions should be used.
        public bool useTransitions = true;

        // The scene transition.
        public SceneTransition sceneTransition;

        // Awake is caleld when a script instance is being loaded.
        private void Awake()
        {
            // Turns off the entrance animation if scene transitions shouldn't be used.
            sceneTransition.useSceneEnterAnim = useTransitions;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Labels for translation.
            string titleLabel = "Results";
            string scoreLabel = "Final Score";
            string roomsClearedLabel = "Rooms Cleared";
            string totalTimeLabel = "Total Time";
            string totalTurnsLabel = "Total Turns";
            string questionsCorrectLabel = "Questions Correct";
            string questionsAskedLabel = "Questions Asked";
            string noRepeatsLabel = "No Repeats";
            string finalLevelLabel = "Final Level";
            string finalMovesLabel = "Final Moves";

            // The main menu title text.
            string finishLabel = "Finish";

            // The speak key for the title.
            string titleSpeakKey = "";

            // The language manager text.
            LanguageManager lm = LanguageManager.Instance;

            // Translate the text.
            if (lm.TranslateAndLanguageSet())
            {
                titleSpeakKey = "kwd_results";
                titleLabel = lm.GetLanguageText(titleSpeakKey);

                scoreLabel = lm.GetLanguageText("kwd_finalScore");
                roomsClearedLabel = lm.GetLanguageText("kwd_roomsCleared");
                totalTimeLabel = lm.GetLanguageText("kwd_totalTime");
                totalTurnsLabel = lm.GetLanguageText("kwd_totalTurns");
                questionsCorrectLabel = lm.GetLanguageText("kwd_questionsCorrect");
                questionsAskedLabel = lm.GetLanguageText("kwd_questionsAsked");
                noRepeatsLabel = lm.GetLanguageText("kwd_noRepeats");
                finalLevelLabel = lm.GetLanguageText("kwd_finalLevel");
                finalMovesLabel = lm.GetLanguageText("kwd_finalMoves");

                finishLabel = lm.GetLanguageText("kwd_finish");
            }
            else
            {
                lm.MarkText(titleText);
                lm.MarkText(saveFeedbackText);

                lm.MarkText(scoreText);
                lm.MarkText(roomsClearedText);
                lm.MarkText(totalTimeText);
                lm.MarkText(totalTurnsText);
                lm.MarkText(questionsCorrectText);
                lm.MarkText(questionsAsked);
                lm.MarkText(finalLevelText);
                lm.MarkText(moveSubtitleText);

                lm.MarkText(move0Text);
                lm.MarkText(move1Text);
                lm.MarkText(move2Text);
                lm.MarkText(move3Text);

                lm.MarkText(finishButtonText);
            }

            // Change out titles and buttons with translated label.
            titleText.text = titleLabel;
            moveSubtitleText.text = finalMovesLabel;

            finishButtonText.text = finishLabel;

            // Change out button text with translated.

            // Finds the results object.
            ResultsData rd = FindObjectOfType<ResultsData>();

            // Results object has been found.
            if(rd != null)
            {
                // Final score
                scoreText.text = scoreLabel + ": " + rd.finalScore;
                
                // Rooms cleared.
                roomsClearedText.text = roomsClearedLabel + ": " + rd.roomsCompleted.ToString() + "/" + rd.roomsTotal.ToString();

                // Total time.
                {
                    // Calculates the total time, limiting it to 99 miuntes and 59 seconds.
                    // Max Time = 60 * 99 + 59 = 5940 + 59 = 5999 [99:59]
                    float totalTime = Mathf.Clamp(rd.totalTime, 0, 5999.0F); // total time in seconds.

                    // NEW - USES MODULUS //
                    float minutes = Mathf.Floor(totalTime / 60.0F); // minutes (floor round to remove seconds).
                    float seconds = Mathf.Ceil(totalTime - (minutes * 60.0F)); // seconds (round up to remove nanoseconds).

                    // Sets the text.
                    totalTimeText.text = totalTimeLabel + ": " + minutes.ToString("00") + ":" + seconds.ToString("00");

                }

                // Total turns.
                totalTurnsText.text = totalTurnsLabel + ": " + rd.totalTurns.ToString();

                // Questions Correct Text
                questionsCorrectText.text = questionsCorrectLabel + ": " + 
                    rd.questionsCorrect.ToString() + "/" + rd.questionsUsed.ToString();

                // Questions Correct (No Repeats) Text
                // questionsAsked.text = questionsCorrectLabel + " (" + noRepeatsLabel + "): " +
                //     rd.questionsCorrectNoRepeats.ToString() + "/" + rd.questionsUsedNoRepeats.ToString();

                // Questions Asked (No Repeats) Text
                questionsAsked.text = questionsAskedLabel + " (" + noRepeatsLabel + "): " + 
                    rd.questionsUsedNoRepeats.ToString();

                // Final player level
                finalLevelText.text = finalLevelLabel + ": " + rd.finalLevel.ToString();

                // Move text.
                move0Text.text = rd.move0;
                move1Text.text = rd.move1;
                move2Text.text = rd.move2;
                move3Text.text = rd.move3;

                // Destroy the object.
                Destroy(rd.gameObject);
            }

            // The system manager.
            SystemManager system = SystemManager.Instance;

            // Say the name of the title text.
            if (GameSettings.Instance.UseTextToSpeech && titleSpeakKey != "")
            {
                // Voice the title text.
                system.textToSpeech.SpeakText(titleSpeakKey);
            }

            // Sets the save text.
            if (saveFeedbackText != null)
            {
                saveFeedbackText.text = string.Empty;
                system.saveSystem.feedbackText = saveFeedbackText;
            }
            else
            {
                // Just empty out the string.
                saveFeedbackText.text = string.Empty;

                // Mark this as debug text.
                LanguageManager.Instance.MarkText(saveFeedbackText);
            }

            // Refreshes the feedback text.
            system.saveSystem.RefreshFeedbackText();
        }

        // Goes to the main menu.
        public void ToTitleScene()
        {
            SceneManager.LoadScene("TitleScene");
        }

        // Call this function to complete the game. This is called by the "finish" button.
        public void CompleteGame()
        {
            // Do not return to the main menu scene if running through the LOL platform.
            // This is because you can't have the game get repeated in the same session.
            ToTitleScene();
        }
    }
}