using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTMPText : MonoBehaviour
    {
        [SerializeField] public string localizationKey;

        private TextMeshProUGUI text;
        
        public void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        public void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();
            text.text = LocalizationManager.Localize(localizationKey);
        }
    }
}