using UnityEngine;

namespace Prototype.CardComponents.Implementations
{
    public class DrainComponent : CardComponent, IOnDestroyMonsterWithWeapon
    {
        public override string DisplayName => $"Drain";
        public override string Description => $"When defeating enemies, Heal for half that enemy's health rounded down";
        public override string[] Aliases => new[] {"drain"};

        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon)
        {
            var health = Mathf.FloorToInt(monster.Data.value/2f);
            arena.AddHealth(health);
        }
    }
}