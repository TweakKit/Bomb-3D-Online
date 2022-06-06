using System.Collections;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class SlowTrapItemObject : NetworkedEntity
    {
        #region Membes

        [SerializeField]
        private float _existTime = 10.0f;
        [SerializeField]
        private float _slowAffectTime = 5.0f;
        [SerializeField]
        [Range(0, 100)]
        private float _speedPercentReduce = -70.0f;
        [SerializeField]
        private float _hitDamageValue = 20.0f;

        #endregion Members

        #region API Methods

        [Server]
        protected virtual void OnCollisionEnter(Collision collision)
        {
            NetworkedPlayer networkedPlayer = collision.gameObject.GetComponent<NetworkedPlayer>();
            if (networkedPlayer)
            {
                IPlayerAffectAction playerAffectAction = new SlowTrapAffectAction(_hitDamageValue, _speedPercentReduce, _slowAffectTime);
                networkedPlayer.ApplyAffectAction(playerAffectAction);
            }
        }

        #endregion API Methods

        #region Class Methods

        public override void OnStartServer()
        {
            StartCoroutine(DestroyAfterExist());
        }

        private IEnumerator DestroyAfterExist()
        {
            yield return new WaitForSeconds(_existTime);
            NetworkServer.Destroy(gameObject);
        }

        #endregion Class Methods
    }

    public class SlowTrapAffectAction : PlayerSimpleAffectAction
    {
        #region Members

        private float _hitDamageValue;
        private float _speedPercentReduce;

        #endregion Members

        #region Class Methods

        public SlowTrapAffectAction(float hitDamageValue, float speedPercentReduce, float affectDuration) : base(affectDuration)
        {
            _hitDamageValue = hitDamageValue;
            _speedPercentReduce = speedPercentReduce;
        }

        protected override void StartTask()
        {
            base.StartTask();
            _owner.GetHit(_hitDamageValue);
            _owner.AddSpeedPercent(_speedPercentReduce);
        }

        protected override void FinishTask()
        {
            base.FinishTask();
            _owner.SubtractSpeedPercent(_speedPercentReduce);
        }

        #endregion Methods
    }
}