using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Button = UnityEngine.UI.Button;

namespace Prototype
{
    public class ResultScreenController : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI targetScoreText;
        public Transform defeatedContainer;
        public Transform remainingContainer;
        public GameObject resultCardPrefab;
        public Button nextButton;
        public ScrollRect defeatedView, remainingView;

        [Header("Effect References")]    // assign panel utama result screen
        public GameObject confettiPrefab;       // assign particle system confetti

        [Header("Animation Settings")]
        public float delayBetweenCards = 0.3f;

        public int Score => score;

        private int score = 0;

        private bool done = false;
        
        private void Awake()
        {
            nextButton.onClick.AddListener(() => done = true);
        }

        public IEnumerator PlayResult(GameResult gameResult, int targetScore)
        {
            nextButton.gameObject.SetActive(false);

            done = false;
            resultText.text = gameResult.win ? "You lost..." : "You won!";
            score = 0;
            scoreText.text = "0";
            targetScoreText.text = targetScore.ToString();
            
            foreach (var data in gameResult.enemyDefeated)
                yield return ShowCardAndAddScore(gameResult.win, data.Data, defeatedContainer, defeatedView);
            
            foreach (var data in gameResult.itemsLeft)
                yield return ShowCardAndAddScore(gameResult.win,data.Data , remainingContainer, remainingView);

            // 🎉 Confetti di akhir
            SpawnConfetti();

            nextButton.gameObject.SetActive(true);

            yield return new WaitUntil(() => done);
        }

        private IEnumerator ShowCardAndAddScore(bool win, CardData data, Transform container, ScrollRect rect)
        {
            var cardScore = !win ? 0 : Mathf.Abs(data.cost) * 5;
            
            // 🔢 Score naik animasi
            int startValue = score;
            int endValue = score + cardScore;
            score = endValue;
            
            GameObject cardObj = Instantiate(resultCardPrefab, container);
            var display = cardObj.GetComponent<ResultCardDisplay>();
            display.Setup(data, cardScore);

            cardObj.transform.localScale = Vector3.zero;
            cardObj.transform
                .DOScale(Vector3.one, 0.25f)
                .SetEase(Ease.OutBack);

            // 🔢 Animasi naik skor + scale text
            DOTween.To(() => startValue, x =>
            {
                startValue = x;
                scoreText.text = startValue.ToString();
            }, endValue, delayBetweenCards).OnStart(() =>
            {
                // Scale up
                scoreText.rectTransform.DOScale(1.3f, 0.12f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        // Scale back to normal
                        scoreText.rectTransform.DOScale(1f, 0.33f)
                            .SetEase(Ease.OutBack);
                    });
            });
            
            rect.ScrollToBottom();
            
            yield return new WaitForSeconds(delayBetweenCards);
        }

private void SpawnConfetti()
{
    if (confettiPrefab != null)
    {
        // Instantiate prefab
        GameObject confetti = Instantiate(confettiPrefab);

        // Cari Canvas (harus Canvas UI, bukan world)
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            // Set parent ke canvas
            confetti.transform.SetParent(canvas.transform, false);

            RectTransform rect = confetti.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Pindahkan ke bagian atas layar
                float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
                rect.anchoredPosition = new Vector2(0f, canvasHeight / 2f); // di tengah atas
            }
            else
            {
                Debug.LogWarning("Confetti prefab tidak punya RectTransform.");
            }

            var ps = confetti.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }

            Destroy(confetti, 0.5f);
        }
    }
}
    }
}