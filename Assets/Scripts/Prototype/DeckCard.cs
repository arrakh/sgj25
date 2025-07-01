using TMPro;
using UnityEngine;

namespace Prototype
{
    //Shouldn't even inherit from Card if it wasnt too monolithic
    public class DeckCard : Card
    {
        [SerializeField] private TextMeshProUGUI hypeLabel;
        [SerializeField] private Color goodHype, badHype;
        
        //Bad because copied from LibraryCard but this is a systemic issue
        protected override void OnDisplay(CardData data)
        {
            base.OnDisplay(data);

            hypeLabel.text = data.cost.ToString();
            hypeLabel.color = data.cost > 0 ? goodHype : badHype;
        }
    }
}