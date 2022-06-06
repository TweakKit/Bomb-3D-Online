using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// This action makes the character move.
    /// </summary>
    [Serializable]
    public abstract class AIActionMove : AIAction
    {
        #region Members

        protected bool _canFindNewPath;
        protected bool _hasFoundAPath;
        protected int _pathMoveIndex;
        protected float _moveSpeed;
        protected Vector3 _pathTargetPosition;
        protected List<Vector2> _pathPositions;

        #endregion Members

        #region Class Methods

        public AIActionMove(AIActionMove other) : base(other) { }

        public override void Init(AIState ownerState, CharacterModel ownerModel)
        {
            base.Init(ownerState, ownerModel);
            _moveSpeed = OwnerModel.MoveSpeed;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _canFindNewPath = true;
            _hasFoundAPath = false;
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _pathPositions = null;
        }

        /// <summary>
        /// On PerformAction we move the character.
        /// </summary>
        public override void PerformAction()
        {
            Move();
        }

        /// <summary>
        /// Move the character.
        /// </summary>
        protected virtual void Move()
        {
            CheckFindPath();
            if (CheckCanMoveOnPath())
                MoveOnPath();
        }

        protected virtual void CheckFindPath()
        {
            if (_canFindNewPath)
            {
                _canFindNewPath = false;

                if (MapManager.IsFullyBlocked(OwnerModel.Position))
                {
                    _canFindNewPath = true;
                    OwnerModel.MovePosition = OwnerModel.MovePosition;
                    return;
                }

                FindNewPath();

                if (!_hasFoundAPath)
                {
                    _canFindNewPath = true;
                    OwnerModel.MovePosition = OwnerModel.MovePosition;
                }
            }
        }

        protected virtual bool CheckCanMoveOnPath()
        {
            return _hasFoundAPath;
        }

        protected abstract void FindNewPath();
        protected abstract void MoveOnPath();

        protected virtual void FinishedMoving()
        {
            RefindNewPath();
        }

        protected virtual void RefindNewPath()
        {
            _hasFoundAPath = false;
            _canFindNewPath = true;
        }

        #endregion Class Methods
    }

    public struct AIMoveData : IEquatable<AIMoveData>
    {
        #region Members

        public static AIMoveData illegalMoveData = new AIMoveData(-Vector3.one, -Vector3.one);

        public Vector3 moveToPosition;
        public Vector3 breakablePosition;

        #endregion Members

        #region Properties

        public bool IsIllegalData => this == illegalMoveData;
        public bool IsLegalData => this != illegalMoveData;

        #endregion Properties

        #region Struct Methods

        public AIMoveData(Vector3 moveToPosition, Vector3 breakablePosition)
        {
            this.moveToPosition = moveToPosition;
            this.breakablePosition = breakablePosition;
        }

        public bool Equals(AIMoveData other)
        {
            return moveToPosition == other.moveToPosition && breakablePosition == other.breakablePosition;
        }

        public static bool operator ==(AIMoveData moveData1, AIMoveData moveData2)
        {
            return moveData1.Equals(moveData2);
        }

        public static bool operator !=(AIMoveData moveData1, AIMoveData moveData2)
        {
            return !(moveData1 == moveData2);
        }

        public override bool Equals(object obj)
        {
            return obj is AIMoveData other && other.Equals(this);
        }

        public override int GetHashCode()
        {
            return moveToPosition.GetHashCode() + breakablePosition.GetHashCode();
        }

        public override string ToString()
        {
            string info = "Move to position = " + moveToPosition + " - Breakable position = " + breakablePosition;
            return info;
        }

        #endregion Struct Methods
    }
}