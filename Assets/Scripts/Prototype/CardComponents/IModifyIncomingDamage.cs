namespace Prototype.CardComponents
{
    public interface IModifyIncomingDamage
    {
        public void Modify(ArenaController arenaController, ref int damage);
    }
}