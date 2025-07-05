using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] private GameObject holder;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ToggleVisibility();
        }

        private void ToggleVisibility()
        {
            var visible = holder.activeInHierarchy;
            holder.SetActive(!visible);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}