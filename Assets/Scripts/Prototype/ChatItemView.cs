/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Prototype
{
    public class ChatItemView : MonoBehaviour
    {
        public Image avatarImage;
        public TextMeshProUGUI messageText;

        public void Setup(ChatMessage msg)
        {
            if (avatarImage != null)
                avatarImage.sprite = msg.avatar;

            if (messageText != null)
                messageText.text = $"<color=#FFD700><b>{msg.username}:</b></color> {msg.message}";
        }
    }
}