/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using System.Collections;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] private FadeScreen fade;
        
        void Start()
        {
            Audio.PlayBgm("menu");
        }

        public void ToGame()
        {
            StartCoroutine(ToGameCoroutine());
        }

        private IEnumerator ToGameCoroutine()
        {
            yield return fade.FadeInCoroutine(1f);
            
            SceneManager.LoadScene("Prototype");
        }

        public void ChangeLanguage(string language)
        {
            LocalizationManager.Language = language;
        }
    }
}
