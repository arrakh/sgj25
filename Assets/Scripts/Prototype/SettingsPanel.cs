using System;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Prototype
{
    public class SettingsPanel : MonoBehaviour
    {
        const string SFX_VOLUME = "sfx-volume";
        const string BGM_VOLUME = "bgm-volume";
        
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Slider sfxSlider, bgmSlider;

        private bool hasPressedTwice = false;

        private void Start()
        {
            var sfx = PlayerPrefs.GetFloat(SFX_VOLUME, 0.5f);
            var bgm = PlayerPrefs.GetFloat(BGM_VOLUME, 0.5f);
            
            Audio.SetSfxVolume(sfx);
            Audio.SetBgmVolume(bgm);

            sfxSlider.value = sfx;
            bgmSlider.value = bgm;

            sfxSlider.onValueChanged.AddListener(OnSfxChange);
            bgmSlider.onValueChanged.AddListener(OnBgmChange);
            
            deleteButton.onClick.AddListener(DeleteProgress);
        }

        private void OnSfxChange(float value)
        {
            Audio.SetSfxVolume(value);
            PlayerPrefs.SetFloat(SFX_VOLUME, value);
        }

        private void OnBgmChange(float value)
        {
            Audio.SetBgmVolume(value);
            PlayerPrefs.SetFloat(BGM_VOLUME, value);
        }

        public void DeleteProgress()
        {
            if (!hasPressedTwice)
            {
                hasPressedTwice = true;
                warningText.text = LocalizationManager.Localize("reset-confirm");
                return;
            }
            
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("Menu");
        }
    }
}