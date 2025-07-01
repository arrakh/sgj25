using TMPro;
using UnityEngine;

namespace Prototype
{
    //Shouldn't even inherit from Card if it wasnt too monolithic
    public class LibraryCard : Card
    {
        [SerializeField] private TextMeshProUGUI amountText;

        [SerializeField] private TextMeshProUGUI hypeLabel;
        [SerializeField] private Color goodHype, badHype;
        
        public int Amount => amount;
        private int amount;

        protected override void OnDisplay(CardData data)
        {
            base.OnDisplay(data);

            hypeLabel.text = data.cost.ToString();
            hypeLabel.color = data.cost > 0 ? goodHype : badHype;
        }

        //this function kinda stupid because amount data is here, should be visual only
        public void SetAmount(int newAmount)
        {
            amount = newAmount;
            amountText.text = amount.ToString();
        }
    }
}