using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype
{
    public class PauseScreen : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ToggleVisibility();
        }

        private void ToggleVisibility()
        {
            var visible = gameObject.activeInHierarchy;
            gameObject.SetActive(!visible);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}