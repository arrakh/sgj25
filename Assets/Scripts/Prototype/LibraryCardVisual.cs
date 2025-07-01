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
        
        public int Amount => amount;
        private int amount;

        protected override void OnDisplay(CardInstance instance)
        {
            base.OnDisplay(instance);

            hypeLabel.text = instance.Data.cost.ToString();
            hypeLabel.color = instance.Data.cost > 0 ? goodHype : badHype;
        }

        //this function kinda stupid because amount data is here, should be visual only
        public void SetAmount(int newAmount)
        {
            amount = newAmount;
            amountText.text = amount.ToString();
        }
    }
}