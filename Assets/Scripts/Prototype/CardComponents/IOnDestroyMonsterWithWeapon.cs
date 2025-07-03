namespace Prototype.CardComponents
{
    public interface IOnDestroyMonsterWithWeapon
    {
        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon);
    }
}