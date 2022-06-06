using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZB.Model;
using UnityRandom = UnityEngine.Random;

namespace ZB.Gameplay
{
    public class LevelLoader : MonoBehaviour
    {
        #region Members

        protected const int definedMoveableTile1ID = 0;
        protected const int definedMoveableTile2ID = 3;
        protected const int definedMoveableTile3ID = 4;
        protected const int definedMoveableTile4ID = 5;
        protected const int definedWallTileID = 1;
        protected const int definedBlockTileID = 2;

        protected static string mapLayoutDataString = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1;1,0,0,4,4,3,0,4,5,0,3,4,4,4,0,0,1;1,4,2,5,2,0,3,4,2,3,3,5,2,5,2,4,1;1,5,4,5,2,5,2,0,5,0,2,0,2,4,5,4,1;1,0,0,4,5,4,3,0,4,0,0,3,0,4,4,5,1;1,0,2,0,2,4,4,0,0,0,4,0,2,3,2,4,1;1,4,0,3,0,4,2,5,2,0,2,4,0,0,0,0,1;1,4,5,0,0,5,4,0,3,5,4,4,3,0,3,3,1;1,4,2,5,2,0,4,2,3,2,5,4,2,4,2,0,1;1,5,5,4,0,0,3,0,4,5,4,5,5,5,0,0,1;1,5,4,0,4,2,2,0,5,4,2,2,5,0,3,3,1;1,5,0,5,5,0,3,4,2,4,4,5,0,3,0,5,1;1,4,3,2,5,3,2,5,4,5,2,0,3,2,5,5,1;1,0,0,5,0,0,4,5,4,5,5,0,3,5,0,5,1;1,5,3,2,3,0,5,4,2,5,0,3,0,2,3,0,1;1,4,5,3,0,0,4,4,5,0,3,3,5,5,0,3,1;1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1";
        protected static string mapDetailDataString = "1,1,1,0.10,10,300,15,15,225,64,0.15,34,0.15,34,0.20,45,0.05,2,110,37.50,50,0.40,0.60,20,22,1,0,0,0,0,2,0,0,0,0,1,0,0,0,0,20,0,0,0,0,30,6,1,0,0,0,0,6,0,0,0,0,200,60,0.30";

        [Header("Tiles mesh combined holders")]
        [SerializeField]
        protected Transform _moveableTiles1MeshCombinedHolder;
        [SerializeField]
        protected Transform _moveableTiles2MeshCombinedHolder;
        [SerializeField]
        protected Transform _moveableTiles3MeshCombinedHolder;
        [SerializeField]
        protected Transform _moveableTiles4MeshCombinedHolder;
        [SerializeField]
        protected Transform _wallTilesMeshCombinedHolder;
        [SerializeField]
        protected Transform _blockTilesMeshCombinedHolder;

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
        [SerializeField]
        protected GameObject _bigBoomChestTilePrefab;

        protected MapDetailData _loadedMapDetailData;
        protected int _spawnChestBrickBoxTotal;
        protected int _spawnChestHiddenChestTotal;

        #endregion Members

        #region Properties

        public static bool HasBigBoomSecret { get; protected set; } = false;

#if UNITY_EDITOR
        protected static bool HasLoadedData { get; set; } = false;
#endif

        #endregion Properties

        #region API Methods

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (!HasLoadedData)
            {
                SceneManager.LoadScene(Constants.MainMenuSceneName);
                return;
            }
#endif
            EventManager.AddListener<Vector3>(GameEventType.ExposeHiddenItem, OnExposeHiddenItem);
        }

        protected virtual void Start()
        {
            CreateMap();
            CreateCharacters();
            CompleteLoadingLevel();
        }

        #endregion API Methods

        #region Class Methods

        public static void SetMapLayoutData(string mapLayoutData)
        {
            Debug.Log("mapLayoutDataString: " + mapLayoutData);
            mapLayoutDataString = mapLayoutData;

#if UNITY_EDITOR
            HasLoadedData = true;
#endif
        }

        public static void SetMapDetailData(string mapDetailData)
        {
            Debug.Log("mapDetailData: " + mapDetailData);
            mapDetailDataString = mapDetailData;
        }

        protected virtual void CreateMap()
        {
            List<MapAnchor> blockAnchors;
            CreateBaseTiles(out blockAnchors);
            CreateDetailTiles(blockAnchors);
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
                            wallTileGameObject.transform.SetParent(_wallTilesMeshCombinedHolder);
                            wallTileGameObject.transform.position = wallTilePosition;
                            mapSlots[i, j] = new MapSlot(MapSlotType.Block);
                            break;

                        case definedMoveableTile1ID:
                        case definedMoveableTile2ID:
                        case definedMoveableTile3ID:
                        case definedMoveableTile4ID:
                            GameObject moveableTileGameObject;
                            Vector3 moveableTilePosition = new Vector3(j * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                       MapSetting.OnGroundHeight,
                                                                       i * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                            switch (mapTileID)
                            {
                                case definedMoveableTile1ID:
                                    moveableTileGameObject = Instantiate(_moveableTile1Prefab);
                                    moveableTileGameObject.transform.SetParent(_moveableTiles1MeshCombinedHolder);
                                    moveableTileGameObject.transform.position = moveableTilePosition;
                                    break;
                                case definedMoveableTile2ID:
                                    moveableTileGameObject = Instantiate(_moveableTile2Prefab);
                                    moveableTileGameObject.transform.SetParent(_moveableTiles2MeshCombinedHolder);
                                    moveableTileGameObject.transform.position = moveableTilePosition;
                                    break;
                                case definedMoveableTile3ID:
                                    moveableTileGameObject = Instantiate(_moveableTile3Prefab);
                                    moveableTileGameObject.transform.SetParent(_moveableTiles3MeshCombinedHolder);
                                    moveableTileGameObject.transform.position = moveableTilePosition;
                                    break;
                                case definedMoveableTile4ID:
                                    moveableTileGameObject = Instantiate(_moveableTile4Prefab);
                                    moveableTileGameObject.transform.SetParent(_moveableTiles4MeshCombinedHolder);
                                    moveableTileGameObject.transform.position = moveableTilePosition;
                                    break;
                            }
                            mapSlots[i, j] = new MapSlot(MapSlotType.Empty);
                            break;

                        case definedBlockTileID:
                            GameObject blockTileGameObject = Instantiate(_blockTilePrefab);
                            Vector3 blockTilePosition = new Vector3(j * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                    MapSetting.AboveGroundHeight,
                                                                    i * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                            blockTileGameObject.transform.SetParent(_blockTilesMeshCombinedHolder);
                            blockTileGameObject.transform.position = blockTilePosition;
                            mapSlots[i, j] = new MapSlot(MapSlotType.Block);
                            blockAnchors.Add(new MapAnchor(i, j));
                            break;
                    }
                }
            }

            _moveableTiles1MeshCombinedHolder.GetComponent<MeshCombiner>().CombineMeshes();
            _moveableTiles2MeshCombinedHolder.GetComponent<MeshCombiner>().CombineMeshes();
            _moveableTiles3MeshCombinedHolder.GetComponent<MeshCombiner>().CombineMeshes();
            _moveableTiles4MeshCombinedHolder.GetComponent<MeshCombiner>().CombineMeshes();
            _wallTilesMeshCombinedHolder.GetComponent<MeshCombiner>().CombineMeshes();
            _blockTilesMeshCombinedHolder.GetComponent<MeshCombiner>().CombineMeshes();

            MapManager.Init(mapSlots);
        }

        protected virtual void CreateDetailTiles(List<MapAnchor> blockAnchors)
        {
            List<string> mapDetails = new List<string>(mapDetailDataString.Split(','));
            _loadedMapDetailData = new MapDetailData(mapDetails);
            _spawnChestBrickBoxTotal = _loadedMapDetailData.brickBoxTotal;
            _spawnChestHiddenChestTotal = _loadedMapDetailData.hiddenChestTotal;

            int spawnTilesTotal = _loadedMapDetailData.brickDecorTotal + _loadedMapDetailData.brickBoxTotal + _loadedMapDetailData.chestTotal - _loadedMapDetailData.hiddenChestTotal;
            for (int i = 0; i < spawnTilesTotal; i++)
            {
                MapSlotType mapSlotType = _loadedMapDetailData.GetSpawnMapSlotType();
                MapAnchor randomBlockAnchor = blockAnchors.GetRandomFromList();
                MapAnchor tileSpawnAnchor = MapManager.GetAnchorAroundBlockAnchor(randomBlockAnchor);
                while (tileSpawnAnchor == MapAnchor.illegalAnchor)
                {
                    blockAnchors.Remove(randomBlockAnchor);
                    randomBlockAnchor = blockAnchors.GetRandomFromList();
                    tileSpawnAnchor = MapManager.GetAnchorAroundBlockAnchor(randomBlockAnchor);
                }

                switch (mapSlotType)
                {
                    case MapSlotType.BrickDecor:
                        int randomBrickDecorType = UnityRandom.Range(1, 3);
                        MapItemModel brickDecorModel = GetBrickModel(randomBrickDecorType == 1 ? MapItemType.BrickDecor1 : MapItemType.BrickDecor2);
                        GameObject brickDecorTileGameObject = Instantiate(randomBrickDecorType == 1 ? _brickDecorTile1Prefab : _brickDecorTile2Prefab);
                        Vector3 brickDecorTilePosition = new Vector3(tileSpawnAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                     MapSetting.AboveGroundHeight,
                                                                     tileSpawnAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        brickDecorTileGameObject.transform.SetParent(transform);
                        brickDecorTileGameObject.GetComponent<MapItemInstanceComponent>().Build(brickDecorModel, brickDecorTilePosition);
                        MapManager.UpdateMap(tileSpawnAnchor, mapSlotType);
                        break;

                    case MapSlotType.BrickBox:
                        MapItemModel brickBoxModel = GetBrickModel(MapItemType.BrickBox);
                        GameObject brickBoxTileGameObject = Instantiate(_brickBoxTilePrefab);
                        Vector3 brickBoxTilePosition = new Vector3(tileSpawnAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                   MapSetting.AboveGroundHeight,
                                                                   tileSpawnAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        brickBoxTileGameObject.transform.SetParent(transform);
                        brickBoxTileGameObject.GetComponent<MapItemInstanceComponent>().Build(brickBoxModel, brickBoxTilePosition);
                        MapManager.UpdateMap(tileSpawnAnchor, mapSlotType);
                        break;

                    case MapSlotType.ChestBox:
                        MapItemModel chestBoxModel = GetChestModel(MapItemType.ChestBox, _loadedMapDetailData.levelChestBaseToken);
                        GameObject chestBoxTileGameObject = Instantiate(GetChestTilePrefab(chestBoxModel.Level));
                        Vector3 chestBoxTilePosition = new Vector3(tileSpawnAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                   MapSetting.AboveGroundHeight,
                                                                   tileSpawnAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                        chestBoxTileGameObject.transform.SetParent(transform);
                        chestBoxTileGameObject.GetComponent<MapItemInstanceComponent>().Build(chestBoxModel, chestBoxTilePosition);
                        MapManager.UpdateMap(tileSpawnAnchor, mapSlotType);
                        EventManager.Invoke<Vector3>(GameEventType.SpawnChest, chestBoxTilePosition);
                        break;
                }
            }

            HasBigBoomSecret = UnityRandom.Range(0.0f, 1.0f) <= _loadedMapDetailData.bigBoomChestRate;
            if (HasBigBoomSecret)
            {
                MapAnchor randomBlockAnchor = blockAnchors.GetRandomFromList();
                MapAnchor tileSpawnAnchor = MapManager.GetAnchorAroundBlockAnchor(randomBlockAnchor);
                while (tileSpawnAnchor == MapAnchor.illegalAnchor)
                {
                    blockAnchors.Remove(randomBlockAnchor);
                    randomBlockAnchor = blockAnchors.GetRandomFromList();
                    tileSpawnAnchor = MapManager.GetAnchorAroundBlockAnchor(randomBlockAnchor);
                }

                MapSlotType mapSlotType = MapSlotType.ChestBox;

                MapItemModel bigBoomChestBoxModel = GetBigBoomChestModel(MapItemType.ChestBox);
                GameObject bigBoomChestBoxTileGameObject = Instantiate(_bigBoomChestTilePrefab);
                Vector3 bigBoomChestBoxTilePosition = new Vector3(tileSpawnAnchor.x * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize,
                                                                  MapSetting.AboveGroundHeight,
                                                                  tileSpawnAnchor.y * MapSetting.MapSquareSize + MapSetting.MapSquareHalfSize);
                bigBoomChestBoxTileGameObject.transform.SetParent(transform);
                bigBoomChestBoxTileGameObject.GetComponent<MapItemInstanceComponent>().Build(bigBoomChestBoxModel, bigBoomChestBoxTilePosition);
                MapManager.UpdateMap(tileSpawnAnchor, mapSlotType);
                EventManager.Invoke<Vector3>(GameEventType.SpawnChest, bigBoomChestBoxTilePosition);
            }
        }

        protected virtual void CreateCharacters()
        {
            List<MapAnchor> characterOccupiedSlots = new List<MapAnchor>();
            GameObjectsSpawner.Instance.CreateHeroes(ref characterOccupiedSlots);

            for (int i = 0; i < _loadedMapDetailData.enemyTotalDetails1; i++)
                GameObjectsSpawner.Instance.CreateEnemy(1, _loadedMapDetailData.levelEnemyBaseToken, ref characterOccupiedSlots);

            for (int i = 0; i < _loadedMapDetailData.enemyTotalDetails2; i++)
                GameObjectsSpawner.Instance.CreateEnemy(2, _loadedMapDetailData.levelEnemyBaseToken, ref characterOccupiedSlots);

            for (int i = 0; i < _loadedMapDetailData.enemyTotalDetails3; i++)
                GameObjectsSpawner.Instance.CreateEnemy(3, _loadedMapDetailData.levelEnemyBaseToken, ref characterOccupiedSlots);

            for (int i = 0; i < _loadedMapDetailData.enemyTotalDetails4; i++)
                GameObjectsSpawner.Instance.CreateEnemy(4, _loadedMapDetailData.levelEnemyBaseToken, ref characterOccupiedSlots);

            for (int i = 0; i < _loadedMapDetailData.enemyTotalDetails5; i++)
                GameObjectsSpawner.Instance.CreateEnemy(5, _loadedMapDetailData.levelEnemyBaseToken, ref characterOccupiedSlots);
        }

        protected virtual void CompleteLoadingLevel()
        {
            GameplayManager.Instance.SetLevelPlayTime(_loadedMapDetailData.timePlay);
            EventManager.Invoke(GameEventType.LoadedLevel);
        }

        protected virtual MapItemModel GetBrickModel(MapItemType brickType)
        {
            MapItemModel brickModel = new BrickModel(brickType.ToString(), brickType.ToString(), brickType, 1, 1);
            return brickModel;
        }

        protected virtual MapItemModel GetChestModel(MapItemType brickType, float baseToken)
        {
            int chestLevel = _loadedMapDetailData.GetChestLevel();
            ChestStat chestStat = DataManager.Instance.GetChestData(chestLevel);
            MapItemModel chestModel = new ChestModel(chestStat, brickType, baseToken);
            return chestModel;
        }

        protected virtual MapItemModel GetBigBoomChestModel(MapItemType brickType)
        {
            BigBombChestStat bigBombChestStat = DataManager.Instance.GetBigBombChestStat();
            MapItemModel bigBoomChestModel = new BigBoomChestModel(bigBombChestStat, brickType);
            return bigBoomChestModel;
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

        protected virtual void OnExposeHiddenItem(Vector3 exposedPosition)
        {
            if (UnityRandom.Range(0, _spawnChestBrickBoxTotal) < _spawnChestHiddenChestTotal)
            {
                _spawnChestHiddenChestTotal--;
                MapItemModel chestBoxModel = GetChestModel(MapItemType.ChestBox, _loadedMapDetailData.levelChestBaseToken);
                GameObject chestBoxTileGameObject = Instantiate(GetChestTilePrefab(chestBoxModel.Level));
                chestBoxTileGameObject.transform.SetParent(transform);
                chestBoxTileGameObject.GetComponent<MapItemInstanceComponent>().Build(chestBoxModel, exposedPosition);
                MapManager.UpdateMap(exposedPosition, MapSlotType.ChestBox);
                EventManager.Invoke<Vector3>(GameEventType.SpawnChest, exposedPosition);
            }

            _spawnChestBrickBoxTotal--;
        }

        #endregion Class Methods
    }

    public struct MapDetailData
    {
        #region Members

        private enum MapDetailField
        {
            MapIndexField = 0,
            LevelIndexField,
            ElementalIndexField,
            ElementalBuffField,
            LevelEnergyField,
            TimePlayField,
            MapWidthField,
            MapHeightField,
            MapAreaField,
            WallTotalField,
            BlockRateField,
            BlockTotalField,
            BrickDecorRateField,
            BrickDecorTotalField,
            BrickBoxRateField,
            BrickBoxTotalField,
            HiddenChestRateField,
            HiddenChestTotalField,
            FreeTileField,
            MinTokenTotalField,
            MaxTokenTotalField,
            ChestTokenRateField,
            EnemyTokenRateField,
            ChestTokenField,
            ChestTotalField,
            HiddenChestRateDetails1Field,
            HiddenChestRateDetails2Field,
            HiddenChestRateDetails3Field,
            HiddenChestRateDetails4Field,
            HiddenChestRateDetails5Field,
            HiddenChestTotalDetails1Field,
            HiddenChestTotalDetails2Field,
            HiddenChestTotalDetails3Field,
            HiddenChestTotalDetails4Field,
            HiddenChestTotalDetails5Field,
            ChestRateDetails1Field,
            ChestRateDetails2Field,
            ChestRateDetails3Field,
            ChestRateDetails4Field,
            ChestRateDetails5Field,
            ChestTotalDetails1Field,
            ChestTotalDetails2Field,
            ChestTotalDetails3Field,
            ChestTotalDetails4Field,
            ChestTotalDetails5Field,
            EnemyTokenField,
            EnemyTotalField,
            EnemyRateDetails1Field,
            EnemyRateDetails2Field,
            EnemyRateDetails3Field,
            EnemyRateDetails4Field,
            EnemyRateDetails5Field,
            EnemyTotalDetails1Field,
            EnemyTotalDetails2Field,
            EnemyTotalDetails3Field,
            EnemyTotalDetails4Field,
            EnemyTotalDetails5Field,
            LevelTokenQuantity,
            LeveltokenResetFee,
            BigBoomChestRate,
        }

        public int mapIndex;
        public int levelIndex;
        public int elementalIndex;
        public double elementalBuff;
        public int levelEnergy;
        public double timePlay;
        public int mapWidth;
        public int mapHeight;
        public int mapArea;
        public int wallTotal;
        public double blockRate;
        public int blockTotal;
        public double brickDecorRate;
        public int brickDecorTotal;
        public double brickBoxRate;
        public int brickBoxTotal;
        public double hiddenChestRate;
        public int hiddenChestTotal;
        public int freeTile;
        public double minTokenTotal;
        public double maxTokenTotal;
        public double chestTokenRate;
        public double enemyTokenRate;
        public double chestToken;
        public int chestTotal;
        public double hiddenChestRateDetails1;
        public double hiddenChestRateDetails2;
        public double hiddenChestRateDetails3;
        public double hiddenChestRateDetails4;
        public double hiddenChestRateDetails5;
        public int hiddenChestTotalDetails1;
        public int hiddenChestTotalDetails2;
        public int hiddenChestTotalDetails3;
        public int hiddenChestTotalDetails4;
        public int hiddenChestTotalDetails5;
        public double chestRateDetails1;
        public double chestRateDetails2;
        public double chestRateDetails3;
        public double chestRateDetails4;
        public double chestRateDetails5;
        public int chestTotalDetails1;
        public int chestTotalDetails2;
        public int chestTotalDetails3;
        public int chestTotalDetails4;
        public int chestTotalDetails5;
        public double enemyToken;
        public int enemyTotal;
        public double enemyRateDetails1;
        public double enemyRateDetails2;
        public double enemyRateDetails3;
        public double enemyRateDetails4;
        public double enemyRateDetails5;
        public int enemyTotalDetails1;
        public int enemyTotalDetails2;
        public int enemyTotalDetails3;
        public int enemyTotalDetails4;
        public int enemyTotalDetails5;
        public double levelTokenQuantity;
        public double levelTokenResetFee;
        public double bigBoomChestRate;

        public float levelToken;
        public float levelChestToken;
        public float levelEnemyToken;
        public float levelChestBaseToken;
        public float levelEnemyBaseToken;
        private List<MapSlotType> _randomMapSlotTypes;
        private List<int> _chestLevels;

        #endregion Members

        #region Struct Methods

        public MapDetailData(List<string> mapDetails)
        {
            Dictionary<MapDetailField, object> mapDataDictionary = new Dictionary<MapDetailField, object>();
            int index = 0;

            foreach (MapDetailField mapDetailField in Enum.GetValues(typeof(MapDetailField)))
            {
                mapDataDictionary.Add(mapDetailField, mapDetails[index]);
                index++;
            }

            mapIndex = Convert.ToInt32(mapDataDictionary[MapDetailField.MapIndexField]);
            levelIndex = Convert.ToInt32(mapDataDictionary[MapDetailField.LevelIndexField]);
            elementalIndex = Convert.ToInt32(mapDataDictionary[MapDetailField.ElementalIndexField]);
            elementalBuff = Convert.ToDouble(mapDataDictionary[MapDetailField.ElementalBuffField]);
            levelEnergy = Convert.ToInt32(mapDataDictionary[MapDetailField.LevelEnergyField]);
            timePlay = Convert.ToDouble(mapDataDictionary[MapDetailField.TimePlayField]);
            mapWidth = Convert.ToInt32(mapDataDictionary[MapDetailField.MapWidthField]);
            mapHeight = Convert.ToInt32(mapDataDictionary[MapDetailField.MapHeightField]);
            mapArea = Convert.ToInt32(mapDataDictionary[MapDetailField.MapAreaField]);
            wallTotal = Convert.ToInt32(mapDataDictionary[MapDetailField.WallTotalField]);
            blockRate = Convert.ToDouble(mapDataDictionary[MapDetailField.BlockRateField]);
            blockTotal = Convert.ToInt32(mapDataDictionary[MapDetailField.BlockTotalField]);
            brickDecorRate = Convert.ToDouble(mapDataDictionary[MapDetailField.BrickDecorRateField]);
            brickDecorTotal = Convert.ToInt32(mapDataDictionary[MapDetailField.BrickDecorTotalField]);
            brickBoxRate = Convert.ToDouble(mapDataDictionary[MapDetailField.BrickBoxRateField]);
            brickBoxTotal = Convert.ToInt32(mapDataDictionary[MapDetailField.BrickBoxTotalField]);
            hiddenChestRate = Convert.ToDouble(mapDataDictionary[MapDetailField.HiddenChestRateField]);
            hiddenChestTotal = Convert.ToInt32(mapDataDictionary[MapDetailField.HiddenChestTotalField]);
            freeTile = Convert.ToInt32(mapDataDictionary[MapDetailField.FreeTileField]);
            minTokenTotal = Convert.ToDouble(mapDataDictionary[MapDetailField.MinTokenTotalField]);
            maxTokenTotal = Convert.ToDouble(mapDataDictionary[MapDetailField.MaxTokenTotalField]);
            chestTokenRate = Convert.ToDouble(mapDataDictionary[MapDetailField.ChestTokenRateField]);
            enemyTokenRate = Convert.ToDouble(mapDataDictionary[MapDetailField.EnemyTokenRateField]);
            chestToken = Convert.ToDouble(mapDataDictionary[MapDetailField.ChestTokenField]);
            chestTotal = Convert.ToInt32(mapDataDictionary[MapDetailField.ChestTotalField]);
            hiddenChestRateDetails1 = Convert.ToDouble(mapDataDictionary[MapDetailField.HiddenChestRateDetails1Field]);
            hiddenChestRateDetails2 = Convert.ToDouble(mapDataDictionary[MapDetailField.HiddenChestRateDetails2Field]);
            hiddenChestRateDetails3 = Convert.ToDouble(mapDataDictionary[MapDetailField.HiddenChestRateDetails3Field]);
            hiddenChestRateDetails4 = Convert.ToDouble(mapDataDictionary[MapDetailField.HiddenChestRateDetails4Field]);
            hiddenChestRateDetails5 = Convert.ToDouble(mapDataDictionary[MapDetailField.HiddenChestRateDetails5Field]);
            hiddenChestTotalDetails1 = Convert.ToInt32(mapDataDictionary[MapDetailField.HiddenChestTotalDetails1Field]);
            hiddenChestTotalDetails2 = Convert.ToInt32(mapDataDictionary[MapDetailField.HiddenChestTotalDetails2Field]);
            hiddenChestTotalDetails3 = Convert.ToInt32(mapDataDictionary[MapDetailField.HiddenChestTotalDetails3Field]);
            hiddenChestTotalDetails4 = Convert.ToInt32(mapDataDictionary[MapDetailField.HiddenChestTotalDetails4Field]);
            hiddenChestTotalDetails5 = Convert.ToInt32(mapDataDictionary[MapDetailField.HiddenChestTotalDetails5Field]);
            chestRateDetails1 = Convert.ToDouble(mapDataDictionary[MapDetailField.ChestRateDetails1Field]);
            chestRateDetails2 = Convert.ToDouble(mapDataDictionary[MapDetailField.ChestRateDetails2Field]);
            chestRateDetails3 = Convert.ToDouble(mapDataDictionary[MapDetailField.ChestRateDetails3Field]);
            chestRateDetails4 = Convert.ToDouble(mapDataDictionary[MapDetailField.ChestRateDetails4Field]);
            chestRateDetails5 = Convert.ToDouble(mapDataDictionary[MapDetailField.ChestRateDetails5Field]);
            chestTotalDetails1 = Convert.ToInt32(mapDataDictionary[MapDetailField.ChestTotalDetails1Field]);
            chestTotalDetails2 = Convert.ToInt32(mapDataDictionary[MapDetailField.ChestTotalDetails2Field]);
            chestTotalDetails3 = Convert.ToInt32(mapDataDictionary[MapDetailField.ChestTotalDetails3Field]);
            chestTotalDetails4 = Convert.ToInt32(mapDataDictionary[MapDetailField.ChestTotalDetails4Field]);
            chestTotalDetails5 = Convert.ToInt32(mapDataDictionary[MapDetailField.ChestTotalDetails5Field]);
            enemyToken = Convert.ToDouble(mapDataDictionary[MapDetailField.EnemyTokenField]);
            enemyTotal = Convert.ToInt32(mapDataDictionary[MapDetailField.EnemyTotalField]);
            enemyRateDetails1 = Convert.ToDouble(mapDataDictionary[MapDetailField.EnemyRateDetails1Field]);
            enemyRateDetails2 = Convert.ToDouble(mapDataDictionary[MapDetailField.EnemyRateDetails2Field]);
            enemyRateDetails3 = Convert.ToDouble(mapDataDictionary[MapDetailField.EnemyRateDetails3Field]);
            enemyRateDetails4 = Convert.ToDouble(mapDataDictionary[MapDetailField.EnemyRateDetails4Field]);
            enemyRateDetails5 = Convert.ToDouble(mapDataDictionary[MapDetailField.EnemyRateDetails5Field]);
            enemyTotalDetails1 = Convert.ToInt32(mapDataDictionary[MapDetailField.EnemyTotalDetails1Field]);
            enemyTotalDetails2 = Convert.ToInt32(mapDataDictionary[MapDetailField.EnemyTotalDetails2Field]);
            enemyTotalDetails3 = Convert.ToInt32(mapDataDictionary[MapDetailField.EnemyTotalDetails3Field]);
            enemyTotalDetails4 = Convert.ToInt32(mapDataDictionary[MapDetailField.EnemyTotalDetails4Field]);
            enemyTotalDetails5 = Convert.ToInt32(mapDataDictionary[MapDetailField.EnemyTotalDetails5Field]);
            enemyTotalDetails5 = Convert.ToInt32(mapDataDictionary[MapDetailField.EnemyTotalDetails5Field]);
            levelTokenQuantity = Convert.ToDouble(mapDataDictionary[MapDetailField.LevelTokenQuantity]);
            levelTokenResetFee = Convert.ToDouble(mapDataDictionary[MapDetailField.LeveltokenResetFee]);
            bigBoomChestRate = Convert.ToDouble(mapDataDictionary[MapDetailField.BigBoomChestRate]);

            if (hiddenChestTotal > brickBoxTotal)
                hiddenChestTotal = brickBoxTotal;

            _randomMapSlotTypes = new List<MapSlotType>();

            for (int i = 0; i < brickDecorTotal; i++)
                _randomMapSlotTypes.Add(MapSlotType.BrickDecor);
            for (int i = 0; i < brickBoxTotal; i++)
                _randomMapSlotTypes.Add(MapSlotType.BrickBox);
            for (int i = 0; i < chestTotal - hiddenChestTotal; i++)
                _randomMapSlotTypes.Add(MapSlotType.ChestBox);

            _randomMapSlotTypes.Shuffle();

            _chestLevels = new List<int>();
            for (int i = 0; i < hiddenChestTotalDetails1; i++)
                _chestLevels.Add(1);
            for (int i = 0; i < hiddenChestTotalDetails2; i++)
                _chestLevels.Add(2);
            for (int i = 0; i < hiddenChestTotalDetails3; i++)
                _chestLevels.Add(3);
            for (int i = 0; i < hiddenChestTotalDetails4; i++)
                _chestLevels.Add(4);
            for (int i = 0; i < hiddenChestTotalDetails5; i++)
                _chestLevels.Add(5);
            for (int i = 0; i < chestTotalDetails1; i++)
                _chestLevels.Add(1);
            for (int i = 0; i < chestTotalDetails2; i++)
                _chestLevels.Add(2);
            for (int i = 0; i < chestTotalDetails3; i++)
                _chestLevels.Add(3);
            for (int i = 0; i < chestTotalDetails4; i++)
                _chestLevels.Add(4);
            for (int i = 0; i < chestTotalDetails5; i++)
                _chestLevels.Add(5);
            _chestLevels.Shuffle();

            levelToken = UnityRandom.Range((float)minTokenTotal, (float)maxTokenTotal);

            int totalEnemyTokenRate = 0;
            for (int i = 0; i < enemyTotalDetails1; i++)
                totalEnemyTokenRate += DataManager.Instance.GetEnemyData(1).tokenRate;
            for (int i = 0; i < enemyTotalDetails2; i++)
                totalEnemyTokenRate += DataManager.Instance.GetEnemyData(2).tokenRate;
            for (int i = 0; i < enemyTotalDetails3; i++)
                totalEnemyTokenRate += DataManager.Instance.GetEnemyData(3).tokenRate;
            for (int i = 0; i < enemyTotalDetails4; i++)
                totalEnemyTokenRate += DataManager.Instance.GetEnemyData(4).tokenRate;
            for (int i = 0; i < enemyTotalDetails5; i++)
                totalEnemyTokenRate += DataManager.Instance.GetEnemyData(5).tokenRate;
            levelEnemyToken = (float)(levelToken * enemyTokenRate);
            levelEnemyBaseToken = levelEnemyToken / totalEnemyTokenRate;

            int totalChestTokenRate = 0;
            for (int i = 0; i < hiddenChestTotalDetails1; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(1).rate;
            for (int i = 0; i < hiddenChestTotalDetails2; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(2).rate;
            for (int i = 0; i < hiddenChestTotalDetails3; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(3).rate;
            for (int i = 0; i < hiddenChestTotalDetails4; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(4).rate;
            for (int i = 0; i < hiddenChestTotalDetails5; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(5).rate;
            for (int i = 0; i < chestTotalDetails1; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(1).rate;
            for (int i = 0; i < chestTotalDetails2; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(2).rate;
            for (int i = 0; i < chestTotalDetails3; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(3).rate;
            for (int i = 0; i < chestTotalDetails4; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(4).rate;
            for (int i = 0; i < chestTotalDetails5; i++)
                totalChestTokenRate += DataManager.Instance.GetChestData(5).rate;
            levelChestToken = (float)(levelToken * chestTokenRate);
            levelChestBaseToken = levelChestToken / totalChestTokenRate;
        }

        public MapSlotType GetSpawnMapSlotType()
        {
            int randomIndex = UnityRandom.Range(0, _randomMapSlotTypes.Count);
            MapSlotType randomMapSlotType = _randomMapSlotTypes[randomIndex];
            _randomMapSlotTypes.RemoveAt(randomIndex);
            return randomMapSlotType;
        }

        public int GetChestLevel()
        {
            int randomIndex = UnityRandom.Range(0, _chestLevels.Count);
            int ramdomChestLevel = _chestLevels[randomIndex];
            _chestLevels.RemoveAt(randomIndex);
            return ramdomChestLevel;
        }

        #endregion Struct Methods
    }
}