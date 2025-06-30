namespace Prototype.CardComponents
{
    public interface ICardComponent
    {
        public string DisplayName { get; }
        public string[] Aliases { get; }

        public void Initialize(string[] args);
    }
}