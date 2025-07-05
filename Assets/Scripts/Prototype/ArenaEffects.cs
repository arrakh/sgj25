using System;
using System.Collections.Generic;
using DG.Tweening;
using Prototype.CardComponents.Implementations;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.FlyingTexts;
using Random = UnityEngine.Random;

namespace Prototype
{
    public class ArenaEffects : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Image vignette;
        [SerializeField] private Image character;
        [SerializeField] private RectTransform healthRect;
        [SerializeField] private RectTransform weaponValueRect;
        [SerializeField] private Image mimicImage;
        [SerializeField] private EquippedInfo equippedWeapon;
        [SerializeField] private EquippedInfo equippedTool;
        [SerializeField] private FlyingTextController flyingText;

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

        [Header("Particle Effects")] 
        [SerializeField] private ParticleSystem healBurst;
        [SerializeField] private ParticleSystem otherBurst;
        [SerializeField] private ParticleSystem hitImpact;

        private List<Tween> currentTweens = new();

        public void OnHealthChanged(int from, int to)
        {
            if (from == to) return;
            bool isHealed = from < to;
            var difference = Mathf.Abs(from - to);
            var text = $"{(isHealed ? '+' : '-')}{difference}";
            var flyText = flyingText.Create(text, healthRect.position, 2f);
            flyText.color = isHealed ? Color.green : Color.red;
            
            var strength = Mathf.Clamp01(difference / (float) damageMax);
            if (isHealed) DoHealEffect(strength);
            else DoDamageEffect(strength);
        }

        public void OnCardConsumed(CardVisual card)
        {
            switch (card.Type)
            {
                case CardType.Weapon: Audio.PlaySfx("bling"); break;
                case CardType.Monster: Audio.PlaySfx("slash"); Audio.PlaySfx("monster-dead"); break;
                case CardType.Tool: Audio.PlaySfx("bling"); break;
                case CardType.Item: 
                    var hasHeal = card.Instance.HasComponent<HealComponent>();
                    Audio.PlaySfx(hasHeal ? "potion" : "bling"); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return;
            Mimic(mimicImage, card.Icon);

            mimicImage.gameObject.SetActive(true);
            mimicImage.color = Color.white;

            DOTween.Kill(mimicImage);

            switch (card.Type)
            {
                case CardType.Weapon:
                    equippedWeapon.SetShowIcon(false);
                    mimicImage.transform.DOScale(1.2f, 0.4f)
                        .SetEase(Ease.OutQuart).OnComplete(() =>
                        {
                            mimicImage.transform.DOMove(equippedWeapon.Icon.transform.position, 0.2f)
                                .SetEase(Ease.InQuart).OnComplete(() =>
                                {
                                    mimicImage.gameObject.SetActive(false);
                                    equippedWeapon.SetShowIcon(true);
                                });
                        });
                    break;
                
                case CardType.Monster:
                    Audio.PlaySfx("slash");
                    SpawnParticle(hitImpact, mimicImage.transform.position);
                    mimicImage.transform.DOScale(1.2f, 0.4f).SetEase(Ease.OutQuart);
                    mimicImage.DOColor(Color.clear, 0.33f);
                    mimicImage.transform.DOShakePosition(0.33f, 10f);
                    
                    break;
                
                case CardType.Tool:
                    equippedTool.SetShowIcon(false);
                    mimicImage.transform.DOScale(1.2f, 0.4f)
                        .SetEase(Ease.OutQuart).OnComplete(() =>
                        {
                            mimicImage.transform.DOMove(equippedTool.Icon.transform.position, 0.2f)
                                .SetEase(Ease.InQuart).OnComplete(() =>
                                {
                                    mimicImage.gameObject.SetActive(false);
                                    equippedTool.SetShowIcon(true);
                                });
                        });
                    break;
                    
                case CardType.Item:
                    var hasHeal = card.Instance.HasComponent<HealComponent>();
                    mimicImage.transform.DOScale(1.2f, 0.4f).SetEase(Ease.OutQuart);
                    mimicImage.DOColor(Color.clear, 0.33f);
                    
                    SpawnParticle(hasHeal ? healBurst : otherBurst, mimicImage.transform.position);
                    
                    
                    break;
                default: throw new Exception($"TRYING TO SPAWN EFFECT BUT CANNOT HANDLE {card.Type}");
            }

            if (card.Instance.HasComponent<HealComponent>())
            {
                
                return;
            }
        }

        private void SpawnParticle(ParticleSystem prefab, Vector3 position)
        {
            var particle = Instantiate(prefab);
            particle.transform.position = position;
            var destroy = particle.gameObject.AddComponent<DestroyAfter>();
            destroy.Set(1f);
        }

        private void Mimic(Image src, Image dst)
        {
            dst.sprite = src.sprite;
            dst.type = src.type;
            dst.color = src.color;

            var srcRT = src.rectTransform;
            var dstRT = dst.rectTransform;

            dstRT.anchorMin = srcRT.anchorMin;
            dstRT.anchorMax = srcRT.anchorMax;
            dstRT.pivot = srcRT.pivot;
            dstRT.anchoredPosition = srcRT.anchoredPosition;
            dstRT.sizeDelta = srcRT.sizeDelta;
            dstRT.localRotation = srcRT.localRotation;
            dstRT.localScale = srcRT.localScale;
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
            var audioKey = $"damaged-{Random.Range(1, 3)}";
            Audio.PlaySfx(audioKey);
            
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