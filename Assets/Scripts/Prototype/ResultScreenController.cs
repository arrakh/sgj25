using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Prototype
{
    public class ResultScreenController : MonoBehaviour
    {
        private const int SCORE_COST_MULT = 50;

        [Header("UI References")] public TextMeshProUGUI resultText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI targetScoreText;
        public Transform defeatedContainer;
        public Transform remainingContainer;
        public GameObject resultCardPrefab;
        public Button nextButton;
        public ScrollRect defeatedView, remainingView;

        [Header("Effect References")] // assign panel utama result screen
        public GameObject confettiPrefab; // assign particle system confetti

        [Header("Animation Settings")] public float delayBetweenCards = 0.3f;

        private List<GameObject> spawned = new();

        public int Score { get; private set; }

        private bool done;

        private void Awake()
        {
            nextButton.onClick.AddListener(() => done = true);
        }

        public IEnumerator PlayResult(GameResult gameResult, int targetScore)
        {
            done = false;

            ResetState();
            
            nextButton.gameObject.SetActive(false);

            var key = gameResult.win ? "you-lost" : "you-win";
            resultText.text = LocalizationManager.Localize(key);
            Score = 0;
            scoreText.text = "0";
            targetScoreText.text = targetScore.ToString();

            foreach (var data in gameResult.enemyDefeated)
                yield return ShowCardAndAddScore(gameResult.win, data.Data, defeatedContainer, defeatedView);

            foreach (var data in gameResult.itemsLeft)
                yield return ShowCardAndAddScore(gameResult.win, data.Data, remainingContainer, remainingView);

            // 🎉 Confetti di akhir
            SpawnConfetti();

            nextButton.gameObject.SetActive(true);

            yield return new WaitUntil(() => done);
        }

        private void ResetState()
        {
            foreach (var obj in spawned)
                Destroy(obj);
            
            spawned.Clear();
        }

        private IEnumerator ShowCardAndAddScore(bool win, CardData data, Transform container, ScrollRect rect)
        {
            var cardScore = !win ? 0 : Mathf.Abs(data.cost) * SCORE_COST_MULT;

            // 🔢 Score naik animasi
            var startValue = Score;
            var endValue = Score + cardScore;
            Score = endValue;

            var cardObj = Instantiate(resultCardPrefab, container);
            var display = cardObj.GetComponent<ResultCardDisplay>();
            display.Setup(data, cardScore);
            spawned.Add(cardObj);

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
                var confetti = Instantiate(confettiPrefab);

                // Cari Canvas (harus Canvas UI, bukan world)
                var canvas = GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    // Set parent ke canvas
                    confetti.transform.SetParent(canvas.transform, false);

                    var rect = confetti.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        // Pindahkan ke bagian atas layar
                        var canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
                        rect.anchoredPosition = new Vector2(0f, canvasHeight / 2f); // di tengah atas
                    }
                    else
                    {
                        Debug.LogWarning("Confetti prefab tidak punya RectTransform.");
                    }

                    var ps = confetti.GetComponent<ParticleSystem>();
                    if (ps != null) ps.Play();

                    Destroy(confetti, 0.5f);
                }
            }
        }
    }
}