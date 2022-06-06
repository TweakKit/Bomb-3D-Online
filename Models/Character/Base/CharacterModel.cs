using System;
using System.Collections.Generic;
using UnityEngine;
using ZB.Model;

namespace ZB.Gameplay
{
    public enum CharacterType
    {
        Aries = 0,
        Taurus = 1,
        Gemini = 2,
        Cancer = 3,
        Leo = 4,
        Virgo = 5,
        Libra = 6,
        Scorpio = 7,
        Sagittarius = 8,
        Capricorn = 9,
        Aquarius = 10,
        Pisces = 11,
        Count = 12,
    }

    public enum RarityType
    {
        Common = 0,
        Rare = 1,
        Epic = 2,
        Legendary = 3,
    }

    public abstract class CharacterModel : EntityModel
    {
        #region Members

        public static readonly float CheckMoveableRadius = 0.3f;

        protected string _houseID;
        protected int _level;
        protected string _colors;
        protected string _element;
        protected string _rarity;
        protected int _hp;
        protected float _defense;
        protected float _moveSpeed;
        protected int _energy;
        protected float _energyRegain;
        protected CharacterType _characterType;
        protected Vector3 _movePosition;

        [NonSerialized]
        public List<AIState> AIstates;
        [NonSerialized]
        public BombModel bombData;
        [NonSerialized]
        public float currentHP;
        [NonSerialized]
        public bool isActivated;
        [NonSerialized]
        public Vector3 rotateDirection;
        [NonSerialized]
        public CharacterModel target;
        [NonSerialized]
        public int currentBombsPlaced;
        [NonSerialized]
        public bool hasPet;
        [NonSerialized]
        public HitBy hitBy;
        [NonSerialized]
        public bool isMoving;
        [NonSerialized]
        public float bonusSpeed;

        #endregion Members

        #region Properties

        public Action AttackEvent { get; set; } = () => { };
        public Action GetDamagedEvent { get; set; } = () => { };
        public Action DieEvent { get; set; } = () => { };
        public Action WinEvent { get; set; } = () => { };
        public Action LoseEvent { get; set; } = () => { };
        public Action ChangeMovementEvent { get; set; } = () => { };

        public Vector3 MovePosition
        {
            get
            {
                return _movePosition;
            }
            set
            {
                if (_movePosition != value)
                {
                    _movePosition = value;
                    if (!isMoving)
                    {
                        isMoving = true;
                        ChangeMovementEvent.Invoke();
                    }
                }
                else
                {
                    if (isMoving)
                    {
                        isMoving = false;
                        ChangeMovementEvent.Invoke();
                    }
                }
            }
        }

        public float BoundRadius { get; protected set; }
        public string HouseID => _houseID;
        public int Level => _level;
        public string Colors => _colors;
        public string Element => _element;
        public string Rarity => _rarity;
        public int HP => _hp;
        public float Defense => _defense;
        public float MoveSpeed => _moveSpeed + bonusSpeed;
        public int Energy => _energy;
        public float EnergyRegain => _energyRegain;
        public CharacterType CharacterType => _characterType;
        public bool IsDead => currentHP <= 0;
        public bool CanAttack => currentBombsPlaced < bombData.Number;
        public override Enum Type => _characterType;
        public abstract bool IsHero { get; }
        public abstract bool IsEnemy { get; }
        public abstract bool IsControlled { get; }

        #endregion Properties

        #region Class Methods

        public CharacterModel(InventoryItem characterData) : base(characterData.properties.type, ZB.CloudFunctionsCall.Utils.SerializeObject(characterData))
        {
            _houseID = characterData.properties.houseId;
            _level = characterData.properties.level;
            _colors = characterData.properties.colors;
            _element = characterData.properties.element;
            _rarity = characterData.properties.rarity;
            _hp = characterData.properties.hp;
            _defense = characterData.properties.def;
            _moveSpeed = characterData.properties.ms;
            _energy = characterData.properties.eng;
            _energyRegain = characterData.properties.er;
            _characterType = characterData.characterType;

            InventoryItem bombItem = characterData.GetBoom();
            if (bombItem != null)
            {
                bombData = new BombModel(bombItem.itemType, bombItem.itemType, bombItem.properties.element, bombItem.properties.rarity, bombItem.properties.dmg, bombItem.properties.ran, bombItem.properties.num, bombItem.properties.ext, bombItem.properties.level, bombItem.properties.colors, IsHero);
                bombData.bombType = (BombType)_characterType;
            }
            else
            {
                bombItem = DataManager.Instance.GetBombDefaultData(_rarity);
                bombData = new BombModel("Default", "Default", "NONE", "NONE", bombItem.properties.dmg, bombItem.properties.ran, bombItem.properties.num, bombItem.properties.ext, bombItem.properties.level, bombItem.properties.colors, IsHero);
                bombData.bombType = BombType.BombDefault;
            }

            InventoryItem armorItem = characterData.GetArmor();
            if (armorItem != null)
            {
                _hp += armorItem.properties.hp;
                _moveSpeed -= _moveSpeed * armorItem.properties.hv;
                _defense += _defense * armorItem.properties.def;
            }

            InventoryItem petItem = characterData.GetPet();
            if (petItem != null)
            {
                _hp += petItem.properties.hp;
                _moveSpeed = petItem.properties.ms;
                hasPet = true;
                BoundRadius = Constants.CharacterWithPetBoundRadius;
            }
            else
            {
                hasPet = false;
                BoundRadius = Constants.CharacterBoundRadius;
            }

            Initialize();
        }

        public CharacterModel(EnemyStat enemyStat, CharacterType enemyType) : base(enemyType.ToString(), enemyStat.Info())
        {
            _houseID = "NULL";
            _level = enemyStat.level;
            _colors = "NULL";
            _element = enemyType.GetElement();
            _rarity = "Common";
            _hp = enemyStat.hp;
            _defense = 0;
            _moveSpeed = enemyStat.speed;
            _energy = 0;
            _energyRegain = 0;
            _characterType = enemyType;

            bombData = new BombModel(enemyType.ToString(), enemyType.ToString(), _element, _rarity, enemyStat.boomDamage, enemyStat.boomRange, enemyStat.boomPut, enemyStat.boomEXT, enemyStat.level, null, IsHero);
            bombData.bombType = (BombType)enemyType;

            Initialize();
        }

        public virtual void SetActivation(bool isActivated)
        {
            this.isActivated = isActivated;
            if (!isActivated)
                MovePosition = MovePosition;
        }

        public void Attack()
        {
            currentBombsPlaced++;
        }

        public void OnBombExplode()
        {
            currentBombsPlaced--;
        }

        public virtual void WinGame()
        {
            SetActivation(false);
            WinEvent.Invoke();
        }

        public virtual void LoseGame()
        {
            SetActivation(false);
            LoseEvent.Invoke();
        }

        private void Initialize()
        {
            bombData.ExplodeEvent += OnBombExplode;
            currentHP = _hp;
            currentBombsPlaced = 0;
            isMoving = false;
            bonusSpeed = 0;
        }

        #endregion Class Methods
    }
}