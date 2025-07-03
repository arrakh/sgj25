using System;

namespace Prototype.CardComponents
{
    public abstract class CardComponent
    {
        public event Action<CardComponent> OnUpdated;
        
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract string[] Aliases { get; }

        protected CardInstance cardInstance;

        protected void RaiseUpdateEvent() => OnUpdated?.Invoke(this);

        public void Initialize(CardInstance instance, string[] args)
        {
            cardInstance = instance;
            OnInitialize(args);
        }

        protected virtual void OnInitialize(string[] args)
        {
            
        }
    }
}