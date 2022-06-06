using ZB.Model;

namespace ZB.Gameplay.PVP
{
    public class NetworkDataManager : DataManager
    {
        public override EnemyStat GetEnemyData(int level)
        {
            EnemyStat enemyStat = new EnemyStat()
            {
                id = "",
                level = level,
                hp = 500,
                speed = 3,
                hitDamage = 10,
                boomDamage = 10,
                boomPut = 1,
                boomRange = 1,
                boomEXT = 3,
                chase = true,
                chaseAOE = 1,
                lostAOE = 1,
                patrolVer = true,
                patrolHor = true,
                changeWay = true,
                patrolLoop = true,
                patrolLoopTime = 2,
                tokenRate = 1
            };

            return enemyStat;
        }

        public override ChestStat GetChestData(int level)
        {
            ChestStat chestStat = new ChestStat()
            {
                id = "",
                name = "",
                level = level,
                hp = 100,
                rate = 1
            };

            return chestStat;
        }

        public override InventoryItem GetBombDefaultData(string rarity)
        {
            InventoryItem bombDefaultItem = new InventoryItem()
            {
                itemId = "",
                itemType = "",
                tokenId = 0,
                properties = new ItemProperties()
                {
                    houseId = "",
                    characterId = "",
                    type = "",
                    level = 1,
                    colors = "",
                    element = "",
                    rarity = "",
                    hp = 0,
                    def = 0,
                    ms = 0,
                    eng = 0,
                    er = 0,
                    dmg = 10,
                    ran = 1,
                    num = 1,
                    ext = 3,
                    hv = 0
                }
            };

            return bombDefaultItem;
        }

        public override BigBombChestStat GetBigBombChestStat()
        {
            return new BigBombChestStat()
            {
                name = "Big Boom Chest",
                hp = 200,
                trapRate = 75.0f,
                trapDmg = 100,
                trapTimeBurst = 1,
                trapArea = 4,
                tokenRate = 25.0f,
                tokenDrop = 100
            };
        }
    }
}