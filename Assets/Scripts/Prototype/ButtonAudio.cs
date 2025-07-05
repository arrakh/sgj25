using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype
{
    [RequireComponent(typeof(Button))]
    public class ButtonAudio : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private string clickId = "tap";
        [SerializeField] private string hoverId = "click";
        
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            
            button.onClick.AddListener(() => Audio.PlaySfx(clickId));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Audio.PlaySfx(hoverId);
        }
    }
}