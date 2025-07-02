namespace Prototype.Tooltip
{
    public interface ITooltipElement
    {
        public bool HasData();
        public TooltipData GetData();
        
        public int UniqueId { get; } 
    }
}