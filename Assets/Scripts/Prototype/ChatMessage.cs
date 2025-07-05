/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class ChatMessage
    {
        public string username;
        public string message;
        public Sprite avatar;

        public ChatMessage(string username, string message, Sprite avatar)
        {
            this.username = username;
            this.message = message;
            this.avatar = avatar;
        }
    }
}