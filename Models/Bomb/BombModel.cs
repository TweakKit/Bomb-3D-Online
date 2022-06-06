using System;

namespace ZB.Gameplay
{
    [Serializable]
    public enum BombType
    {
        BombAries = 0,
        BombTaurus = 1,
        BombGemini = 2,
        BombCancer = 3,
        BombLeo = 4,
        BombVirgo = 5,
        BombLibra = 6,
        BombScorpio = 7,
        BombSagittarius = 8,
        BombCapricorn = 9,
        BombAquarius = 10,
        BombPisces = 11,
        BombDefault = 12,
        BombBigSecret = 13,
        BombTNTExplosive = 14,
        Count = 15,
    }

    public class BombModel : EntityModel
    {
        #region Members

        private string _element;
        private string _rarity;
        private int _damageValue;
        private int _damageRadius;
        private int _number;
        private int _lifetime;
        private int _level;
        private bool _isOwnedByHero;
        private string _colors;

        [NonSerialized]
        public BombType bombType;

        #endregion Members

        #region Properties

        public Action ExplodeEvent { get; set; } = () => { };

        public string Element => _element;
        public string Rarity => _rarity;
        public int DamageValue => _damageValue;
        public int DamageRadius => _damageRadius;
        public int Number => _number;
        public int Lifetime => _lifetime;
        public int Level => _level;
        public bool IsOwnedByHero => _isOwnedByHero;
        public override Enum Type => bombType;
        public string Colors => _colors;

        #endregion Properties

        #region Class Methods

        public BombModel(string name, string description, string element, string rarity, int damageValue, int damageRadius, int number, int lifeTime, int level, string colors, bool isOwnedByHero) : base(name, description)
        {
            _element = element;
            _rarity = rarity;
            _damageValue = damageValue;
            _damageRadius = damageRadius;
            _number = number;
            _lifetime = lifeTime;
            _level = level;
            _isOwnedByHero = isOwnedByHero;
            _colors = colors;
        }

        #endregion Class Methods
    }
}