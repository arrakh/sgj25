using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    [CreateAssetMenu(menuName = "Sprite Database")]
    public class SpriteDatabase : ScriptableObject
    {
        private static Dictionary<string, Sprite> _sprites = new();

        [Serializable]
        public class Entry
        {
            public string id;
            public Sprite sprite;
        }

        public Entry[] sprites;

        public void Initialize()
        {
            foreach (var sprite in sprites)
            {
                if (_sprites.ContainsKey(sprite.id))
                    throw new Exception($"COULD NOT REGISTER ID {sprite.id} IT HAS A DUPLICATE");

                _sprites[sprite.id] = sprite.sprite;
            }
        }

        public static Sprite Get(string id)
        {
            if (!_sprites.ContainsKey(id))
                throw new Exception($"CANNOT FIND SPRITE ID {id}");
            
            return _sprites[id];
        }
    }
}