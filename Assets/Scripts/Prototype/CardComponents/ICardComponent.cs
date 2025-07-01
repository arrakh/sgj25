namespace Prototype.CardComponents
{
    public interface ICardComponent 
    {
        public string DisplayName { get; }
        public string Description { get; }
        public string[] Aliases { get; }

        public void Initialize(CardInstance instance, string[] args);
    }
}