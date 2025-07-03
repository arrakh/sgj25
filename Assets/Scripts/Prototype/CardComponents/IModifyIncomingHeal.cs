namespace Prototype.CardComponents
{
    public interface IModifyIncomingHeal
    {
        public void Modify(ArenaController arenaController, ref int heal);
    }
}