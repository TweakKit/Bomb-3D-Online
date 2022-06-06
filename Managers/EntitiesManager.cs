using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    public class EntitiesManager : Singleton<EntitiesManager>
    {
        #region Properties

        public List<CharacterModel> Heroes { get; private set; } = new List<CharacterModel>();
        public List<CharacterModel> Enemies { get; private set; } = new List<CharacterModel>();
        public List<Vector3> ChestPoints { get; private set; } = new List<Vector3>();
        public bool HasControlHero { get; private set; } = false;
        public bool HasChest { get; private set; } = false;

        public List<CharacterModel> Characters
        {
            get
            {
                List<CharacterModel> characters = new List<CharacterModel>();
                characters.AddRange(Heroes);
                characters.AddRange(Enemies);
                return characters;
            }
        }

        #endregion Properties

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            EventManager.AddListener<CharacterModel>(GameEventType.SpawnHero, OnSpawnHero);
            EventManager.AddListener<CharacterModel>(GameEventType.HeroDie, OnHeroDie);
            EventManager.AddListener<CharacterModel>(GameEventType.SpawnEnemy, OnSpawnEnemy);
            EventManager.AddListener<CharacterModel>(GameEventType.EnemyDie, OnEnemyDie);
            EventManager.AddListener<Vector3>(GameEventType.SpawnChest, OnSpawnChest);
            EventManager.AddListener<Vector3>(GameEventType.DestroyChest, OnDestroyChest);
            EventManager.AddListener(GameEventType.StartGame, OnStartGame);
            EventManager.AddListener(GameEventType.WinGame, OnWinGame);
            EventManager.AddListener(GameEventType.LoseGame, OnLoseGame);
        }

        #endregion API Methods

        #region Class Methods

        private void OnSpawnHero(CharacterModel heroModel)
        {
            Heroes.Add(heroModel);

            if (heroModel.IsControlled)
                HasControlHero = true;
        }

        private void OnHeroDie(CharacterModel heroModel)
        {
            Heroes.Remove(heroModel);
            if (heroModel.IsControlled)
                HasControlHero = false;

            EventManager.Invoke(GameEventType.AfterHeroDie);

            if (Heroes.Count == 0)
                GameplayManager.Instance.LoseGame();
        }

        private void OnSpawnEnemy(CharacterModel enemyModel)
        {
            Enemies.Add(enemyModel);
        }

        private void OnEnemyDie(CharacterModel enemyModel)
        {
            Enemies.Remove(enemyModel);
            if (Enemies.Count == 0)
                GameplayManager.Instance.WinGame();
        }

        private void OnSpawnChest(Vector3 chestPoint)
        {
            ChestPoints.Add(chestPoint);
            HasChest = true;
        }

        private void OnDestroyChest(Vector3 chestPoint)
        {
            ChestPoints.Remove(chestPoint);
            HasChest = ChestPoints.Count > 0;
        }

        private void OnStartGame()
        {
            Heroes.ForEach(x => x.SetActivation(true));
            Enemies.ForEach(x => x.SetActivation(true));
            EventManager.AddListener(GameEventType.PauseGame, OnPauseGame);
            EventManager.AddListener(GameEventType.ContinueGame, OnContinueGame);
        }

        private void OnWinGame()
        {
            Heroes.ForEach(x => x.WinGame());
            Enemies.ForEach(x => x.LoseGame());
        }

        private void OnLoseGame()
        {
            Heroes.ForEach(x => x.LoseGame());
            Enemies.ForEach(x => x.WinGame());
        }

        private void OnPauseGame()
        {
            Heroes.ForEach(x => x.SetActivation(false));
            Enemies.ForEach(x => x.SetActivation(false));

            foreach (var bombGameObject in GameObject.FindGameObjectsWithTag(Constants.BombTag))
            {
                var bombExplodeComponent = bombGameObject.GetComponent<BombExplodeComponent>();
                bombExplodeComponent.CanExplode = false;
            }
        }

        private void OnContinueGame()
        {
            Heroes.ForEach(x => x.SetActivation(true));
            Enemies.ForEach(x => x.SetActivation(true));

            foreach (var bombGameObject in GameObject.FindGameObjectsWithTag(Constants.BombTag))
            {
                var bombExplodeComponent = bombGameObject.GetComponent<BombExplodeComponent>();
                bombExplodeComponent.CanExplode = true;
            }
        }

        #endregion Class Methods
    }
}