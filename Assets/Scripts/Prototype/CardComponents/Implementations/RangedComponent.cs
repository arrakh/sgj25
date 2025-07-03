
using System;
using Utilities;

namespace Prototype.CardComponents.Implementations
{
    
    public class RangedComponent : CardComponent, IMitigateWeaponDegrade, IOnDestroyMonsterWithWeapon
    {
        public Action<CardComponent> OnUpdateVisual { get; }

        private int durability = 1;
        
        public override string DisplayName => $"Ranged {durability}";
        public override string Description => $"Doesn't get weaker when fighting weaker enemies. Breaks in {durability} shots";
        public override string[] Aliases => new[] {"ranged", "range"};

        protected override void OnInitialize(string[] args)
        {
            durability = args.GetInt(0, 1);
        }

        public void Mitigate(ref int newWeaponValue)
        {
            newWeaponValue = cardInstance.Data.value;
            OnUpdateVisual?.Invoke(this);
        }

        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon)
        {
            durability--;
            if (durability <= 0) arena.UnequipWeapon();
            else RaiseUpdateEvent();
        }

    }
}