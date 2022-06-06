using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// This action makes the character find the position of the nearest target to move to and attack.
    /// After attacking the target, the character will move away from the target.
    /// If there is no path to the targets, the character will find bricks and break them.
    /// If there is still no path to the bricks, the character will just move randomly around.
    /// </summary>
    [Serializable]
    public class AIActionMoveKillTarget : AIActionMove
    {
        #region Members

        protected static readonly float stopChasingTargetThresholdSqr = 2 * MapSetting.MapSquareSize * 2 * MapSetting.MapSquareSize;
        protected static readonly float rechaseTargetThresholdSqr = 5 * MapSetting.MapSquareSize * 6 * MapSetting.MapSquareSize;
        protected static readonly float refindTargetThresholdSqr = 8 * MapSetting.MapSquareSize * 8 * MapSetting.MapSquareSize;
        protected static readonly int minMoveAwayTargetTiles = 3;
        protected static readonly int maxMoveAwayTargetTiles = 7;

        protected enum MoveState
        {
            MoveAttackTarget,
            MoveAwayFromTarget,
            MoveBreakStuff,
            MoveRandomly,
        }

        [SerializeField]
        protected bool _isTargetEnemy;
        protected List<CharacterModel> _targets;
        protected Vector3 _moveToPosition;
        protected Vector3 _breakablePosition;
        protected bool _isMoveAwayFromTarget;
        protected MoveState _moveState = MoveState.MoveAttackTarget;

        #endregion Members

        #region Class Methods

        public AIActionMoveKillTarget(AIActionMoveKillTarget other) : base(other)
        {
            _isTargetEnemy = other._isTargetEnemy;
        }

        public override AIAction Clone() => new AIActionMoveKillTarget(this);

        public override void Init(AIState ownerState, CharacterModel ownerModel)
        {
            base.Init(ownerState, ownerModel);
            _targets = _isTargetEnemy ? EntitiesManager.Instance.Enemies : EntitiesManager.Instance.Heroes;
        }

        public override void Dispose()
        {
            base.Dispose();
            _targets = null;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _isMoveAwayFromTarget = false;
        }

        protected override void FindNewPath()
        {
            RunFindPath();
        }

        protected override void MoveOnPath()
        {
            if (_moveState == MoveState.MoveAttackTarget)
            {
                if (OwnerModel.target != null)
                {
                    // If the target is dead, then find another path.
                    if (OwnerModel.target.IsDead)
                    {
                        RefindNewPath();
                        return;
                    }
                    else
                    {
                        // If the target has moved far from the destination where the character is supposed to move to, then find another new path.
                        if (Vector3.SqrMagnitude(OwnerModel.target.Position - _moveToPosition) >= refindTargetThresholdSqr)
                        {
                            RefindNewPath();
                            return;
                        }
                        // If the chased target is now near the character by a specific distance, then stop chasing, attack and move away from the target.
                        else if (Vector3.SqrMagnitude(OwnerModel.target.Position - OwnerModel.Position) <= stopChasingTargetThresholdSqr)
                        {
                            _isMoveAwayFromTarget = true;
                            CheckCanAttackTarget();
                            RefindNewPath();
                            return;
                        }
                    }
                }
            }
            else if (_moveState == MoveState.MoveAwayFromTarget)
            {
                if (OwnerModel.target != null)
                {
                    // If the target is dead, then find another path.
                    if (OwnerModel.target.IsDead)
                    {
                        _isMoveAwayFromTarget = false;
                        RefindNewPath();
                        return;
                    }
                    else
                    {
                        // If the chased target is now far from the character by a specific distance, then find a new path to move to attack them again.
                        if (Vector3.SqrMagnitude(OwnerModel.target.Position - OwnerModel.Position) >= rechaseTargetThresholdSqr)
                        {
                            _isMoveAwayFromTarget = false;
                            RefindNewPath();
                            return;
                        }
                    }
                }
            }
            else if (_moveState == MoveState.MoveBreakStuff)
            {
                // Find another path if the target breakable has been destroyed or there is a bomb at the end of the path.
                if (MapManager.IsEmptyPosition(_breakablePosition) || MapManager.IsBombPosition(_moveToPosition))
                {
                    RefindNewPath();
                    return;
                }
            }

            var nextPosition = Vector3.MoveTowards(OwnerModel.Position, _pathTargetPosition, Time.deltaTime * _moveSpeed);
            if (Vector3.SqrMagnitude(_pathTargetPosition - nextPosition) < (Time.deltaTime * _moveSpeed * Time.deltaTime * _moveSpeed))
            {
                _pathMoveIndex++;

                // Reached the end of the path.
                if (_pathMoveIndex == _pathPositions.Count)
                {
                    FinishedMoving();
                    return;
                }
                else _pathTargetPosition = new Vector3(_pathPositions[_pathMoveIndex].x, OwnerModel.Position.y, _pathPositions[_pathMoveIndex].y);

                // Dodge bombs in 1 tile ahead in the path.
                if ((_pathMoveIndex + 1) < _pathPositions.Count)
                {
                    if (MapManager.IsBombPosition(new Vector3(_pathPositions[_pathMoveIndex + 1].x, OwnerModel.Position.y, _pathPositions[_pathMoveIndex + 1].y)))
                    {
                        RefindNewPath();
                        return;
                    }
                }
            }
            else
            {
                Vector3 moveDirection = (nextPosition - OwnerModel.Position).normalized;
                Vector3 boundMoveToPosition = OwnerModel.Position + (moveDirection * Time.deltaTime * _moveSpeed) + moveDirection * CharacterModel.CheckMoveableRadius;

                if (MapManager.IsMovable(OwnerModel.Position, boundMoveToPosition))
                {
                    OwnerModel.MovePosition = OwnerModel.Position + moveDirection * Time.deltaTime * _moveSpeed;
                    OwnerModel.rotateDirection = moveDirection;
                }
                // Dodge obstacles while moving through the path.
                else
                {
                    RefindNewPath();
                    return;
                }
            }
        }

        protected override void FinishedMoving()
        {
            if (_moveState == MoveState.MoveAwayFromTarget)
                _isMoveAwayFromTarget = false;
            else if (_moveState == MoveState.MoveBreakStuff || _moveState == MoveState.MoveRandomly)
                CheckCanBreakStuff();

            base.FinishedMoving();
        }

        protected virtual void CheckCanAttackTarget()
        {
            if (OwnerModel.CanAttack && MapManager.IsEmptyPosition(OwnerModel.Position))
                OwnerModel.AttackEvent.Invoke();
        }

        protected virtual void CheckCanBreakStuff()
        {
            if (OwnerModel.CanAttack && MapManager.IsEmptyPosition(OwnerModel.Position) && MapManager.IsSurroundedByBreakable(OwnerModel.Position))
                OwnerModel.AttackEvent.Invoke();
        }

        protected virtual void RunFindPath()
        {
            if (_isMoveAwayFromTarget)
            {
                Vector3 moveAwayTargetPosition = MapManager.GetMoveAwayTargetPosition(OwnerModel.Position, OwnerModel.target.Position, minMoveAwayTargetTiles, maxMoveAwayTargetTiles);
                if (moveAwayTargetPosition != Vector3.zero)
                {
                    InitializePath(MoveState.MoveAwayFromTarget, OwnerModel.Position, moveAwayTargetPosition);
                    return;
                }
            }
            else
            {
                Vector3 nearTargetPosition = GetNearTargetPosition();
                if (nearTargetPosition != Vector3.zero)
                {
                    InitializePath(MoveState.MoveAttackTarget, OwnerModel.Position, nearTargetPosition);
                    return;
                }
            }

            // There're still targets but no way there, find bricks to break.
            var AIMoveData = MapManager.GetMoveToBrickPosition(OwnerModel.Position);
            if (AIMoveData.IsLegalData)
            {
                InitializePath(MoveState.MoveBreakStuff, OwnerModel.Position, AIMoveData.moveToPosition, AIMoveData.breakablePosition);
                return;
            }

            // Find a neighbour empty position to move to (move around randomly).
            Vector3 neighbourEmptyPosition = MapManager.GetNeighbourEmptyPosition(OwnerModel.Position);
            if (neighbourEmptyPosition != Vector3.zero)
            {
                InitializePath(MoveState.MoveRandomly, OwnerModel.Position, neighbourEmptyPosition);
                return;
            }

            _hasFoundAPath = false;
        }

        protected virtual Vector3 GetNearTargetPosition()
        {
            float minDistanceSqr = float.MaxValue;
            CharacterModel nearTarget = null;

            foreach (var target in _targets)
            {
                float distanceSqr = Vector3.SqrMagnitude(target.Position - OwnerModel.Position);
                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    nearTarget = target;
                }
            }

            if (nearTarget != null && MapManager.GetPathPositions(OwnerModel.Position, nearTarget.Position).HasPath())
            {
                OwnerModel.target = nearTarget;
                return nearTarget.Position;
            }
            else
            {
                OwnerModel.target = null;
                return Vector3.zero;
            }
        }

        protected virtual void InitializePath(MoveState moveState, Vector3 startPosition, Vector3 endPosition)
        {
            _moveState = moveState;
            _moveToPosition = endPosition;
            _pathPositions = MapManager.GetPathPositions(startPosition, endPosition);
            _pathMoveIndex = 0;
            _pathTargetPosition = startPosition;
            _hasFoundAPath = true;
        }

        protected virtual void InitializePath(MoveState moveState, Vector3 startPosition, Vector3 endPosition, Vector3 breakablePosition)
        {
            _breakablePosition = breakablePosition;
            InitializePath(moveState, startPosition, endPosition);
        }

        #endregion Class Methods
    }
}