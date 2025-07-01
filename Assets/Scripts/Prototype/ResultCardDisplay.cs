using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Prototype
{
    public class ResultCardDisplay : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI valueText;

        public Sprite weapon, monster, potion;

        public void Setup(CardData data)
        {
            valueText.text = data.value.ToString();
            icon.sprite = data.type switch
            {
                CardType.Weapon => weapon,
                CardType.Monster => monster,
                CardType.Potion => potion,
                _ => null
            };
        }
    }
}