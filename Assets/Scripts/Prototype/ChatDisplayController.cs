/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class ChatDisplayController : MonoBehaviour
    {
        [Header("UI Components")]
        public RectTransform chatContainer;            // Content dari ScrollView
        public GameObject chatItemPrefab;              // Prefab chat item
        public float messageInterval = 1.5f;           // Jeda antar pesan

        [Header("Settings")]
        public int maxVisibleMessages = 15;            // Maksimum pesan yang ditampilkan                
        public List<ChatMessage> mockMessages = new List<ChatMessage>();


        private void Start()
        {

            if (mockMessages.Count > 0)
            {
                StartCoroutine(RandomChatLoop());
            }
            else
            {
                Debug.LogWarning("No messages loaded from JSON.");
            }
        }


        private IEnumerator RandomChatLoop()
        {
            while (true)
            {
                int randomIndex = Random.Range(0, mockMessages.Count);
                DisplayMessage(mockMessages[randomIndex]);
                yield return new WaitForSeconds(messageInterval);
            }
        }

        private void DisplayMessage(ChatMessage msg)
        {
            GameObject itemGO = Instantiate(chatItemPrefab, chatContainer);
            ChatItemView view = itemGO.GetComponent<ChatItemView>();
            if (view != null) view.Setup(msg);

            // Destroy message jika lebih dari batas maksimum
            if (chatContainer.childCount > maxVisibleMessages)
            {
                Transform oldest = chatContainer.GetChild(0);
                Destroy(oldest.gameObject);
            }

            // Scroll otomatis ke bawah setelah frame selesai
            StartCoroutine(ScrollToBottomNextFrame());
        }

        private IEnumerator ScrollToBottomNextFrame()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();

            ScrollRect scroll = chatContainer.GetComponentInParent<ScrollRect>();
            if (scroll != null)
            {
                scroll.verticalNormalizedPosition = 0f;
            }
        }
    }
}