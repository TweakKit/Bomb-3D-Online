using System;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace ZB.Gameplay
{
    /// <summary>
    /// This action makes the character patrol around (horizontally/vertically).
    /// </summary>
    [Serializable]
    public class AIActionPatrol : AIAction
    {
        #region Members

        protected enum PatrolState
        {
            FitToTileCenter,
            Normal,
            ChangeDirection,
            Stuck
        }

        protected PatrolState _patrolState;
        protected MapDirection _patrolDirection;
        protected Vector3 _changePatrolDirectionPoint;
        protected float _direction;
        protected float _moveSpeed;

        #endregion Members

        #region Class Methods

        public AIActionPatrol(AIActionPatrol other) : base(other) { }

        public override AIAction Clone() => new AIActionPatrol(this);

        public override void Init(AIState ownerState, CharacterModel ownerModel)
        {
            base.Init(ownerState, ownerModel);
            _moveSpeed = OwnerModel.MoveSpeed;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            if (OwnerModel.Position != MapManager.GetMapPosition(OwnerModel.Position))
                _patrolState = PatrolState.FitToTileCenter;
            else
                _patrolState = PatrolState.Normal;

            _direction = UnityRandom.Range(0, 2) % 2 == 0 ? 1 : -1;
            _patrolDirection = UnityRandom.Range(0, 2) % 2 == 0 ? MapDirection.Horizontal : MapDirection.Vertical;

            if (MapManager.IsBlocked(OwnerModel.Position, _patrolDirection))
                _patrolDirection = _patrolDirection == MapDirection.Horizontal ? MapDirection.Vertical : MapDirection.Horizontal;
        }

        public override void PerformAction()
        {
            Patrol();
        }

        protected virtual void Patrol()
        {
            if (_patrolState == PatrolState.FitToTileCenter)
            {
                Vector3 tileCenterPosition = MapManager.GetMapPosition(OwnerModel.Position);
                Vector3 moveToPosition = OwnerModel.Position + (tileCenterPosition - OwnerModel.Position).normalized * Time.deltaTime * _moveSpeed;
                if (Vector3.SqrMagnitude(moveToPosition - tileCenterPosition) < (Time.deltaTime * _moveSpeed * Time.deltaTime * _moveSpeed))
                {
                    OwnerModel.MovePosition = tileCenterPosition;
                    _patrolState = PatrolState.Normal;
                }
                else OwnerModel.MovePosition = moveToPosition;
            }
            else if (_patrolState == PatrolState.Normal)
            {
                if (MapManager.IsNotBlocked(OwnerModel.Position, _patrolDirection))
                {
                    Vector3 moveDirection = _direction * (_patrolDirection == MapDirection.Horizontal ? Vector3.right : Vector3.forward);
                    Vector3 boundMoveToPosition = OwnerModel.Position + (moveDirection * Time.deltaTime * _moveSpeed) + moveDirection * CharacterModel.CheckMoveableRadius;

                    if (MapManager.IsMovable(OwnerModel.Position, boundMoveToPosition))
                    {
                        OwnerModel.MovePosition = OwnerModel.Position + moveDirection * Time.deltaTime * _moveSpeed;
                        OwnerModel.rotateDirection = moveDirection;
                    }
                    else
                    {
                        _direction *= -1;
                        CheckAttack();
                    }
                }
                else
                {
                    OwnerModel.MovePosition = OwnerModel.MovePosition;
                    bool canChangePatrolDirection = MapManager.GetNonBlockedPosition(OwnerModel.Position, _patrolDirection, out _changePatrolDirectionPoint);
                    if (canChangePatrolDirection)
                    {
                        _patrolState = PatrolState.ChangeDirection;
                        _patrolDirection = _patrolDirection == MapDirection.Horizontal ? MapDirection.Vertical : MapDirection.Horizontal;
                    }
                }
            }
            else if (_patrolState == PatrolState.ChangeDirection)
            {
                var nextPosition = Vector3.MoveTowards(OwnerModel.Position, _changePatrolDirectionPoint, Time.deltaTime * _moveSpeed);
                if (Vector3.SqrMagnitude(_changePatrolDirectionPoint - nextPosition) < (Time.deltaTime * _moveSpeed * Time.deltaTime * _moveSpeed))
                {
                    _patrolState = PatrolState.Normal;
                    _patrolDirection = _patrolDirection == MapDirection.Horizontal ? MapDirection.Vertical : MapDirection.Horizontal;
                    OwnerModel.MovePosition = _changePatrolDirectionPoint;
                }
                else
                {
                    Vector3 moveDirection = (nextPosition - OwnerModel.Position).normalized;
                    OwnerModel.MovePosition = OwnerModel.Position + moveDirection * Time.deltaTime * _moveSpeed;
                    OwnerModel.rotateDirection = moveDirection;
                }
            }
        }

        protected virtual void CheckAttack()
        {
            if (OwnerModel.CanAttack && MapManager.IsEmptyPosition(OwnerModel.Position) && MapManager.IsSurroundedByBreakable(OwnerModel.Position))
                OwnerModel.AttackEvent.Invoke();
        }

        #endregion Class Methods
    }
}