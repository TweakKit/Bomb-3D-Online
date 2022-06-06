using UnityEngine;

namespace ZB.Gameplay
{
    public class CameraManager : MonoBehaviour
    {
        #region Members

        private static readonly float holdTimeBeforeStart = 2.0f;
        private static readonly float zoomInToTargetSpeed = 3.0f;
        private static readonly float zoomToViewSpeed = 5.0f;

        [SerializeField]
        private Vector3 _minBoundOffset = new Vector3(3, 0, 3);
        [SerializeField]
        private Vector3 _maxBoundOffset = new Vector3(3, 0, 3);

        [SerializeField]
        [Tooltip("The offset between the camera and the target.")]
        private Vector3 _cameraOffset;

        [SerializeField]
        [Tooltip("The smooth time the camera reaches the target.")]
        [Min(0.1f)]
        private float _followSmoothTime = 0.3f;

        private Transform _target;
        private Vector3 _desiredPosition;
        private Vector3 _currentVelocity;
        private Vector3 _minBoundPosition;
        private Vector3 _maxBoundPosition;
        private Updater _updater;
        private float _currentHoldTime;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            _updater = new Updater();
            EventManager.AddListener(GameEventType.LoadedLevel, OnLoadedLevel);
            EventManager.AddListener(GameEventType.StartGame, OnStartGame);
            EventManager.AddListener(GameEventType.PauseGame, OnPauseGame);
            EventManager.AddListener(GameEventType.ContinueGame, OnContinueGame);
            EventManager.AddListener(GameEventType.AfterHeroDie, OnAfterHeroDie);
            EventManager.AddListener<HeroModel>(GameEventType.SelectAutoPlayHero, OnSelectAutoPlayHero);
            EventManager.AddListener<int>(GameEventType.ZoomToView, OnZoomToView);
        }

        private void LateUpdate()
        {
            _updater.Update();
        }

        #endregion API Methods

        #region Class Methods

        private void OnLoadedLevel()
        {
            int mapWidth = MapManager.Width;
            int mapHeight = MapManager.Height;
            Vector3 mapCenterPosition = new Vector3(mapWidth / 2 * MapSetting.MapSquareSize + (mapWidth % 2 != 0 ? MapSetting.MapSquareHalfSize : 0),
                                                    MapSetting.AboveGroundHeight,
                                                    mapHeight / 2 * MapSetting.MapSquareSize + (mapHeight % 2 != 0 ? MapSetting.MapSquareHalfSize : 0));
            float minX = (-mapWidth * MapSetting.MapSquareSize) / 2.0f + _minBoundOffset.x;
            float minZ = (-mapHeight * MapSetting.MapSquareSize) / 2.0f + _minBoundOffset.z;
            float maxX = (mapWidth * MapSetting.MapSquareSize) / 2.0f - _maxBoundOffset.x;
            float maxZ = (mapHeight * MapSetting.MapSquareSize) / 2.0f - _maxBoundOffset.z;

            if (minX > maxX)
            {
                float temp = minX;
                minX = maxX;
                maxX = temp;
            }

            if (minZ > maxZ)
            {
                float temp = minZ;
                minZ = maxZ;
                maxZ = temp;
            }

            _minBoundPosition = mapCenterPosition + new Vector3(minX, transform.position.y, minZ);
            _maxBoundPosition = mapCenterPosition + new Vector3(maxX, transform.position.y, maxZ);
            _currentHoldTime = 0.0f;
            _updater.Execute(HoldCamera);
            transform.position = mapCenterPosition - Camera.main.transform.forward * (mapWidth + mapHeight);
        }

        private void OnStartGame()
        {
            _updater.Execute(NormalFocusTarget);
        }

        private void OnPauseGame()
        {
            _updater.Pause();
        }

        private void OnContinueGame()
        {
            _updater.Continue();
        }

        private void OnAfterHeroDie()
        {
            SetTarget();
            if (_target == null)
                _updater.Stop();
        }

        private void OnZoomToView(int zoomLevel)
        {
            if (zoomLevel == 1)
                ZoomLevel1();
            else if (zoomLevel == 2)
                ZoomLevel2();
            else if (zoomLevel == 3)
                ZoomLevel3();
            else if (zoomLevel == 4)
                ZoomLevel4();
        }

        private void OnSelectAutoPlayHero(HeroModel autoPlayHero)
        {
            _target = autoPlayHero.ownerTransform;
        }

        private void HoldCamera()
        {
            _currentHoldTime += Time.deltaTime;
            if (_currentHoldTime >= holdTimeBeforeStart)
            {
                SetTarget();
                _updater.Execute(ZoomInToTarget);
            }
        }

        private void SetTarget()
        {
            if (EntitiesManager.Instance.Heroes.Count > 0)
            {
                var heroModel = EntitiesManager.Instance.Heroes[0];
                _target = heroModel.ownerTransform;
            }
            else _target = null;
        }

        private void ZoomInToTarget()
        {
            _desiredPosition = _target.position + _cameraOffset;
            _desiredPosition.x = Mathf.Clamp(_desiredPosition.x, _minBoundPosition.x, _maxBoundPosition.x);
            _desiredPosition.z = Mathf.Clamp(_desiredPosition.z, _minBoundPosition.z, _maxBoundPosition.z);
            transform.position = Vector3.Lerp(transform.position, _desiredPosition, zoomInToTargetSpeed * Time.deltaTime);

            if (Vector3.SqrMagnitude(_desiredPosition - transform.position) <= zoomInToTargetSpeed * Time.deltaTime * zoomInToTargetSpeed * Time.deltaTime)
            {
                EventManager.Invoke(GameEventType.ZoomToTarget);
                _updater.Stop();
            }
        }

        private void NormalFocusTarget()
        {
            _desiredPosition = _target.position + _cameraOffset;
            _desiredPosition.x = Mathf.Clamp(_desiredPosition.x, _minBoundPosition.x, _maxBoundPosition.x);
            _desiredPosition.z = Mathf.Clamp(_desiredPosition.z, _minBoundPosition.z, _maxBoundPosition.z);
            transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _currentVelocity, _followSmoothTime);
        }

        private void ZoomFocusTarget()
        {
            _desiredPosition = _target.position + _cameraOffset - Camera.main.transform.forward * (MapManager.Width + MapManager.Height) / 2;
            transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _currentVelocity, _followSmoothTime);
        }

        private void ZoomLevel1()
        {
            Vector3 mapCenterPosition = new Vector3(MapManager.Width / 2 * MapSetting.MapSquareSize + (MapManager.Width % 2 != 0 ? MapSetting.MapSquareHalfSize : 0),
                                                    MapSetting.AboveGroundHeight,
                                                    MapManager.Height / 2 * MapSetting.MapSquareSize + (MapManager.Height % 2 != 0 ? MapSetting.MapSquareHalfSize : 0));
            _desiredPosition = _target.position + _cameraOffset - Camera.main.transform.forward * (MapManager.Width + MapManager.Height) / 2;
            _updater.Execute(() => Zoom(1));
        }

        private void ZoomLevel2()
        {
            Vector3 mapCenterPosition = new Vector3(MapManager.Width / 2 * MapSetting.MapSquareSize + (MapManager.Width % 2 != 0 ? MapSetting.MapSquareHalfSize : 0),
                                                    MapSetting.AboveGroundHeight,
                                                    MapManager.Height / 2 * MapSetting.MapSquareSize + (MapManager.Height % 2 != 0 ? MapSetting.MapSquareHalfSize : 0));
            _desiredPosition = mapCenterPosition - Camera.main.transform.forward * (MapManager.Width + MapManager.Height);
            _updater.Execute(() => Zoom(2));
        }

        private void ZoomLevel3()
        {
            Vector3 mapCenterPosition = new Vector3(MapManager.Width / 2 * MapSetting.MapSquareSize + (MapManager.Width % 2 != 0 ? MapSetting.MapSquareHalfSize : 0),
                                                    MapSetting.AboveGroundHeight,
                                                    MapManager.Height / 2 * MapSetting.MapSquareSize + (MapManager.Height % 2 != 0 ? MapSetting.MapSquareHalfSize : 0));
            _desiredPosition = _target.position + _cameraOffset - Camera.main.transform.forward * (MapManager.Width + MapManager.Height) / 2;
            _updater.Execute(() => Zoom(3));
        }

        private void ZoomLevel4()
        {
            _desiredPosition = _target.position + _cameraOffset;
            _updater.Execute(() => Zoom(4));
        }

        private void Zoom(int zoomLevel)
        {
            transform.position = Vector3.Lerp(transform.position, _desiredPosition, zoomToViewSpeed * Time.deltaTime);
            if (Vector3.SqrMagnitude(_desiredPosition - transform.position) <= zoomToViewSpeed * Time.deltaTime * zoomToViewSpeed * Time.deltaTime)
            {
                if (zoomLevel == 1)
                    _updater.Execute(ZoomFocusTarget);
                else if (zoomLevel == 2)
                    _updater.Stop();
                else if (zoomLevel == 3)
                    _updater.Execute(ZoomFocusTarget);
                else if (zoomLevel == 4)
                    _updater.Execute(NormalFocusTarget);
            }
        }

        #endregion Class Methods
    }
}