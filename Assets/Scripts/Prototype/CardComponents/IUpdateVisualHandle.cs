using System;

namespace Prototype.CardComponents
{
    public interface IUpdateVisualHandle
    {
        public Action<ICardComponent> OnUpdateVisual { get; }
    }
}