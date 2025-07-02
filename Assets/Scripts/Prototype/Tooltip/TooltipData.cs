namespace Prototype.Tooltip
{
    public struct TooltipData
    {
        public readonly string title;
        public readonly string content;

        public TooltipData(string title, string content)
        {
            this.title = title;
            this.content = content;
        }
    }
}