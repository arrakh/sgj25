
using System;
using Utilities;

namespace Prototype.CardComponents.Implementations
{
    
    public class RangedComponent : ICardComponent, IMitigateWeaponDegrade, IOnDestroyMonsterWithWeapon, IUpdateVisualHandle
    {
        public Action<ICardComponent> OnUpdateVisual { get; }

        private int durability = 1;
        
        public string DisplayName => $"Ranged {durability}";
        public string Description => $"Doesn't get weaker when fighting weaker enemies. Breaks in {durability} shots";
        public string[] Aliases => new[] {"ranged", "range"};

        private CardInstance instance;

        public void Initialize(CardInstance cardInstance, string[] args)
        {
            instance = cardInstance;
            durability = args.GetInt(0, 1);
        }

        public void Mitigate(ref int newWeaponValue)
        {
            newWeaponValue = instance.Data.value;
            OnUpdateVisual?.Invoke(this);
        }

        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon)
        {
            durability--;
            if (durability <= 0) arena.UnequipWeapon();
        }

    }
}