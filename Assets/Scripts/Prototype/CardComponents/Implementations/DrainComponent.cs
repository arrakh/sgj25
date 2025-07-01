using UnityEngine;

namespace Prototype.CardComponents.Implementations
{
    public class DrainComponent : ICardComponent, IOnDestroyMonsterWithWeapon
    {
        public string DisplayName => $"Drain";
        public string Description => $"When defeating enemies, Heal for half that enemy's health rounded down";
        public string[] Aliases => new[] {"ranged", "range"};

        private CardInstance instance;

        public void Initialize(CardInstance cardInstance, string[] args)
        {
            instance = cardInstance;
        }

        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon)
        {
            var health = Mathf.FloorToInt(monster.Data.value/2f);
            arena.AddHealth(health);
        }
    }
}