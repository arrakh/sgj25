using UnityEngine;

namespace Prototype.CardComponents.Implementations
{
    public class FierceComponent : CardComponent, IModifyOverkillDamage
    {
        public override string DisplayName => "Fierce";

        public override string Description =>
            "When attacked by a weapon with lower power than its own, double the damage dealt to the player";

        public override string[] Aliases => new []{"fierce"};
        
        public void Modify(ArenaController arena, ref int overkillDamage)
        {
            var from = overkillDamage;
            overkillDamage *= 2;
            Debug.Log($"Doubled damage from {from} to {overkillDamage}");
        }
    }
}