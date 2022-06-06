using System;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// Actions are behaviours and describe what your character is doing. 
    /// Examples such as patrolling, placing bombs, shooting, jumping,...
    /// </summary>
    [Serializable]
    public abstract class AIAction
    {
        #region Members

        [SerializeField]
        protected string _lable;

        #endregion Members

        #region Properties

        public AIState OwnerState{ get; private set; }
        public CharacterModel OwnerModel { get; private set; }
        public bool ActionInProgress { get; set; }

        #endregion Properties

        #region Class Methods

        public AIAction(AIAction other)
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

        public abstract AIAction Clone();

        /// <summary>
        /// Called when the Brain enters a State this Decision is in.
        /// </summary>
        public virtual void OnEnterState()
        {
            ActionInProgress = true;
        }

        /// <summary>
        /// Called when the Brain exits a State this Decision is in.
        /// </summary>
        public virtual void OnExitState()
        {
            ActionInProgress = false;
        }

        /// <summary>
        /// This will be performed every frame to perform the action.
        /// </summary>
        public abstract void PerformAction();

        #endregion Class Methods
    }
}