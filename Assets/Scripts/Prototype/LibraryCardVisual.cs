using TMPro;
using UnityEngine;

namespace Prototype
{
    //Shouldn't even inherit from Card if it wasnt too monolithic
    public class LibraryCardVisual : CardVisual
    {
        [SerializeField] private TextMeshProUGUI amountText;

        [SerializeField] private TextMeshProUGUI hypeLabel;
        [SerializeField] private Color goodHype, badHype;
        

        protected override void OnDisplay(CardInstance instance)
        {
            base.OnDisplay(instance);

            hypeLabel.text = instance.Data.cost.ToString();
            hypeLabel.color = instance.Data.cost > 0 ? goodHype : badHype;
        }

        public void SetAmount(int newAmount)
        {
            amountText.text = $"x{newAmount}";
        }
    }
}