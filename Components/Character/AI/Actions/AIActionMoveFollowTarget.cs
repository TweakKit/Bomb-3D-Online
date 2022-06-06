using System;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// This action makes the character follow the chased target.
    /// When the character is near the chased target by a specific distance, stop chasing the target, hit and move away from the target.
    /// When the character is far away from the chased target by a specific distance, make the character chase the target again.
    /// </summary>
    [Serializable]
    public class AIActionMoveFollowTarget : AIActionMove
    {
        #region Members

        protected static readonly float stopChasingTargetDeduction = 0.1f;
        protected static readonly float rechaseTargetThresholdSqr = 3 * MapSetting.MapSquareSize * 3 * MapSetting.MapSquareSize;
        protected static readonly int minMoveAwayTargetTiles = 2;
        protected static readonly int maxMoveAwayTargetTiles = 5;

        protected bool _isMoveAwayFromTarget;
        protected float _stopChasingTargetDistanceThresholdSqr;

        #endregion Members

        #region Class Methods

        public AIActionMoveFollowTarget(AIActionMoveFollowTarget other) : base(other) { }

        public override AIAction Clone() => new AIActionMoveFollowTarget(this);

        public override void OnEnterState()
        {
            base.OnEnterState();
            _isMoveAwayFromTarget = false;
            _stopChasingTargetDistanceThresholdSqr = (OwnerModel.target.BoundRadius + OwnerModel.BoundRadius - stopChasingTargetDeduction) * (OwnerModel.target.BoundRadius + OwnerModel.BoundRadius - stopChasingTargetDeduction);
        }

        protected override void FindNewPath()
        {
            RunFindPath();
        }

        protected override void MoveOnPath()
        {
            if (_isMoveAwayFromTarget)
            {
                // If the chased target is now far from the character by a specific distance, then find a new path to move to attack them again.
                if (Vector3.SqrMagnitude(OwnerModel.target.Position - OwnerModel.Position) >= rechaseTargetThresholdSqr)
                {
                    _isMoveAwayFromTarget = false;
                    RefindNewPath();
                    return;
                }
            }
            else
            {
                // If the chased target is now near the character by a specific distance, then stop chasing, hit and move away from the target.
                if (Vector3.SqrMagnitude(OwnerModel.target.Position - OwnerModel.Position) <= _stopChasingTargetDistanceThresholdSqr)
                {
                    _isMoveAwayFromTarget = true;
                    CheckCanAttackTarget();
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
            base.FinishedMoving();
            if (_isMoveAwayFromTarget)
                _isMoveAwayFromTarget = false;
        }

        protected virtual void CheckCanAttackTarget()
        {
            if (OwnerModel.CanAttack && MapManager.IsEmptyPosition(OwnerModel.Position))
                OwnerModel.AttackEvent.Invoke();
        }

        protected virtual void RunFindPath()
        {
            if (_isMoveAwayFromTarget)
                RunFindPathAwayFromTarget();
            else
                RunFindPathToTarget();
        }

        protected virtual void RunFindPathAwayFromTarget()
        {
            Vector3 moveAwayTargetPosition = MapManager.GetMoveAwayTargetPosition(OwnerModel.Position, OwnerModel.target.Position, minMoveAwayTargetTiles, maxMoveAwayTargetTiles);
            if (moveAwayTargetPosition != Vector3.zero)
            {
                InitializePath(OwnerModel.Position, moveAwayTargetPosition);
                return;
            }

            _hasFoundAPath = false;
        }

        protected virtual void RunFindPathToTarget()
        {
            if (MapManager.GetPathPositions(OwnerModel.Position, OwnerModel.target.Position).HasPath())
                InitializePath(OwnerModel.Position, OwnerModel.target.Position);
            else
                RunFindPathToAroundTarget();
        }

        protected virtual void RunFindPathToAroundTarget()
        {
            Vector3 neighbourTargetPosition = MapManager.GetNeighbourTargetPosition(OwnerModel.Position, OwnerModel.target.Position);
            if (neighbourTargetPosition != Vector3.zero)
                InitializePath(OwnerModel.Position, neighbourTargetPosition);
            else
                RunFindPathAwayFromTarget();
        }

        protected virtual void InitializePath(Vector3 startPosition, Vector3 endPosition)
        {
            _pathPositions = MapManager.GetPathPositions(startPosition, endPosition);
            _pathMoveIndex = 0;
            _pathTargetPosition = startPosition;
            _hasFoundAPath = true;
        }

        #endregion Class Methods
    }
}