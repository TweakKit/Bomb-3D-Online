using System;

namespace ZB.Gameplay
{
    public enum MapItemType
    {
        BrickDecor1 = 1,
        BrickDecor2 = 2,
        BrickBox = 3,
        ChestBox = 4,
    }

    public abstract class MapItemModel : EntityModel
    {
        #region Members

        protected MapItemType _mapItemType;
        protected int _hp;
        protected int _level;

        [NonSerialized]
        public float currentHP;
        [NonSerialized]
        public HitBy hitBy;

        #endregion Members

        #region Properties

        public Action GetDamagedEvent { get; set; } = () => { };
        public Action DieEvent { get; set; } = () => { };

        public bool IsBrick => MapItemType == MapItemType.BrickBox || MapItemType == MapItemType.BrickDecor1 || MapItemType == MapItemType.BrickDecor2;
        public bool IsChest => MapItemType == MapItemType.ChestBox;
        public MapItemType MapItemType => _mapItemType;
        public int HP => _hp;
        public int Level => _level;
        public bool IsDead => currentHP <= 0;
        public override Enum Type => _mapItemType;

        public MapItemEffectType ExplosionEffectType
        {
            get
            {
                switch (_mapItemType)
                {
                    case MapItemType.BrickBox:
                        return MapItemEffectType.BrickBoxTileExplosionEffect;
                    case MapItemType.BrickDecor1:
                    case MapItemType.BrickDecor2:
                        return MapItemEffectType.BrickDecorTileExplosionEffect;
                    case MapItemType.ChestBox:
                        return MapItemEffectType.ChestBoxTileExplosionEffect;
                    default:
                        return MapItemEffectType.BrickBoxTileExplosionEffect;
                }
            }
        }

        #endregion Properties

        #region Class Methods

        public MapItemModel(string name, string description, MapItemType mapItemType, int hp, int level) : base(name, description)
        {
            _mapItemType = mapItemType;
            _hp = hp;
            _level = level;
            currentHP = hp;
        }

        #endregion Class Methods
    }
}