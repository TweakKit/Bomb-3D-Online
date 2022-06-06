using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class MagnetBooster : TimeLimitBooster
    {
        #region Membes

        [SerializeField]
        private int _magnetizeRange = 3;

        #endregion Members

        #region Class Methods

        public override void GetCollected(NetworkedPlayer networkedPlayer)
        {
            IPlayerAffectAction playerAffectAction = new MagnetAffectAction(_magnetizeRange, _usageLimitTime);
            networkedPlayer.ApplyAffectAction(playerAffectAction);
        }

        #endregion Class Methods
    }

    public class MagnetAffectAction : PlayerRuntimeAffectAction
    {
        #region Members

        private static readonly float magnetizeDelay = 0.5f;
        private int _magnetizeRange;
        private float _currentMagnetizeDelay;

        #endregion Members

        #region Class Methods

        public MagnetAffectAction(int magnetizeRange, float affectDuration) : base(affectDuration)
        {
            _magnetizeRange = magnetizeRange;
            _currentMagnetizeDelay = 0.0f;
        }

        protected override void UpdatePerFrameTask()
        {
            base.UpdatePerFrameTask();
            _currentMagnetizeDelay += Time.deltaTime;
            if (_currentMagnetizeDelay >= magnetizeDelay)
            {
                _currentMagnetizeDelay = 0.0f;
                Magnetize();
            }
        }

        private void Magnetize()
        {
            var boostereColliders = Physics.OverlapSphere(_owner.transform.position, (_magnetizeRange + 1) * MapSetting.MapSquareSize, Constants.BoosterLayerMask);
            foreach (var boosterCollider in boostereColliders)
                _owner.CollectBoosterEvent.Invoke(boosterCollider.GetComponent<Booster>());
        }

        #endregion Methods
    }
}