using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    public static class MapManager
    {
        #region Members

        private static int width;
        private static int height;
        private static int maxSize;
        private static MapSlot[,] mapStatus;
        private static PathFinder pathFinder;

        public static int Width => width;
        public static int Height => height;

        #endregion Members

        #region Class Methods

        /// <summary>
        /// Init data for the map.
        /// </summary>
        /// <param name="mapSlots"></param>
        public static void Init(MapSlot[,] mapSlots)
        {
            width = mapSlots.GetLength(1);
            height = mapSlots.GetLength(0);
            maxSize = Mathf.Max(width, height);
            mapStatus = mapSlots;
            pathFinder = new AStarPathFinder();
        }

        /// <summary>
        /// Get a list of shortest path positions between the start position and the end position.
        /// </summary>
        public static List<Vector2> GetPathPositions(Vector3 startPosition, Vector3 endPosition)
        {
            ////////////////////////////////////////

            /*  DON'T KNOW WHY I FORGOT TO SAVE THIS ARRAY OF NODES, SO THAT IT DOESN'T NEED TO INITIALIZE EVERY DAMN TIME */

            ////////////////////////////////////////
            
            Node[,] nodes = new Node[height, width];
            for (int gridIndexY = 0; gridIndexY < height; gridIndexY++)
                for (int gridIndexX = 0; gridIndexX < width; gridIndexX++)
                    nodes[gridIndexY, gridIndexX] = new AStarNode(mapStatus[gridIndexY, gridIndexX].IsEmpty, gridIndexX, gridIndexY);

            Node startNode = nodes[(int)(startPosition.z / MapSetting.MapSquareSize), (int)(startPosition.x / MapSetting.MapSquareSize)];
            Node endNode = nodes[(int)(endPosition.z / MapSetting.MapSquareSize), (int)(endPosition.x / MapSetting.MapSquareSize)];

            List<Node> shortestPathNodes = pathFinder.Find(nodes, startNode, endNode);
            return shortestPathNodes.Select(t => new Vector2(t.GridIndexX * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                             t.GridIndexY * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize)).ToList();
        }

        /// <summary>
        /// Get a list of shortest path positions between the start position and the end position (always assume both are traversable).
        /// </summary>
        public static List<Vector2> GetTraversablePathPositions(Vector3 startPosition, Vector3 endPosition)
        {
            ////////////////////////////////////////

            /*  DON'T KNOW WHY I FORGOT TO SAVE THIS ARRAY OF NODES, SO THAT IT DOESN'T NEED TO INITIALIZE EVERY DAMN TIME */

            ////////////////////////////////////////

            Node[,] nodes = new Node[height, width];
            for (int gridIndexY = 0; gridIndexY < height; gridIndexY++)
                for (int gridIndexX = 0; gridIndexX < width; gridIndexX++)
                    nodes[gridIndexY, gridIndexX] = new AStarNode(mapStatus[gridIndexY, gridIndexX].IsEmpty, gridIndexX, gridIndexY);

            Node startNode = nodes[(int)(startPosition.z / MapSetting.MapSquareSize), (int)(startPosition.x / MapSetting.MapSquareSize)];
            startNode.IsTraversable = true;

            Node endNode = nodes[(int)(endPosition.z / MapSetting.MapSquareSize), (int)(endPosition.x / MapSetting.MapSquareSize)];
            endNode.IsTraversable = true;

            List<Node> shortestPathNodes = pathFinder.Find(nodes, startNode, endNode);
            return shortestPathNodes.Select(t => new Vector2(t.GridIndexX * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                             t.GridIndexY * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize)).ToList();
        }

        /// <summary>
        /// Get an empty position where a character can stand in the map and move freely in at least 3 tiles.
        /// </summary>
        public static Vector3 GetCharacterPosition(ref List<MapAnchor> characterOccupiedSlots)
        {
            List<MapAnchor> emptySlots = new List<MapAnchor>();
            List<MapAnchor> backupEmptySlots = new List<MapAnchor>();

            for (int gridIndexY = 0; gridIndexY < height; gridIndexY++)
            {
                for (int gridIndexX = 0; gridIndexX < width; gridIndexX++)
                {
                    if (mapStatus[gridIndexY, gridIndexX].IsEmpty)
                    {
                        backupEmptySlots.Add(new MapAnchor(gridIndexY, gridIndexX));
                        bool isBreak = false;

                        for (int i = -1; i <= 1 && !isBreak; i++)
                        {
                            for (int j = -1; j <= 1 && !isBreak; j++)
                            {
                                if ((i == 0 || j == 0) && (i ^ j) != 0)
                                {
                                    if (gridIndexX + j >= 0 && gridIndexX + j < width && gridIndexY + i >= 0 && gridIndexY + i < height)
                                    {
                                        if (mapStatus[gridIndexY + i, gridIndexX + j].IsEmpty)
                                        {
                                            for (int k = -1; k <= 1 && !isBreak; k++)
                                            {
                                                for (int l = -1; l <= 1; l++)
                                                {
                                                    if ((k == 0 || l == 0) && (k ^ l) != 0)
                                                    {
                                                        if (gridIndexX + j + l >= 0 && gridIndexX + j + l < width &&
                                                            gridIndexY + i + k >= 0 && gridIndexY + i + k < height &&
                                                            gridIndexX + j + l != gridIndexX && gridIndexY + i + k != gridIndexY)
                                                        {
                                                            if (mapStatus[gridIndexY + i + k, gridIndexX + j + l].IsEmpty)
                                                            {
                                                                emptySlots.Add(new MapAnchor(gridIndexY + i + k, gridIndexX + j + l));
                                                                isBreak = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            characterOccupiedSlots.ForEach(slot =>
            {
                emptySlots.Remove(slot);
                backupEmptySlots.Remove(slot);
            });

            MapAnchor randomEmptySlot = MapAnchor.illegalAnchor;
            if (emptySlots.Count > 0)
            {
                emptySlots.Shuffle();
                while (emptySlots.Count > 0)
                {
                    bool isBreak = true;
                    randomEmptySlot = emptySlots.GetRandomFromList();
                    emptySlots.Remove(randomEmptySlot);

                    foreach (var characterOccupiedSlot in characterOccupiedSlots)
                    {
                        int x = randomEmptySlot.x - characterOccupiedSlot.x;
                        int y = randomEmptySlot.y - characterOccupiedSlot.y;
                        if (Mathf.Abs(x) < Constants.CharacterSpawnSeparationDistanceTiles && Mathf.Abs(y) < Constants.CharacterSpawnSeparationDistanceTiles)
                        {
                            isBreak = false;
                            break;
                        }
                    }

                    if (isBreak)
                        break;
                }
            }

            if (emptySlots.Count == 0)
                randomEmptySlot = backupEmptySlots.GetRandomFromList();

            characterOccupiedSlots.Add(randomEmptySlot);
            return new Vector3(randomEmptySlot.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                               MapSetting.AboveGroundHeight,
                               randomEmptySlot.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
        }

        /// <summary>
        /// Get a position in the map from the input position.
        /// The position in the map here means the position is in the center of every "block".
        /// </summary>
        public static Vector3 GetMapPosition(Vector3 inputPosition)
        {
            int gridIndexX = (int)(inputPosition.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(inputPosition.z / MapSetting.MapSquareSize);

            return new Vector3(gridIndexX * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                               MapSetting.AboveGroundHeight,
                               gridIndexY * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
        }

        /// <summary>
        /// Update the map slot type of a position in the map, determine whether that position is walkable or not.
        /// Called in runtime as objects are spawned, destroyed.
        /// </summary>
        /// <param name="position">The position of an object in the map.</param>
        /// <param name="slotType">The type of this map slot.</param>
        public static void UpdateMap(Vector3 position, MapSlotType slotType)
        {
            int gridIndexX = (int)(position.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(position.z / MapSetting.MapSquareSize);
            mapStatus[gridIndexY, gridIndexX].slotType = slotType;
        }

        /// <summary>
        /// Update the map slot type of an anchor in the map, determine whether that anchor is walkable or not.
        /// Called in runtime as objects are spawned, destroyed.
        /// </summary>
        /// <param name="anchor">The anchor of an object in the map.</param>
        /// <param name="slotType">The type of this map anchor.</param>
        public static void UpdateMap(MapAnchor anchor, MapSlotType slotType)
        {
            mapStatus[anchor.y, anchor.x].slotType = slotType;
        }

        /// <summary>
        /// Get the map slot type of the input position.
        /// </summary>
        /// <param name="position">The examined position.</param>
        public static MapSlotType GetMapSlotType(Vector3 position)
        {
            int gridIndexX = (int)(position.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(position.z / MapSetting.MapSquareSize);
            return mapStatus[gridIndexY, gridIndexX].slotType;
        }

        /// <summary>
        /// Check if a position in the map is empty.
        /// </summary>
        /// <param name="position">The examined position.</param>
        public static bool IsEmptyPosition(Vector3 position)
        {
            int gridIndexX = (int)(position.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(position.z / MapSetting.MapSquareSize);
            return mapStatus[gridIndexY, gridIndexX].IsEmpty;
        }

        /// <summary>
        /// Check if a position in the map is bomb.
        /// </summary>
        /// <param name="position">The examined position.</param>
        public static bool IsBombPosition(Vector3 position)
        {
            int gridIndexX = (int)(position.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(position.z / MapSetting.MapSquareSize);
            return mapStatus[gridIndexY, gridIndexX].IsBomb;
        }

        /// <summary>
        /// Check if a position in the map is block.
        /// </summary>
        /// <param name="position">The examined position.</param>
        public static bool IsBlockPosition(Vector3 position)
        {
            int gridIndexX = (int)(position.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(position.z / MapSetting.MapSquareSize);
            return mapStatus[gridIndexY, gridIndexX].IsBlock;
        }

        /// <summary>
        /// Check if a position in the map is breakable.
        /// </summary>
        /// <param name="position">The examined position.</param>
        public static bool IsBreakablePosition(Vector3 position)
        {
            int gridIndexX = (int)(position.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(position.z / MapSetting.MapSquareSize);
            return mapStatus[gridIndexY, gridIndexX].IsBreakable;
        }


        /// <summary>
        /// Get a list of map anchors where chests are there.
        /// </summary>
        public static List<MapAnchor> GetChestSlots()
        {
            List<MapAnchor> slots = new List<MapAnchor>();

            for (int gridIndexY = 0; gridIndexY < height; gridIndexY++)
                for (int gridIndexX = 0; gridIndexX < width; gridIndexX++)
                    if (mapStatus[gridIndexY, gridIndexX].IsChestBox)
                        slots.Add(new MapAnchor(gridIndexY, gridIndexX));

            return slots;
        }

        /// <summary>
        /// Get a move data including a move to position and a chest position(using TPL).
        /// </summary>
        /// <param name="characterPosition">The position of the character.</param>>
        public static Task<AIMoveData> GetMoveToChestPositionByTPL(Vector3 characterPosition, List<MapAnchor> chestSlots)
        {
            return Task.Run(() => GetMoveToChestPosition(characterPosition, chestSlots));
        }

        /// <summary>
        /// Get a move data including a move to position and a chest position.
        /// </summary>
        /// <param name="characterPosition">The position of the character.</param>>
        public static AIMoveData GetMoveToChestPosition(Vector3 characterPosition, List<MapAnchor> chestSlots)
        {
            List<Vector3> chestPoints = chestSlots.Select(chestSlot => new Vector3(chestSlot.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                                   MapSetting.AboveGroundHeight,
                                                                                   chestSlot.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize)).ToList();
            return GetMoveToChestPosition(characterPosition, chestPoints);
        }

        /// <summary>
        /// Get a move data including a move to position and a chest position.
        /// </summary>
        /// <param name="characterPosition">The position of the character.</param>>
        public static AIMoveData GetMoveToChestPosition(Vector3 characterPosition, List<Vector3> chestPoints)
        {
            int findPathToChestTimes = 0;
            while (chestPoints.Count > 0)
            {
                float minDistanceSqr = float.MaxValue;
                Vector3 nearestChestPoint = Vector3.zero;

                foreach (var chestPoint in chestPoints)
                {
                    float distanceSqr = (chestPoint.x - characterPosition.x) * (chestPoint.x - characterPosition.x) + (chestPoint.z - characterPosition.z) * (chestPoint.z - characterPosition.z);
                    if (distanceSqr < minDistanceSqr)
                    {
                        minDistanceSqr = distanceSqr;
                        nearestChestPoint = chestPoint;
                    }
                }

                chestPoints.Remove(nearestChestPoint);
                var pathPositions = GetTraversablePathPositions(characterPosition, nearestChestPoint);
                if (pathPositions.Count <= 2)
                {
                    findPathToChestTimes++;
                    if (findPathToChestTimes >= Constants.GiveUpFindPathToChestTimes)
                        break;
                }
                else return new AIMoveData(new Vector3(pathPositions[pathPositions.Count - 2].x, nearestChestPoint.y, pathPositions[pathPositions.Count - 2].y), nearestChestPoint);
            }

            return AIMoveData.illegalMoveData;
        }

        /// <summary>
        /// Get a move data including a move to position and a brick position(using TPL).
        /// </summary>
        /// <param name="characterPosition">The position of the character.</param>>
        public static Task<AIMoveData> GetMoveToBrickPositionByTPL(Vector3 characterPosition)
        {
            return Task.Run(() => GetMoveToBrickPosition(characterPosition));
        }

        /// <summary>
        /// Get a move data including a move to position and a brick position.
        /// </summary>
        /// <param name="characterPosition">The position of the character.</param>>
        public static AIMoveData GetMoveToBrickPosition(Vector3 characterPosition)
        {
            int gridIndexX = (int)(characterPosition.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(characterPosition.z / MapSetting.MapSquareSize);
            int centerOffset = 1;

            while (centerOffset < maxSize)
            {
                List<MapAnchor> brickSlots = new List<MapAnchor>();

                for (int offset = -centerOffset; offset <= centerOffset; offset++)
                {
                    if (gridIndexX + offset >= 0 && gridIndexX + offset < width && gridIndexY + centerOffset < height)
                    {
                        if (mapStatus[gridIndexY + centerOffset, gridIndexX + offset].IsBrick)
                            brickSlots.Add(new MapAnchor(gridIndexY + centerOffset, gridIndexX + offset));
                    }
                }

                for (int offset = -centerOffset; offset <= centerOffset; offset++)
                {
                    if (gridIndexX + offset >= 0 && gridIndexX + offset < width && gridIndexY - centerOffset >= 0)
                    {
                        if (mapStatus[gridIndexY - centerOffset, gridIndexX + offset].IsBrick)
                            brickSlots.Add(new MapAnchor(gridIndexY - centerOffset, gridIndexX + offset));
                    }
                }

                for (int offset = -centerOffset + 1; offset <= centerOffset - 1; offset++)
                {
                    if (gridIndexX - centerOffset >= 0 && gridIndexY + offset >= 0 && gridIndexY + offset < height)
                    {
                        if (mapStatus[gridIndexY + offset, gridIndexX - centerOffset].IsBrick)
                            brickSlots.Add(new MapAnchor(gridIndexY + offset, gridIndexX - centerOffset));
                    }
                }

                for (int offset = -centerOffset + 1; offset <= centerOffset - 1; offset++)
                {
                    if (gridIndexX + centerOffset < width && gridIndexY + offset >= 0 && gridIndexY + offset < height)
                    {
                        if (mapStatus[gridIndexY + offset, gridIndexX + centerOffset].IsBrick)
                            brickSlots.Add(new MapAnchor(gridIndexY + offset, gridIndexX + centerOffset));
                    }
                }

                while (brickSlots.Count > 0)
                {
                    MapAnchor randomBrickAnchor = brickSlots.GetRandomFromList();
                    brickSlots.Remove(randomBrickAnchor);

                    Vector3 brickPosition = new Vector3(randomBrickAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                        MapSetting.AboveGroundHeight,
                                                        randomBrickAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                    var pathPositions = GetTraversablePathPositions(characterPosition, brickPosition);
                    if (pathPositions.Count > 2)
                        return new AIMoveData(new Vector3(pathPositions[pathPositions.Count - 2].x, brickPosition.y, pathPositions[pathPositions.Count - 2].y), brickPosition);
                }

                centerOffset++;
            }

            return AIMoveData.illegalMoveData;
        }

        /// <summary>
        /// Get a move away position from the target.
        /// </summary>
        /// <param name="characterPosition">The position of the character.</param>
        /// <param name="targetPosition">The position of the target character.</param>
        /// <param name="minMoveAwayTargetTiles">The number of min tiles that's away from the target.</param>
        /// <param name="maxMoveAwayTargetTiles">The number of max tiles that's away from the target.</param>
        public static Vector3 GetMoveAwayTargetPosition(Vector3 characterPosition, Vector3 targetPosition, int minMoveAwayTargetTiles, int maxMoveAwayTargetTiles)
        {
            Vector3 direction = (characterPosition - targetPosition).normalized;
            Vector3 crossDirection = Vector3.Cross(direction, Vector3.up);
            List<Vector3> moveAwayDirections = new List<Vector3>() { direction, crossDirection, -crossDirection };

            while (moveAwayDirections.Count > 0)
            {
                Vector3 randomMoveAwayDirection = moveAwayDirections.GetRandomFromList();
                moveAwayDirections.Remove(randomMoveAwayDirection);

                int currentCheckingMoveAwayTargetTiles = minMoveAwayTargetTiles;
                while (currentCheckingMoveAwayTargetTiles <= maxMoveAwayTargetTiles)
                {
                    Vector3 checkedPosition = characterPosition + randomMoveAwayDirection * (currentCheckingMoveAwayTargetTiles * MapSetting.MapSquareSize);
                    int gridIndexX = (int)(checkedPosition.x / MapSetting.MapSquareSize);
                    int gridIndexY = (int)(checkedPosition.z / MapSetting.MapSquareSize);
                    if (gridIndexX >= 0 && gridIndexX < width && gridIndexY >= 0 && gridIndexY < height)
                    {
                        if (mapStatus[gridIndexY, gridIndexX].IsEmpty)
                        {
                            Vector3 emptyPosition = new Vector3(gridIndexX * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                MapSetting.AboveGroundHeight,
                                                                gridIndexY * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);

                            if (GetPathPositions(characterPosition, emptyPosition).HasPath())
                                return emptyPosition;
                        }
                    }
                    else break;

                    currentCheckingMoveAwayTargetTiles++;
                }
            }

            return Vector3.zero;
        }

        /// Get a random neighbour empty position around the target position provided that a possible path can be found.
        /// </summary>
        public static Vector3 GetNeighbourTargetPosition(Vector3 fromPosition, Vector3 targetPosition)
        {
            int gridIndexX = (int)(targetPosition.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(targetPosition.z / MapSetting.MapSquareSize);
            List<MapAnchor> emptySlots = new List<MapAnchor>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i == 0 || j == 0) && (i ^ j) != 0)
                    {
                        if (mapStatus[gridIndexY + i, gridIndexX + j].IsEmpty)
                            emptySlots.Add(new MapAnchor(gridIndexY + i, gridIndexX + j));
                    }
                }
            }

            while (emptySlots.Count > 0)
            {
                MapAnchor randomEmptySlot = emptySlots.GetRandomFromList();
                emptySlots.Remove(randomEmptySlot);
                Vector3 emptyPosition = new Vector3(randomEmptySlot.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                    MapSetting.AboveGroundHeight,
                                                    randomEmptySlot.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);

                if (GetPathPositions(fromPosition, emptyPosition).HasPath())
                    return emptyPosition;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Get a random neighbour empty position around the input position.
        /// </summary>
        public static Vector3 GetNeighbourEmptyPosition(Vector3 fromPosition)
        {
            int gridIndexX = (int)(fromPosition.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(fromPosition.z / MapSetting.MapSquareSize);
            List<MapAnchor> emptySlots = new List<MapAnchor>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        if (mapStatus[gridIndexY + i, gridIndexX + j].IsEmpty)
                            emptySlots.Add(new MapAnchor(gridIndexY + i, gridIndexX + j));
                    }
                }
            }

            if (emptySlots.Count > 0)
            {
                MapAnchor randomSlot = emptySlots.GetRandomFromList();
                return new Vector3(randomSlot.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                   MapSetting.AboveGroundHeight,
                                   randomSlot.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
            }
            else return Vector3.zero;
        }

        /// <summary>
        /// Check if the input position is surrounded by any breakable.
        /// </summary>
        /// <param name="position">The examined position.</param>
        public static bool IsSurroundedByBreakable(Vector3 position)
        {
            int gridIndexX = (int)(position.x / MapSetting.MapSquareSize);
            int gridIndexY = (int)(position.z / MapSetting.MapSquareSize);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i == 0 || j == 0) && (i ^ j) != 0)
                    {
                        int neighbourGridIndexY = gridIndexY + i;
                        int neighbourGridIndexX = gridIndexX + j;
                        if (mapStatus[neighbourGridIndexY, neighbourGridIndexX].IsBreakable)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a position in the map can be movable.
        /// </summary>
        /// <param name="position">The examined position.</param>
        public static bool IsMovable(Vector3 lastPosition, Vector3 moveToPosition)
        {
            int lastGridIndexX = (int)(lastPosition.x / MapSetting.MapSquareSize);
            int lastGridIndexY = (int)(lastPosition.z / MapSetting.MapSquareSize);
            int moveToGridIndexX = (int)(moveToPosition.x / MapSetting.MapSquareSize);
            int moveToGridIndexY = (int)(moveToPosition.z / MapSetting.MapSquareSize);
            return mapStatus[moveToGridIndexY, moveToGridIndexX].IsEmpty || (mapStatus[lastGridIndexY, lastGridIndexX].IsBomb && mapStatus[moveToGridIndexY, moveToGridIndexX].IsBomb);
        }

        /// <summary>
        /// Get a random anchor that is around the input block anchor.
        /// </summary>
        public static MapAnchor GetAnchorAroundBlockAnchor(MapAnchor blockAnchor)
        {
            Queue<MapAnchor> examinedAnchors = new Queue<MapAnchor>();
            examinedAnchors.Enqueue(blockAnchor);

            while (examinedAnchors.Count > 0)
            {
                MapAnchor examinedAnchor = examinedAnchors.Dequeue();
                List<MapAnchor> emptyNeighbourAnchors = new List<MapAnchor>();
                List<MapAnchor> nonEmptyNeighbourAnchors = new List<MapAnchor>();
                int examinedAnchorY = examinedAnchor.y;
                int examinedAnchorX = examinedAnchor.x;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            int neighbourAnchorY = examinedAnchorY + i;
                            int neighbourAnchorX = examinedAnchorX + j;
                            if (neighbourAnchorX >= 0 && neighbourAnchorX < width && neighbourAnchorY >= 0 && neighbourAnchorY < height)
                            {
                                if (mapStatus[neighbourAnchorY, neighbourAnchorX].IsEmpty)
                                    emptyNeighbourAnchors.Add(new MapAnchor(neighbourAnchorY, neighbourAnchorX));
                                else
                                    nonEmptyNeighbourAnchors.Add(new MapAnchor(neighbourAnchorY, neighbourAnchorX));
                            }
                        }
                    }
                }

                if (emptyNeighbourAnchors.Count == 0)
                {
                    nonEmptyNeighbourAnchors.Shuffle();
                    foreach (var nonEmptyNeightbourAnchor in nonEmptyNeighbourAnchors)
                        examinedAnchors.Enqueue(nonEmptyNeightbourAnchor);
                }
                else return emptyNeighbourAnchors.GetRandomFromList();
            }

            return MapAnchor.illegalAnchor;
        }

        /// <summary>
        /// Check if a position is not blocked in the input specified direction.
        /// </summary>
        /// <param name="worldPosition">The examined world position.</param>
        /// <param name="checkDirection">The direction for checking.</param>
        public static bool IsNotBlocked(Vector3 worldPosition, MapDirection checkDirection)
        {
            int gridIndexY = (int)(worldPosition.z / MapSetting.MapSquareSize);
            int gridIndexX = (int)(worldPosition.x / MapSetting.MapSquareSize);

            if (checkDirection == MapDirection.Horizontal)
            {
                return (gridIndexX + 1) < width && mapStatus[gridIndexY, gridIndexX + 1].IsEmpty ||
                       (gridIndexX - 1) >= 0 && mapStatus[gridIndexY, gridIndexX - 1].IsEmpty;
            }
            else
            {
                return (gridIndexY + 1) < height && mapStatus[gridIndexY + 1, gridIndexX].IsEmpty ||
                       (gridIndexY - 1) >= 0 && mapStatus[gridIndexY - 1, gridIndexX].IsEmpty;
            }
        }

        /// <summary>
        /// Check if a position is blocked in the input specified direction.
        /// </summary>
        /// <param name="worldPosition">The examined world position.</param>
        /// <param name="checkDirection">The direction for checking.</param>
        public static bool IsBlocked(Vector3 worldPosition, MapDirection checkDirection)
        {
            return !IsNotBlocked(worldPosition, checkDirection);
        }

        /// <summary>
        /// Check if a position is not fully blocked in four directions.
        /// </summary>
        /// <param name="worldPosition">The examined world position.</param>
        public static bool IsNotFullyBlocked(Vector3 worldPosition)
        {
            int gridIndexY = (int)(worldPosition.z / MapSetting.MapSquareSize);
            int gridIndexX = (int)(worldPosition.x / MapSetting.MapSquareSize);

            return mapStatus[gridIndexY, gridIndexX + 1].IsEmpty ||
                   mapStatus[gridIndexY, gridIndexX - 1].IsEmpty ||
                   mapStatus[gridIndexY + 1, gridIndexX].IsEmpty ||
                   mapStatus[gridIndexY - 1, gridIndexX].IsEmpty;
        }

        /// <summary>
        /// Check if a position is fully blocked in four directions.
        /// </summary>
        /// <param name="worldPosition">The examined world position.</param>
        public static bool IsFullyBlocked(Vector3 worldPosition)
        {
            return !IsNotFullyBlocked(worldPosition);
        }

        /// <summary>
        /// Get a non-blocked position based on the world position and the blocked direction.
        /// </summary>
        /// <param name="worldPosition">The examined world position.</param>
        /// <param name="blockedDirection">The blocked direction.</param>
        /// <param name="nonBlockedPosition">The non-blocked position returned is a position that is not blocked and is in different
        /// direction from the blocked direction.</param>
        public static bool GetNonBlockedPosition(Vector3 worldPosition, MapDirection blockedDirection, out Vector3 nonBlockedPosition)
        {
            int gridIndexY = (int)(worldPosition.z / MapSetting.MapSquareSize);
            int gridIndexX = (int)(worldPosition.x / MapSetting.MapSquareSize);

            if (blockedDirection == MapDirection.Horizontal)
            {
                if (UnityEngine.Random.Range(0, 2) % 2 == 0)
                {
                    if ((gridIndexY + 1) < height && mapStatus[gridIndexY + 1, gridIndexX].IsEmpty)
                    {
                        nonBlockedPosition = new Vector3(gridIndexX * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                         worldPosition.y,
                                                         (gridIndexY + 1) * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        return true;
                    }
                    else if ((gridIndexY - 1) >= 0 && mapStatus[gridIndexY - 1, gridIndexX].IsEmpty)
                    {
                        nonBlockedPosition = new Vector3(gridIndexX * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                         worldPosition.y,
                                                         (gridIndexY - 1) * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        return true;
                    }
                }
                else
                {
                    if ((gridIndexY - 1) >= 0 && mapStatus[gridIndexY - 1, gridIndexX].IsEmpty)
                    {
                        nonBlockedPosition = new Vector3(gridIndexX * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                         worldPosition.y,
                                                         (gridIndexY - 1) * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        return true;
                    }
                    else if ((gridIndexY + 1) < height && mapStatus[gridIndexY + 1, gridIndexX].IsEmpty)
                    {
                        nonBlockedPosition = new Vector3(gridIndexX * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                         worldPosition.y,
                                                         (gridIndexY + 1) * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        return true;
                    }
                }
            }
            else
            {
                if (UnityEngine.Random.Range(0, 2) % 2 == 0)
                {
                    if ((gridIndexX + 1) < width && mapStatus[gridIndexY, gridIndexX + 1].IsEmpty)
                    {
                        nonBlockedPosition = new Vector3((gridIndexX + 1) * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                          worldPosition.y,
                                                          gridIndexY * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        return true;
                    }
                    else if ((gridIndexX - 1) >= 0 && mapStatus[gridIndexY, gridIndexX - 1].IsEmpty)
                    {
                        nonBlockedPosition = new Vector3((gridIndexX - 1) * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                          worldPosition.y,
                                                          gridIndexY * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        return true;
                    }
                }
                else
                {
                    if ((gridIndexX - 1) >= 0 && mapStatus[gridIndexY, gridIndexX - 1].IsEmpty)
                    {
                        nonBlockedPosition = new Vector3((gridIndexX - 1) * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                          worldPosition.y,
                                                          gridIndexY * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        return true;
                    }
                    else if ((gridIndexX + 1) < width && mapStatus[gridIndexY, gridIndexX + 1].IsEmpty)
                    {
                        nonBlockedPosition = new Vector3((gridIndexX + 1) * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                          worldPosition.y,
                                                          gridIndexY * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        return true;
                    }
                }
            }

            nonBlockedPosition = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Get a random off map border position, off positions are in the corner and the center of the map.
        /// </summary>
        /// <param name="fromPosition"></param>
        /// <returns></returns>
        public static Vector3 GetOffBorderPosition(Vector3 fromPosition)
        {
            int gridIndexY = (int)(fromPosition.z / MapSetting.MapSquareSize);
            int gridIndexX = (int)(fromPosition.x / MapSetting.MapSquareSize);
            int offBorderDistance = (width + height) / 2;

            var borderAnchors = new List<MapAnchor>()
            {
                new MapAnchor(-offBorderDistance,-offBorderDistance),
                new MapAnchor(height/2, -offBorderDistance),
                new MapAnchor(height - 1 + offBorderDistance, -offBorderDistance),
                new MapAnchor(height - 1 + offBorderDistance, width/2),
                new MapAnchor(height - 1 + offBorderDistance, width - 1+ offBorderDistance),
                new MapAnchor(height/2, width - 1 + offBorderDistance),
                new MapAnchor(-offBorderDistance, width - 1 + offBorderDistance),
                new MapAnchor(-offBorderDistance, width/2),
            };

            borderAnchors = borderAnchors.Where(anchor => Mathf.Abs(anchor.y - gridIndexY) >= height / 2 || Mathf.Abs(anchor.x - gridIndexX) >= width / 2).ToList();
            MapAnchor randomBorderAnchor = borderAnchors.GetRandomFromList();
            return new Vector3(randomBorderAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                               MapSetting.AboveGroundHeight,
                               randomBorderAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
        }

        #endregion Class Methods
    }

    public enum MapDirection
    {
        Horizontal,
        Vertical
    }

    public struct MapAnchor : IEquatable<MapAnchor>
    {
        #region Members

        public static MapAnchor illegalAnchor = new MapAnchor(-1, -1);

        public int y;
        public int x;

        #endregion Members

        #region Struct Methods

        public MapAnchor(int y, int x)
        {
            this.y = y;
            this.x = x;
        }

        public bool Equals(MapAnchor other)
        {
            return y == other.y && x == other.x;
        }

        public static bool operator ==(MapAnchor anchor1, MapAnchor anchor2)
        {
            return anchor1.Equals(anchor2);
        }

        public static bool operator !=(MapAnchor anchor1, MapAnchor anchor2)
        {
            return !(anchor1 == anchor2);
        }

        public override bool Equals(object obj)
        {
            return obj is MapAnchor anchor && anchor.Equals(this);
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() + x.GetHashCode();
        }

        public override string ToString()
        {
            string info = "y = " + y + " - x = " + x;
            return info;
        }

        #endregion Struct Methods
    }
}