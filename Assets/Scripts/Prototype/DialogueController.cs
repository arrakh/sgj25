/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Assets.SimpleLocalization.Scripts;

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

        private int currentIndex = 0;
        private bool isDialogueActive = false;
        private Coroutine dialogueRoutine;
        private bool isTyping = false;

        void Start()
        {
            LocalizationManager.Read();
            LocalizationManager.Language = "English";
            InitDialogue();
        }

        public void InitDialogue()
        {
            if (dialogueLines.Length == 0)
            {
                Debug.LogWarning("Dialogue lines is empty!");
                return;
            }

            dialoguePanel.SetActive(true);
            skipButton.gameObject.SetActive(true);
            currentIndex = 0;
            isDialogueActive = true;

            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextOrSkipTyping);

            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipDialogue);

            if (dialogueRoutine != null) StopCoroutine(dialogueRoutine);
            dialogueRoutine = StartCoroutine(PlayDialogue());
        }

        private IEnumerator PlayDialogue()
        {
            
            while (currentIndex < dialogueLines.Length)
            {
                narationImage.sprite = dialogueLines[currentIndex].image;
                var translated = LocalizationManager.Localize(dialogueLines[currentIndex].content);
                yield return StartCoroutine(TypeLine(translated));
                yield return new WaitForSeconds(delayAfterLine);
                currentIndex++;
            }

            EndDialogue();
        }

        private IEnumerator TypeLine(string line)
        {
            dialogueText.text = "";
            isTyping = true;

            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }

            isTyping = false;
        }

        private void NextOrSkipTyping()
        {
            if (isTyping)
            {
                Debug.Log("testtt");
                // Langsung tampilkan kalimat lengkap jika masih ngetik
                StopAllCoroutines();
                var translated = LocalizationManager.Localize(dialogueLines[currentIndex].content);
                dialogueText.text = translated;
                isTyping = false;

                // Restart coroutine lanjut dialog (auto next)
                dialogueRoutine = StartCoroutine(WaitAfterForceComplete());
            }
        }

        private IEnumerator WaitAfterForceComplete()
        {
            yield return new WaitForSeconds(delayAfterLine);
            currentIndex++;
            if (currentIndex < dialogueLines.Length)
            {
                dialogueRoutine = StartCoroutine(PlayDialogue());
            }
            else
            {
                EndDialogue();
            }
        }

        public void SkipDialogue()
        {
            if (dialogueRoutine != null)
            {
                StopCoroutine(dialogueRoutine);
            }
            StopAllCoroutines();
            EndDialogue();
        }

        private void EndDialogue()
        {
            isDialogueActive = false;
            dialoguePanel.SetActive(false);
            skipButton.gameObject.SetActive(false);

        }
    }
}