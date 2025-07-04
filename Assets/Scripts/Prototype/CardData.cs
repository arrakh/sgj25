/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using System;
using Newtonsoft.Json;

namespace Prototype
{
    [Serializable]
    public struct CardData : IEquatable<CardData>
    {
        public string id;
        public string displayName;
        public string spriteId;
        public CardType type;
        public int value;
        public int cost;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)] 
        public string[] components;

        public static CardData Empty => new() {id = String.Empty};

        public bool Equals(CardData other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is CardData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (id != null ? id.GetHashCode() : 0);
        }
    }
}