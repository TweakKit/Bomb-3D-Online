using System;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace ZB.Gameplay
{
    /// <summary>
    /// This action makes the character cross patrol.
    /// </summary>
    [Serializable]
    public class AIActionCrossPatrol : AIAction
    {
        #region Members

        protected enum CrossPatrolState
        {
            FitToTileCenter,
            Normal,
            Stuck
        }

        protected static readonly int crossInSpecificDirectionTimes = 2;

        protected CrossPatrolState _crossPatrolState;
        protected MapDirection _crossPatrolDirection;
        protected float _direction;
        protected float _moveSpeed;
        protected int _currentCrossInSpecificDirectionTimes;

        #endregion Members

        #region Class Methods

        public AIActionCrossPatrol(AIActionCrossPatrol other) : base(other) { }

        public override AIAction Clone() => new AIActionCrossPatrol(this);

        public override void Init(AIState ownerState, CharacterModel ownerModel)
        {
            base.Init(ownerState, ownerModel);
            _moveSpeed = OwnerModel.MoveSpeed;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            if (OwnerModel.Position != MapManager.GetMapPosition(OwnerModel.Position))
                _crossPatrolState = CrossPatrolState.FitToTileCenter;
            else
                _crossPatrolState = CrossPatrolState.Normal;

            _direction = UnityRandom.Range(0, 2) % 2 == 0 ? 1 : -1;
            _crossPatrolDirection = UnityRandom.Range(0, 2) % 2 == 0 ? MapDirection.Horizontal : MapDirection.Vertical;
            _currentCrossInSpecificDirectionTimes = 0;

            if (MapManager.IsBlocked(OwnerModel.Position, _crossPatrolDirection))
                _crossPatrolDirection = _crossPatrolDirection == MapDirection.Horizontal ? MapDirection.Vertical : MapDirection.Horizontal;
        }

        public override void PerformAction()
        {
            CrossPatrol();
        }

        protected virtual void CrossPatrol()
        {
            if (_crossPatrolState == CrossPatrolState.FitToTileCenter)
            {
                Vector3 tileCenterPosition = MapManager.GetMapPosition(OwnerModel.Position);
                Vector3 moveToPosition = OwnerModel.Position + (tileCenterPosition - OwnerModel.Position).normalized * Time.deltaTime * _moveSpeed;
                if (Vector3.SqrMagnitude(moveToPosition - tileCenterPosition) < (Time.deltaTime * _moveSpeed * Time.deltaTime * _moveSpeed))
                {
                    OwnerModel.MovePosition = tileCenterPosition;
                    _crossPatrolState = CrossPatrolState.Normal;
                }
                else OwnerModel.MovePosition = moveToPosition;
            }
            else if (_crossPatrolState == CrossPatrolState.Normal)
            {
                if (MapManager.IsFullyBlocked(OwnerModel.Position))
                {
                    _crossPatrolState = CrossPatrolState.Stuck;
                    OwnerModel.MovePosition = OwnerModel.MovePosition;
                    return;
                }

                Vector3 moveDirection = _direction * (_crossPatrolDirection == MapDirection.Horizontal ? Vector3.right : Vector3.forward);
                Vector3 boundMoveToPosition = OwnerModel.Position + (moveDirection * Time.deltaTime * _moveSpeed) + moveDirection * CharacterModel.CheckMoveableRadius;

                if (MapManager.IsMovable(OwnerModel.Position, boundMoveToPosition))
                {
                    OwnerModel.MovePosition = OwnerModel.Position + moveDirection * Time.deltaTime * _moveSpeed;
                    OwnerModel.rotateDirection = moveDirection;
                }
                else
                {
                    _direction *= -1;
                    _currentCrossInSpecificDirectionTimes++;
                    CheckAttack();
                }

                if (_currentCrossInSpecificDirectionTimes >= crossInSpecificDirectionTimes)
                {
                    Vector3 changeCrossPatrolDirectionPoint;
                    bool canChangeCrossPatrolDirection = MapManager.GetNonBlockedPosition(OwnerModel.Position, _crossPatrolDirection, out changeCrossPatrolDirectionPoint);
                    if (canChangeCrossPatrolDirection)
                    {
                        OwnerModel.MovePosition = MapManager.GetMapPosition(OwnerModel.Position);
                        _currentCrossInSpecificDirectionTimes = 0;
                        _crossPatrolDirection = _crossPatrolDirection == MapDirection.Horizontal ? MapDirection.Vertical : MapDirection.Horizontal;

                        if (_crossPatrolDirection == MapDirection.Horizontal)
                            _direction = Mathf.Sign(changeCrossPatrolDirectionPoint.x - OwnerModel.Position.x) > 1 ? 1 : -1;
                        else
                            _direction = Mathf.Sign(changeCrossPatrolDirectionPoint.z - OwnerModel.Position.z) > 1 ? 1 : -1;
                    }
                }
            }
            else if (_crossPatrolState == CrossPatrolState.Stuck)
            {
                if (UnityRandom.Range(0, 2) % 2 == 0)
                {
                    Vector3 changeCrossPatrolDirectionPoint;
                    bool canChangeCrossPatrolDirection = MapManager.GetNonBlockedPosition(OwnerModel.Position, MapDirection.Horizontal, out changeCrossPatrolDirectionPoint);
                    if (canChangeCrossPatrolDirection)
                    {
                        _direction = Mathf.Sign(changeCrossPatrolDirectionPoint.z - OwnerModel.Position.z) > 1 ? 1 : -1;
                        _crossPatrolDirection = MapDirection.Vertical;
                        _crossPatrolState = CrossPatrolState.Normal;
                        return;
                    }

                    canChangeCrossPatrolDirection = MapManager.GetNonBlockedPosition(OwnerModel.Position, MapDirection.Vertical, out changeCrossPatrolDirectionPoint);
                    if (canChangeCrossPatrolDirection)
                    {
                        _direction = Mathf.Sign(changeCrossPatrolDirectionPoint.x - OwnerModel.Position.x) > 1 ? 1 : -1;
                        _crossPatrolDirection = MapDirection.Horizontal;
                        _crossPatrolState = CrossPatrolState.Normal;
                        return;
                    }
                }
                else
                {
                    Vector3 changeCrossPatrolDirectionPoint;
                    bool canChangeCrossPatrolDirection = MapManager.GetNonBlockedPosition(OwnerModel.Position, MapDirection.Vertical, out changeCrossPatrolDirectionPoint);
                    if (canChangeCrossPatrolDirection)
                    {
                        _direction = Mathf.Sign(changeCrossPatrolDirectionPoint.x - OwnerModel.Position.x) > 1 ? 1 : -1;
                        _crossPatrolDirection = MapDirection.Horizontal;
                        _crossPatrolState = CrossPatrolState.Normal;
                        return;
                    }

                    canChangeCrossPatrolDirection = MapManager.GetNonBlockedPosition(OwnerModel.Position, MapDirection.Horizontal, out changeCrossPatrolDirectionPoint);
                    if (canChangeCrossPatrolDirection)
                    {
                        _direction = Mathf.Sign(changeCrossPatrolDirectionPoint.z - OwnerModel.Position.z) > 1 ? 1 : -1;
                        _crossPatrolDirection = MapDirection.Vertical;
                        _crossPatrolState = CrossPatrolState.Normal;
                        return;
                    }
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