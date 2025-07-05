/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */

using System.Collections;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Prototype
{
    public class DialogueController : MonoBehaviour
    {
        [Header("UI Components")] 
        public GameObject dialoguePanel;
        public TextMeshProUGUI dialogueText;
        public Button nextButton;
        public Button skipButton;
        public Image narationImage;

        [Header("Dialogue Settings")] 
        public DialogueEntry[] dialogueLines;
        public float typeSpeed = 0.005f;
        public float delayAfterLine = 2f;

        private int currentIndex;
        private bool isTyping;
        private bool isDialogueActive;
        private bool requestSkipLine;
        private bool requestEndDialogue;
        private bool dialogueEnded;
        private bool requestSkipWait;

        private Coroutine playRoutine;

        public bool DialogueEnded => dialogueEnded;

        public void Initialize()
        {
            gameObject.SetActive(false);
            
            Audio.PlayBgm("menu");

            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                Debug.LogWarning("Dialogue lines are empty!");
                return;
            }

            currentIndex = 0;
            isTyping = false;
            isDialogueActive = true;
            requestSkipLine = false;
            requestSkipWait = false;
            requestEndDialogue = false;
            dialogueEnded = false;

            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnNextPressed);

            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(OnSkipPressed);

            dialoguePanel.SetActive(true);
            skipButton.gameObject.SetActive(true);
            gameObject.SetActive(true);

            playRoutine = StartCoroutine(PlayDialogue());
        }

        private IEnumerator PlayDialogue()
        {
            while (currentIndex < dialogueLines.Length && !requestEndDialogue)
            {
                requestSkipWait = false;
                // Set visuals for this line
                narationImage.sprite = dialogueLines[currentIndex].image;
                var text = LocalizationManager.Localize(dialogueLines[currentIndex].content);

                yield return StartCoroutine(TypeLine(text));

                if (requestEndDialogue) break;

                var timer = 0f;
                while (timer < delayAfterLine && !requestEndDialogue && !requestSkipWait)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }

                currentIndex++;
            }

            EndDialogue();
        }

        private IEnumerator TypeLine(string line)
        {
            dialogueText.text = "";
            isTyping = true;
            requestSkipLine = false;

            foreach (var c in line)
            {
                if (requestSkipLine || requestEndDialogue) break;

                dialogueText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }

            dialogueText.text = line;
            isTyping = false;
            requestSkipLine = false;
        }

        private void OnNextPressed()
        {
            if (isTyping) requestSkipLine = true;
            else requestSkipWait = true;
        }

        private void OnSkipPressed()
        {
            if (!isDialogueActive) return;

            requestEndDialogue = true;


            if (playRoutine != null)
            {
                StopCoroutine(playRoutine);
                playRoutine = null;
            }

            EndDialogue();
        }

        private void EndDialogue()
        {
            if (dialogueEnded) return;
            dialogueEnded = true;
            isDialogueActive = false;

            dialoguePanel.SetActive(false);
            skipButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}