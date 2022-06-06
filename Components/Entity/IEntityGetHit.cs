namespace ZB.Gameplay
{
    public enum HitBy
    {
        Hero,
        Enemy,
        HeroBomb,
        EnemyBomb,
    }

    public interface IEntityGetHit
    {
        void GetHit(float damageValue, HitBy hitBy);
    }
}