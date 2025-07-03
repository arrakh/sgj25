using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Prototype
{
    public class ResultCardDisplay : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI valueText;

        public void Setup(CardData data)
        {
            valueText.text = data.value.ToString();
            icon.sprite = SpriteDatabase.Get(data.spriteId);
        }
    }
}