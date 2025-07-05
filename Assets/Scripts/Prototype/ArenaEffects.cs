using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class ArenaEffects : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Image vignette;
        [SerializeField] private Image character;
        [SerializeField] private RectTransform healthRect;
        [SerializeField] private RectTransform weaponValueRect;

        [Header("Misc")] 
        [SerializeField] private float vignetteStrength;
        [SerializeField] private int damageMax = 10;

        [Header("Heal Effect")]
        [SerializeField] private float healDuration = 0.6f;
        [SerializeField] private Color healCharacterFromColor;
        [SerializeField] private Color healVignetteFromColor;
        [SerializeField] private float healShakeStrength = 10f;
        [SerializeField] private float healShakeDuration = 0.33f;

        [Header("Damage Effect")]
        [SerializeField] private float damageDuration = 0.6f;
        [SerializeField] private Color damageCharacterFromColor;
        [SerializeField] private Color damageVignetteFromColor;
        [SerializeField] private float damageShakeStrength = 10f;
        [SerializeField] private float damageShakeDuration = 0.33f;

        private List<Tween> currentTweens = new();

        public void OnHealthChanged(int from, int to)
        {
            if (from == to) return;
            bool isHealed = from < to;
            var strength = Mathf.Clamp01(Mathf.Abs(from - to) / (float) damageMax);
            if (isHealed) DoHealEffect(strength);
            else DoDamageEffect(strength);
        }

        public void OnWeaponValueChanged()
        {
            weaponValueRect.localScale = Vector3.one * 1.1f;
            currentTweens.Add(weaponValueRect.DOScale(Vector3.one, 0.6f));
        }

        private void ClearAllTweens()
        {
            foreach (var tween in currentTweens)
                tween.Kill();
            currentTweens.Clear();
        }
        
        private void DoHealEffect(float strength)
        {
            ClearAllTweens();
            character.color = ScaleAlpha(healCharacterFromColor, strength * vignetteStrength);
            currentTweens.Add(character.DOColor(Color.white, healDuration));

            vignette.color = ScaleAlpha(healVignetteFromColor, strength * vignetteStrength);
            currentTweens.Add(vignette.DOColor(Color.clear, healDuration));
            
            currentTweens.Add(character.transform.DOShakePosition(healShakeDuration, Vector3.up * healShakeStrength));
            
            healthRect.localScale = Vector3.one * 1.1f;
            currentTweens.Add(healthRect.DOScale(Vector3.one, healShakeDuration));
        }
        
        private void DoDamageEffect(float strength)
        {
            ClearAllTweens();
            character.color = ScaleAlpha(damageCharacterFromColor, strength * vignetteStrength);
            currentTweens.Add(character.DOColor(Color.white, damageDuration));

            vignette.color = ScaleAlpha(damageVignetteFromColor, strength * vignetteStrength);
            currentTweens.Add(vignette.DOColor(Color.clear, damageDuration));
            
            currentTweens.Add(character.transform.DOShakePosition(damageShakeDuration, Vector3.up * damageShakeStrength));
            
            healthRect.localScale = Vector3.one * 1.1f;
            currentTweens.Add(healthRect.DOScale(Vector3.one, damageShakeDuration));
        }

        private Color ScaleAlpha(Color color, float alphaScale)
        {
            return new Color(color.r, color.g, color.b, color.a * alphaScale);
        }
    }
}