using System.Collections.Generic;
using Prototype.CardComponents;

namespace Prototype
{
    public class CardInstance
    {
        public CardInstance(CardData data)
        {
            this.data = data;

            if (data.components == null) return;
            foreach (var component in data.components)
            {
                var instance = CardComponentFactory.Create(this, component);
                components.Add(instance);
            }
        }

        public CardData Data => data;
        public IEnumerable<CardComponent> Components => components;

        private CardData data;
        private List<CardComponent> components = new();

        public void SetValue(int newValue)
        {
            data.value = newValue;
        }
    }
}