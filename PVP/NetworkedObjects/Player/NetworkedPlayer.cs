using System;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity, INetworkedPlayer
    {
        #region Members

        [HideInInspector]
        [SyncVar]
        public int teamIndex;

        [SyncVar(hook = nameof(OnCurrentHPChange))]
        public float currentHP;

        [HideInInspector]
        [SyncVar]
        public bool hasPet;

        [HideInInspector]
        [SyncVar]
        public float token;

        [HideInInspector]
        [SyncVar]
        public int bonusBombNumber;

        [HideInInspector]
        [SyncVar]
        public int bonusBombRange;

        [HideInInspector]
        [SyncVar]
        public float bonusMoveSpeedValue;

        [HideInInspector]
        [SyncVar]
        public float bonusMoveSpeedPercent;

        [HideInInspector]
        [SyncVar]
        public bool isSelfDefend;

        [HideInInspector]
        public float maxHP;
        [HideInInspector]
        public GameObject killedBy;
        [HideInInspector]
        public bool isMoving;
        [HideInInspector]
        public float defense;

        [SerializeField]
        protected float _moveSpeed = 4.0f;
        [SerializeField]
        protected int _bombNumber = 1;
        [SerializeField]
        protected GameObject _bombPrefab;
        [SerializeField]
        protected NetworkedBombData _bombData;

        #endregion Members

        #region Properties

        public static NetworkedEntity Local { get; private set; }
        public Action ClickAttackEvent { get; set; } = () => { };
        public Action GetDamagedEvent { get; set; } = () => { };
        public Action DieEvent { get; set; } = () => { };
        public Action WinEvent { get; set; } = () => { };
        public Action LoseEvent { get; set; } = () => { };
        public Action ChangeMovementEvent { get; set; } = () => { };
        public Action<Booster> CollectBoosterEvent { get; set; } = _ => { };

        public float MoveSpeed
        {
            get
            {
                return (_moveSpeed + bonusMoveSpeedValue) * ((bonusMoveSpeedPercent + 100) / 100);
            }
        }

        public int BombNumber
        {
            get
            {
                return _bombNumber + bonusBombNumber;
            }
        }

        #endregion Properties

        #region API Methods

        protected virtual void Awake()
        {
            gameObject.layer = Constants.PlayerLayerIndex;
        }

        protected virtual void Update()
        {
            UpdateInput();
            UpdateAffectAction();
        }

        protected virtual void FixedUpdate()
        {
            UpdateMovement();
        }

        #endregion API Methods

        #region Class Methods

        public override void OnStartAuthority()
        {
            enabled = true;
            bonusBombNumber = 0;
            bonusBombRange = 0;
            bonusMoveSpeedValue = 0.0f;
            bonusMoveSpeedPercent = 0.0f;

            RegisterInput();
            RegisterMovement();
            RegisterAffectAction();
            RegisterAnimation();
            RegisterAttack();
            RegisterGetHit();
            RegisterHUD();
            RegisterDie();
            RegisterGiveToken();
            RegisterCollectBooster();
            RegisterUseItem();

            Local = this;
            EventManager.Invoke<NetworkedPlayer>(GameEventType.InitPlayer, this);
        }

        public override void OnStartClient()
        {
            InitHUDOnAllClients();
        }

        [ServerCallback]
        public void Reset()
        {
            isDead = false;
            bonusBombNumber = 0;
            bonusBombRange = 0;
            bonusMoveSpeedValue = 0.0f;
            bonusMoveSpeedPercent = 0.0f;
        }

        #endregion Class Methods
    }
}