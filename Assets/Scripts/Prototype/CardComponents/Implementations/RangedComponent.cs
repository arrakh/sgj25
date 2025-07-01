
using Utilities;

namespace Prototype.CardComponents.Implementations
{
    
    public class RangedComponent : ICardComponent
    {
        private int durability = 1;
        
        public string DisplayName => $"Ranged {durability}";
        public string[] Aliases => new[] {"ranged"};
        
        public void Initialize(string[] args)
        {
            durability = args.GetInt(0, 1);
        }
    }
}