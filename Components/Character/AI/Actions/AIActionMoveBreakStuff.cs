using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// This action makes the character move to the positions of the breakable stuff to break them.
    /// Breakable stuff is either chest or brick.
    /// If there is still at least a chest in the game, the character will find a shortest path to move to it and break it.
    /// If there is no path to the chests, the character will find bricks to break instead, until there's at least a chest again, the character will get back to find and break chests.
    /// If there is no path to the chests and bricks, the character will just move randomly around.
    /// </summary>
    [Serializable]
    public class AIActionMoveBreakStuff : AIActionMove
    {
        #region Members

        protected Vector3 _moveToPosition;
        protected Vector3 _breakablePosition;

        #endregion Members

        #region Class Methods

        public AIActionMoveBreakStuff(AIActionMoveBreakStuff other) : base(other) { }

        public override AIAction Clone() => new AIActionMoveBreakStuff(this);

        protected override void FindNewPath()
        {
            RunFindPath();
        }

        protected override void MoveOnPath()
        {
            // Find another path if the target breakable has been destroyed or there is a bomb at the end of the path.
            if (MapManager.IsEmptyPosition(_breakablePosition) || MapManager.IsBombPosition(_moveToPosition))
            {
                RefindNewPath();
                return;
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
            CheckCanBreakStuff();
            base.FinishedMoving();
        }

        protected virtual void CheckCanBreakStuff()
        {
            if (OwnerModel.CanAttack && MapManager.IsEmptyPosition(OwnerModel.Position) && MapManager.IsSurroundedByBreakable(OwnerModel.Position))
                OwnerModel.AttackEvent.Invoke();
        }

        protected virtual void RunFindPath()
        {
            // Find a position where this character can move to to break chests, or bricks if no way to move to chests.
            if (EntitiesManager.Instance.HasChest)
            {
                var AImoveData = MapManager.GetMoveToChestPosition(OwnerModel.Position, new List<Vector3>(EntitiesManager.Instance.ChestPoints));
                if (AImoveData.IsIllegalData)
                {
                    if (EntitiesManager.Instance.HasChest)
                    {
                        AImoveData = MapManager.GetMoveToBrickPosition(OwnerModel.Position);
                        if (AImoveData.IsLegalData)
                        {
                            InitializePath(OwnerModel.Position, AImoveData.moveToPosition, AImoveData.breakablePosition);
                            return;
                        }
                    }
                }
                else
                {
                    InitializePath(OwnerModel.Position, AImoveData.moveToPosition, AImoveData.breakablePosition);
                    return;
                }
            }

            // Find a neighbour empty position to move to (move around randomly).
            Vector3 neighbourEmptyPosition = MapManager.GetNeighbourEmptyPosition(OwnerModel.Position);
            if (neighbourEmptyPosition != Vector3.zero)
            {
                InitializePath(OwnerModel.Position, neighbourEmptyPosition, Vector3.zero);
                return;
            }

            _hasFoundAPath = false;
        }

        protected virtual async Task RunFindPathByTPL(Vector3 ownerPosition)
        {
            // Find a position where this character can move to to break chests, or bricks if no way to move to chests.
            List<MapAnchor> chestSlots = MapManager.GetChestSlots();
            if (chestSlots.Count > 0)
            {
                var AImoveData = await MapManager.GetMoveToChestPositionByTPL(ownerPosition, chestSlots);
                if (AImoveData.IsIllegalData)
                {
                    if (chestSlots.Count > 0)
                    {
                        AImoveData = await MapManager.GetMoveToBrickPositionByTPL(ownerPosition);
                        if (AImoveData.IsLegalData)
                        {
                            InitializePath(ownerPosition, AImoveData.moveToPosition, AImoveData.breakablePosition);
                            return;
                        }
                    }
                }
                else
                {
                    InitializePath(ownerPosition, AImoveData.moveToPosition, AImoveData.breakablePosition);
                    return;
                }
            }

            // Find a neighbour empty position to move to (move around randomly).
            Vector3 neighbourEmptyPosition = MapManager.GetNeighbourEmptyPosition(ownerPosition);
            if (neighbourEmptyPosition != Vector3.zero)
            {
                InitializePath(ownerPosition, neighbourEmptyPosition, Vector3.zero);
                return;
            }
        }

        protected virtual void InitializePath(Vector3 startPosition, Vector3 endPosition, Vector3 breakablePosition)
        {
            _moveToPosition = endPosition;
            _breakablePosition = breakablePosition;
            _pathPositions = MapManager.GetPathPositions(startPosition, endPosition);
            _pathMoveIndex = 0;
            _pathTargetPosition = startPosition;
            _hasFoundAPath = true;
        }

        #endregion Class Methods
    }
}