using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Prototype
{
    public class ResultScreenManager : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI scoreText;
        public Transform defeatedContainer;
        public Transform remainingContainer;
        public GameObject resultCardPrefab;

        [Header("Effect References")]
        public RectTransform panelToShake;      // assign panel utama result screen
        public GameObject confettiPrefab;       // assign particle system confetti

        [Header("Animation Settings")]
        public float delayBetweenCards = 0.3f;

        private int score = 0;

        public void ShowResult(bool isAlive, List<CardData> defeated, List<CardData> remaining)
        {
            resultText.text = isAlive ? "You lived" : "You died";
            score = 0;
            scoreText.text = "Score: 0";

            StartCoroutine(PlayResult(defeated, remaining));
        }

        private IEnumerator PlayResult(List<CardData> defeated, List<CardData> remaining)
        {
            foreach (var data in defeated)
            {
                yield return ShowCardAndAddScore(data, defeatedContainer);
            }

            foreach (var data in remaining)
            {
                yield return ShowCardAndAddScore(data, remainingContainer);
            }

            // 🎉 Confetti di akhir
            SpawnConfetti();
        }

        private IEnumerator ShowCardAndAddScore(CardData data, Transform container)
        {
            GameObject cardObj = Instantiate(resultCardPrefab, container);
            var display = cardObj.GetComponent<ResultCardDisplay>();
            display.Setup(data);

            cardObj.transform.localScale = Vector3.zero;
            yield return cardObj.transform
                .DOScale(Vector3.one, 0.25f)
                .SetEase(Ease.OutBack)
                .WaitForCompletion();

            // 🔢 Score naik animasi
            int startValue = score;
            int endValue = score + data.value;
            score = endValue;

            // 🔢 Animasi naik skor + scale text
            DOTween.To(() => startValue, x =>
            {
                startValue = x;
                scoreText.text = $"Score: {startValue}";
            }, endValue, 0.3f).OnStart(() =>
            {
                // Scale up
                scoreText.rectTransform.DOScale(1.3f, 0.15f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        // Scale back to normal
                        scoreText.rectTransform.DOScale(1f, 0.15f)
                            .SetEase(Ease.OutBack);
                    });
            });
            // 💢 Shake panel
            if (panelToShake != null)
            {
                panelToShake.DOShakeAnchorPos(0.3f, 15f, 10, 90);
            }

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