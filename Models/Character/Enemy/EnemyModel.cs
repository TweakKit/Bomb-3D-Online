using ZB.Model;

namespace ZB.Gameplay
{
    public class EnemyModel : CharacterModel
    {
        #region Members

        public static readonly float FaceToFaceDotValue = -0.9f;

        private int _collideDamage;
        private int _chaseTargetAOE;
        private int _loseTargetAOE;
        private float _token;

        #endregion Members

        #region Properties

        public float CollideDamage => _collideDamage;
        public int ChaseTargetAOE => _chaseTargetAOE;
        public int LoseTargetAOE => _loseTargetAOE;
        public float Token => _token;
        public override bool IsHero { get { return false; } }
        public override bool IsEnemy { get { return true; } }
        public override bool IsControlled { get { return false; } }

        #endregion Properties

        #region Class Methods

        public EnemyModel(EnemyStat enemyStat, CharacterType enemyType, float baseToken) : base(enemyStat, enemyType)
        {
            _collideDamage = enemyStat.hitDamage;
            _chaseTargetAOE = enemyStat.chaseAOE + 1;
            _loseTargetAOE = enemyStat.lostAOE + 1;
            _token = enemyStat.tokenRate * baseToken;
            AIstates = DataManager.Instance.GetEnemyAIStates(_level);
        }

        #endregion Class Methods
    }
}