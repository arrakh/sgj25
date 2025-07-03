using TMPro;
using UnityEngine;

namespace Prototype
{
    //Shouldn't even inherit from Card if it wasnt too monolithic
    public class DeckCardVisual : CardVisual
    {
        [SerializeField] private TextMeshProUGUI hypeLabel;
        [SerializeField] private Color goodHype, badHype;
        
        //Bad because copied from LibraryCard but this is a systemic issue
        protected override void OnDisplay(CardInstance instance)
        {
            base.OnDisplay(instance);

            hypeLabel.text = instance.Data.cost.ToString();
            hypeLabel.color = instance.Data.cost > 0 ? goodHype : badHype;
        }
    }
}