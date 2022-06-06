using System;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// Decisions will be evaluated by transitions, every frame, and will return true or false.
    /// Examples such as time spent in a state, distance to a target, or object detection within an area,...
    /// </summary>
    [Serializable]
    public abstract class AIDecision
    {
        #region Members

        [SerializeField]
        protected string _lable;

        #endregion Members

        #region Properties

        public AIState OwnerState { get; private set; }
        public CharacterModel OwnerModel { get; private set; }
        public bool DecisionInProgress { get; set; }

        #endregion Properties

        #region Class Methods

        public AIDecision(AIDecision other)
        {
            _lable = other._lable;
        }

        public virtual void Init(AIState ownerState, CharacterModel ownerModel)
        {
            OwnerState = ownerState;
            OwnerModel = ownerModel;
        }

        public virtual void Dispose()
        {
            OwnerState = null;
            OwnerModel = null;
        }

        public virtual void Validate() { }

        public abstract AIDecision Clone();

        /// <summary>
        /// Called when the Brain enters a State this Decision is in.
        /// </summary>
        public virtual void OnEnterState()
        {
            DecisionInProgress = true;
        }

        /// <summary>
        /// Called when the Brain exits a State this Decision is in.
        /// </summary>
        public virtual void OnExitState()
        {
            DecisionInProgress = false;
        }

        /// <summary>
        /// This will be performed every frame while the Brain is in a state this Decision is in. 
        /// Should return true or false, which will then determine the transition's outcome.
        /// </summary>
        public abstract bool Decide();

        #endregion Class Methods
    }
}