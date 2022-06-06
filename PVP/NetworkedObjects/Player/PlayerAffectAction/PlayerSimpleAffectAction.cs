using UnityEngine;

namespace ZB.Gameplay.PVP
{
    /// <summary>
    /// Simple affect action doesn't do any extra task in every frame or/add per second.
    /// </summary>
    public class PlayerSimpleAffectAction : IPlayerAffectAction
    {
        #region Members

        protected float _affectDuration;
        protected bool _affectLifetime;
        protected NetworkedPlayer _owner;

        #endregion Members

        #region Properties

        public float AffectDuration { get { return _affectDuration; } }
        public float CurrentAffectDuration { protected set; get; }
        public bool HasFinished { get; set; }

        #endregion Properties

        #region Class Methods

        public PlayerSimpleAffectAction(float affectDuration)
        {
            _affectLifetime = affectDuration == -1;
            _affectDuration = affectDuration;
            CurrentAffectDuration = _affectLifetime ? float.MaxValue : _affectDuration;
            HasFinished = false;
        }

        public virtual void Apply(NetworkedPlayer owner)
        {
            _owner = owner;
            StartTask();
        }

        public virtual void Update()
        {
            if (!_affectLifetime)
            {
                CurrentAffectDuration -= Time.deltaTime;
                if (CurrentAffectDuration <= 0)
                {
                    FinishTask();
                    HasFinished = true;
                }
            }
        }

        public virtual void Stop()
        {
            StopTask();
        }

        protected virtual void StartTask() { }
        protected virtual void FinishTask() { }
        protected virtual void StopTask() { }

        #endregion Class Methods
    }
}