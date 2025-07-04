using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;

namespace Prototype
{
    public class ResultCardDisplay : MonoBehaviour
    {
        public Image icon;
        public Image background;
        public TextMeshProUGUI valueText;
        public TextMeshProUGUI scoreText;
        public float scoreYDistance = 10;

        public void Setup(CardData data, int score)
        {
            valueText.text = data.value.ToString();
            icon.sprite = SpriteDatabase.Get(data.spriteId);
            background.sprite = data.type.GetBackground();

            bool hasScore = score > 0;
            scoreText.gameObject.SetActive(hasScore);

            if (!hasScore) return;
            
            scoreText.text = $"+{score}";
            var randOffsetRange = scoreYDistance / 5f;
            scoreText.transform.DOLocalMoveY(scoreYDistance + Random.Range(-randOffsetRange, randOffsetRange), Random.Range(2.2f, 2.6f)).SetEase(Ease.OutQuart);
            scoreText.DOColor(Color.clear, 1f).SetEase(Ease.OutQuart).SetDelay(2f);
        }
    }
}