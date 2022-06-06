using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class NetworkLevelLoader : NetworkBehaviour
    {
        #region Members

        protected const int definedMoveableTile1ID = 0;
        protected const int definedMoveableTile2ID = 3;
        protected const int definedMoveableTile3ID = 4;
        protected const int definedMoveableTile4ID = 5;
        protected const int definedPrepareMoveableTileID = 6;
        protected const int definedPrepareLockTileID = 7;
        protected const int definedWallTileID = 1;
        protected const int definedBlockTileID = 2;

        protected static string mapLayoutDataString = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1;1,0,0,4,4,3,0,4,5,0,3,4,4,4,0,0,1;1,4,2,5,2,0,3,4,2,3,3,5,2,5,2,4,1;1,5,4,5,2,5,2,0,5,0,2,0,2,4,5,4,1;1,0,0,4,5,4,3,0,4,0,0,3,0,4,4,5,1;1,0,2,0,2,4,4,0,0,0,4,0,2,3,2,4,1;1,4,0,3,0,4,2,5,2,0,2,4,0,0,0,0,1;1,4,5,0,0,5,4,0,3,5,4,4,3,0,3,3,1;1,4,2,5,2,0,4,2,3,2,5,4,2,4,2,0,1;1,5,5,4,0,0,3,0,4,5,4,5,5,5,0,0,1;1,5,4,0,4,2,2,0,5,4,2,2,5,0,3,3,1;1,5,0,5,5,0,3,4,2,4,4,5,0,3,0,5,1;1,4,3,2,5,3,2,5,4,5,2,0,3,2,5,5,1;1,0,0,5,0,0,4,5,4,5,5,0,3,5,0,5,1;1,5,3,2,3,0,5,4,2,5,0,3,0,2,3,0,1;1,4,5,3,0,0,4,4,5,0,3,3,5,5,0,3,1;1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1";
        protected static string mapDetailDataString = "1,1,1,0.10,10,300,15,15,225,64,0.15,34,0.15,34,0.20,45,0.05,2,110,37.50,50,0.40,0.60,20,22,1,0,0,0,0,2,0,0,0,0,1,0,0,0,0,20,0,0,0,0,30,6,1,0,0,0,0,6,0,0,0,0,200,60,0.30";

        [Header("Moveable tile prefabs")]
        [SerializeField]
        protected GameObject _moveableTile1Prefab;
        [SerializeField]
        protected GameObject _moveableTile2Prefab;
        [SerializeField]
        protected GameObject _moveableTile3Prefab;
        [SerializeField]
        protected GameObject _moveableTile4Prefab;

        [Header("Wall tile prefab")]
        [SerializeField]
        protected GameObject _wallTilePrefab;

        [Header("Block tile prefab")]
        [SerializeField]
        protected GameObject _blockTilePrefab;

        [Header("Brick decor tile prefabs")]
        [SerializeField]
        protected GameObject _brickDecorTile1Prefab;
        [SerializeField]
        protected GameObject _brickDecorTile2Prefab;

        [Header("Brick box tile prefab")]
        [SerializeField]
        protected GameObject _brickBoxTilePrefab;

        [Header("Chest box tile prefabs")]
        [SerializeField]
        protected GameObject _woodenChestTilePrefab;
        [SerializeField]
        protected GameObject _bronzeChestTilePrefab;
        [SerializeField]
        protected GameObject _silverChestTilePrefab;
        [SerializeField]
        protected GameObject _goldenChestTilePrefab;
        [SerializeField]
        protected GameObject _diamondChestTilePrefab;

        [Header("Boosters data")]
        [SerializeField]
        protected BoosterData[] _boostersData;

        protected List<int> _boosterSpawnTypes;
        protected int _spawnChestBrickBoxTotal;
        protected int _spawnChestHiddenChestTotal;
        protected int _spawnBoosterBrickBoxTotal;
        protected int _spawnHiddenBoosterTotal;
        protected MapDetailData _loadedMapDetailData;

        #endregion Members

        #region Class Methods

        public override void OnStartServer()
        {
            List<MapAnchor> blockAnchors;
            CreateBaseTiles(out blockAnchors);
            CreateDetailTiles(blockAnchors);

            InitBoosterSpawnData();
            EventManager.AddListener<Vector3>(GameEventType.ExposeHiddenItem, OnExposeHiddenItem);
        }

        protected virtual void InitBoosterSpawnData()
        {
            _boosterSpawnTypes = new List<int>();
            for (int i = 0; i < _boostersData.Length; i++)
                for (int j = 0; j < _boostersData[i].maxNumber; j++)
                    _boosterSpawnTypes.Add(i);
            _boosterSpawnTypes.Shuffle();
            _spawnHiddenBoosterTotal = _boosterSpawnTypes.Count;
        }

        protected virtual void CreateBaseTiles(out List<MapAnchor> blockAnchors)
        {
            List<string> mapTileTextLines = new List<string>(mapLayoutDataString.Split(';'));
            mapTileTextLines.Reverse();
            int mapWidth = mapTileTextLines[0].Split(',').Count();
            int mapHeight = mapTileTextLines.Count;
            MapSlot[,] mapSlots = new MapSlot[mapHeight, mapWidth];
            blockAnchors = new List<MapAnchor>();

            for (int i = 0; i < mapTileTextLines.Count; i++)
            {
                string[] mapTileIDs = mapTileTextLines[i].Split(',');
                for (int j = 0; j < mapTileIDs.Length; j++)
                {
                    int mapTileID = Convert.ToInt32(mapTileIDs[j]);

                    switch (mapTileID)
                    {
                        case definedWallTileID:
                            GameObject wallTileGameObject = Instantiate(_wallTilePrefab);
                            Vector3 wallTilePosition = new Vector3(j * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                   MapSetting.WallHeight,
                                                                   i * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                            wallTileGameObject.transform.SetParent(transform);
                            wallTileGameObject.transform.position = wallTilePosition;
                            mapSlots[i, j] = new MapSlot(MapSlotType.Block);
                            NetworkServer.Spawn(wallTileGameObject, _wallTilePrefab.GetComponent<NetworkIdentity>().assetId);
                            break;

                        case definedMoveableTile1ID:
                        case definedMoveableTile2ID:
                        case definedMoveableTile3ID:
                        case definedMoveableTile4ID:
                        case definedPrepareMoveableTileID:
                        case definedPrepareLockTileID:
                            GameObject moveableTileGameObject;
                            Vector3 moveableTilePosition = new Vector3(j * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                       MapSetting.OnGroundHeight,
                                                                       i * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                            switch (mapTileID)
                            {
                                case definedMoveableTile1ID:
                                case definedPrepareMoveableTileID:
                                case definedPrepareLockTileID:
                                    moveableTileGameObject = Instantiate(_moveableTile1Prefab);
                                    moveableTileGameObject.transform.SetParent(transform);
                                    moveableTileGameObject.transform.position = moveableTilePosition;
                                    NetworkServer.Spawn(moveableTileGameObject, _moveableTile1Prefab.GetComponent<NetworkIdentity>().assetId);
                                    break;
                                case definedMoveableTile2ID:
                                    moveableTileGameObject = Instantiate(_moveableTile2Prefab);
                                    moveableTileGameObject.transform.SetParent(transform);
                                    moveableTileGameObject.transform.position = moveableTilePosition;
                                    NetworkServer.Spawn(moveableTileGameObject, _moveableTile2Prefab.GetComponent<NetworkIdentity>().assetId);
                                    break;
                                case definedMoveableTile3ID:
                                    moveableTileGameObject = Instantiate(_moveableTile3Prefab);
                                    moveableTileGameObject.transform.SetParent(transform);
                                    moveableTileGameObject.transform.position = moveableTilePosition;
                                    NetworkServer.Spawn(moveableTileGameObject, _moveableTile3Prefab.GetComponent<NetworkIdentity>().assetId);
                                    break;
                                case definedMoveableTile4ID:
                                    moveableTileGameObject = Instantiate(_moveableTile4Prefab);
                                    moveableTileGameObject.transform.SetParent(transform);
                                    moveableTileGameObject.transform.position = moveableTilePosition;
                                    NetworkServer.Spawn(moveableTileGameObject, _moveableTile4Prefab.GetComponent<NetworkIdentity>().assetId);
                                    break;
                            }
                            mapSlots[i, j] = new MapSlot(MapSlotType.Empty);
                            break;

                        case definedBlockTileID:
                            GameObject blockTileGameObject = Instantiate(_blockTilePrefab);
                            Vector3 blockTilePosition = new Vector3(j * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                    MapSetting.AboveGroundHeight,
                                                                    i * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                            blockTileGameObject.transform.SetParent(transform);
                            blockTileGameObject.transform.position = blockTilePosition;
                            mapSlots[i, j] = new MapSlot(MapSlotType.Block);
                            blockAnchors.Add(new MapAnchor(i, j));
                            NetworkServer.Spawn(blockTileGameObject, _blockTilePrefab.GetComponent<NetworkIdentity>().assetId);
                            break;
                    }
                }
            }

            MapManager.Init(mapSlots);
        }

        protected virtual void CreateDetailTiles(List<MapAnchor> blockAnchors)
        {
            List<string> mapDetails = new List<string>(mapDetailDataString.Split(','));
            _loadedMapDetailData = new MapDetailData(mapDetails);
            _spawnChestBrickBoxTotal = _loadedMapDetailData.brickBoxTotal;
            _spawnChestHiddenChestTotal = _loadedMapDetailData.hiddenChestTotal;
            _spawnBoosterBrickBoxTotal = _loadedMapDetailData.brickBoxTotal;

            int spawnTilesTotal = _loadedMapDetailData.brickDecorTotal + _loadedMapDetailData.brickBoxTotal + _loadedMapDetailData.chestTotal - _loadedMapDetailData.hiddenChestTotal;
            for (int i = 0; i < spawnTilesTotal; i++)
            {
                MapAnchor randomBlockAnchor = blockAnchors.GetRandomFromList();
                MapAnchor tileSpawnAnchor = MapManager.GetAnchorAroundBlockAnchor(randomBlockAnchor);
                MapSlotType mapSlotType = _loadedMapDetailData.GetSpawnMapSlotType();

                switch (mapSlotType)
                {
                    case MapSlotType.BrickDecor:
                        int randomBrickDecorType = UnityRandom.Range(1, 3);
                        GameObject brickPrefab = randomBrickDecorType == 1 ? _brickDecorTile1Prefab : _brickDecorTile2Prefab;
                        GameObject brickDecorTileGameObject = Instantiate(brickPrefab);
                        Vector3 brickDecorTilePosition = new Vector3(tileSpawnAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                     MapSetting.AboveGroundHeight,
                                                                     tileSpawnAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        brickDecorTileGameObject.transform.SetParent(transform);
                        brickDecorTileGameObject.transform.position = brickDecorTilePosition;
                        MapManager.UpdateMap(tileSpawnAnchor, mapSlotType);
                        NetworkServer.Spawn(brickDecorTileGameObject, brickPrefab.GetComponent<NetworkIdentity>().assetId);
                        break;

                    case MapSlotType.BrickBox:
                        GameObject brickBoxTileGameObject = Instantiate(_brickBoxTilePrefab);
                        Vector3 brickBoxTilePosition = new Vector3(tileSpawnAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                   MapSetting.AboveGroundHeight,
                                                                   tileSpawnAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        brickBoxTileGameObject.transform.SetParent(transform);
                        brickBoxTileGameObject.transform.position = brickBoxTilePosition;
                        MapManager.UpdateMap(tileSpawnAnchor, mapSlotType);
                        NetworkServer.Spawn(brickBoxTileGameObject, _brickBoxTilePrefab.GetComponent<NetworkIdentity>().assetId);
                        break;

                    case MapSlotType.ChestBox:
                        GameObject chestBoxTileGameObject = Instantiate(GetChestTilePrefab(1));
                        Vector3 chestBoxTilePosition = new Vector3(tileSpawnAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                   MapSetting.AboveGroundHeight,
                                                                   tileSpawnAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        chestBoxTileGameObject.transform.SetParent(transform);
                        chestBoxTileGameObject.transform.position = chestBoxTilePosition;
                        MapManager.UpdateMap(tileSpawnAnchor, mapSlotType);
                        NetworkServer.Spawn(chestBoxTileGameObject, GetChestTilePrefab(1).GetComponent<NetworkIdentity>().assetId);
                        break;
                }
            }
        }

        protected virtual void OnExposeHiddenItem(Vector3 exposedPosition)
        {
            if (UnityRandom.Range(0, _spawnChestBrickBoxTotal) < _spawnChestHiddenChestTotal)
            {
                _spawnChestHiddenChestTotal--;
                GameObject chestBoxTileGameObject = Instantiate(GetChestTilePrefab(1));
                chestBoxTileGameObject.transform.SetParent(transform);
                chestBoxTileGameObject.transform.position = exposedPosition;
                MapManager.UpdateMap(exposedPosition, MapSlotType.ChestBox);
                NetworkServer.Spawn(chestBoxTileGameObject, GetChestTilePrefab(1).GetComponent<NetworkIdentity>().assetId);
            }
            else
            {
                if (UnityRandom.Range(0, _spawnBoosterBrickBoxTotal) < _spawnHiddenBoosterTotal)
                {
                    _spawnHiddenBoosterTotal--;
                    int randomIndex = UnityRandom.Range(0, _boosterSpawnTypes.Count);
                    GameObject spawnBoosterPrefab = _boostersData[_boosterSpawnTypes[randomIndex]].boosterPrefab;
                    _boosterSpawnTypes.RemoveAt(randomIndex);
                    GameObject boosterGameObject = Instantiate(spawnBoosterPrefab);
                    boosterGameObject.transform.position = exposedPosition;
                    NetworkServer.Spawn(boosterGameObject, spawnBoosterPrefab.GetComponent<NetworkIdentity>().assetId);
                }

                _spawnBoosterBrickBoxTotal--;
            }

            _spawnChestBrickBoxTotal--;
        }

        protected virtual GameObject GetChestTilePrefab(int level)
        {
            switch (level)
            {
                default:
                case 1:
                    return _woodenChestTilePrefab;
                case 2:
                    return _bronzeChestTilePrefab;
                case 3:
                    return _silverChestTilePrefab;
                case 4:
                    return _goldenChestTilePrefab;
                case 5:
                    return _diamondChestTilePrefab;
            }
        }

        #endregion Class Methods

        #region Owner Structs

        [Serializable]
        protected struct BoosterData
        {
            #region Members

            public int maxNumber;
            public GameObject boosterPrefab;

            #endregion Members
        }

        #endregion Owner Structs
    }
}