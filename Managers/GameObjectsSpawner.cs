using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    [Serializable]
    public struct CharacterSpawnData
    {
        public CharacterType characterType;
        public GameObject characterPrefab;
        public GameObject characterWithPetPrefab;
    }

    public enum CharacterIndicatorType
    {
        HeroControlIndicator = 1,
        HeroAutoPlayIndicator = 2,
        EnemyIndicator = 3,
    }

    [Serializable]
    public struct CharacterIndicatorpawnData
    {
        public CharacterIndicatorType characterIndicatorType;
        public GameObject characterIndicatorPrefab;
    }

    public enum EntityHUDType
    {
        HeroHUD = 1,
        EnemyHUD = 2,
        MapItemHUD = 3,
    }

    [Serializable]
    public struct EntityHUDSpawnData
    {
        public EntityHUDType entityHUDType;
        public EntityHUD entityHUDPrefab;
    }

    /// <summary>
    /// This class is responsible for spawning game objects in the scene, such as characters, effects,...
    /// Note: Why pool is not used in this case? Because the spawned objects are used only once.
    /// </summary>
    public class GameObjectsSpawner : Singleton<GameObjectsSpawner>
    {
        #region Members

        [SerializeField]
        private CharacterSpawnData[] _charactersSpawnData;
        [SerializeField]
        private CharacterIndicatorpawnData[] _characterIndicatorsSpawnData;
        [SerializeField]
        private EntityHUDSpawnData[] _entityHUDSpawnData;

        #endregion Members

        #region Class Methods

        public virtual void CreateHeroes(ref List<MapAnchor> characterOccupiedSlots)
        {
            if (DataManager.ControlledCharacterData != null)
            {
                var characterData = DataManager.ControlledCharacterData;
                CharacterModel heroModel = new HeroControlModel(characterData);
                characterData.SetItems(CreateACharacterGameObject(heroModel, ref characterOccupiedSlots));
                EventManager.Invoke<CharacterModel>(GameEventType.SpawnHero, heroModel);
            }

            for (int i = 0; i < DataManager.AutoPlayCharactersData.Count; i++)
            {
                var characterData = DataManager.AutoPlayCharactersData[i];
                CharacterModel heroModel = new HeroModel(characterData);
                characterData.SetItems(CreateACharacterGameObject(heroModel, ref characterOccupiedSlots));
                EventManager.Invoke<CharacterModel>(GameEventType.SpawnHero, heroModel);
            }
        }

        public virtual void CreateEnemy(int level, float baseToken, ref List<MapAnchor> characterOccupiedSlots)
        {
            List<CharacterType> enemyTypes = new List<CharacterType>() { CharacterType.Aries, CharacterType.Capricorn, CharacterType.Scorpio, CharacterType.Cancer, CharacterType.Leo, CharacterType.Taurus };
            CharacterType enemyType = enemyTypes.GetRandomFromList();
            CharacterModel enemyModel = new EnemyModel(DataManager.Instance.GetEnemyData(level), enemyType, baseToken);
            GameObject obj = CreateACharacterGameObject(enemyModel, ref characterOccupiedSlots);
            // user empty character for set enemy color
            Model.InventoryItem characterEmpty = new Model.InventoryItem();
            characterEmpty.itemType = "Hero";
            characterEmpty.UpdateColor(obj);
            EventManager.Invoke<CharacterModel>(GameEventType.SpawnEnemy, enemyModel);
        }

        public virtual GameObject GetCharacterIndicator(CharacterIndicatorType characterIndicatorType)
        {
            GameObject indicatorGameObject = Instantiate(_characterIndicatorsSpawnData.First(x => x.characterIndicatorType == characterIndicatorType).characterIndicatorPrefab);
            return indicatorGameObject;
        }

        public virtual CharacterHUD GetCharacterHUD(EntityHUDType characterHUDType)
        {
            CharacterHUD characterHUD = GetEntityHUD(characterHUDType) as CharacterHUD;
            return characterHUD;
        }

        public virtual EntityHUD GetEntityHUD(EntityHUDType entityHUDType)
        {
            EntityHUD entityHUD = Instantiate(_entityHUDSpawnData.First(x => x.entityHUDType == entityHUDType).entityHUDPrefab);
            return entityHUD;
        }

        public virtual GameObject CreateACharacterGameObject(CharacterModel characterModel, ref List<MapAnchor> characterOccupiedSlots)
        {
            CharacterSpawnData characterSpawnData = _charactersSpawnData.First(x => x.characterType == characterModel.CharacterType);

            if (characterModel.IsHero)
            {
                HeroModel heroModel = characterModel as HeroModel;
                GameObject heroGameObject = null;

                if (heroModel.hasPet)
                    heroGameObject = Instantiate(characterSpawnData.characterWithPetPrefab);
                else
                    heroGameObject = Instantiate(characterSpawnData.characterPrefab);

                Vector3 heroPosition = MapManager.GetCharacterPosition(ref characterOccupiedSlots);
                heroGameObject.transform.SetParent(transform);
                heroGameObject.transform.position = heroPosition;
                AddComponentsForHeroGameObject(heroGameObject, heroModel.IsControlled);
                heroGameObject.GetComponent<CharacterInstanceComponent>().Build(heroModel, heroPosition);
                return heroGameObject;
            }
            else
            {
                EnemyModel enemyModel = characterModel as EnemyModel;
                GameObject enemyGameObject = Instantiate(characterSpawnData.characterPrefab);
                Vector3 enemyPosition = MapManager.GetCharacterPosition(ref characterOccupiedSlots);
                enemyGameObject.transform.SetParent(transform);
                enemyGameObject.transform.position = enemyPosition;
                AddComponentsForEnemyGameObject(enemyGameObject);
                enemyGameObject.GetComponent<CharacterInstanceComponent>().Build(enemyModel, enemyPosition);
                return enemyGameObject;
            }
        }

        protected virtual void AddComponentsForHeroGameObject(GameObject heroGameObject, bool isControlledHero)
        {
            heroGameObject.AddComponent(typeof(CharacterInstanceComponent));

            if (isControlledHero)
            {
                heroGameObject.AddComponent(typeof(CharacterControlInputComponent));
                heroGameObject.AddComponent(typeof(CharacterControlMovementComponent));

                GameObject interpolationRootGameObject = heroGameObject.GetChild(Constants.CharacterInterpolationRootName);
                interpolationRootGameObject.AddComponent(typeof(CharacterMovementInterpolation));
            }
            else
            {
                heroGameObject.AddComponent(typeof(CharacterAIBrainComponent));
                heroGameObject.AddComponent(typeof(CharacterAIMovementComponent));
            }

            heroGameObject.AddComponent(typeof(CharacterAnimationComponent));
            heroGameObject.AddComponent(typeof(CharacterIndicatorComponent));
            heroGameObject.AddComponent(typeof(CharacterAttackComponent));
            heroGameObject.AddComponent(typeof(CharacterGetHitComponent));
            heroGameObject.AddComponent(typeof(CharacterHUDComponent));
            heroGameObject.AddComponent(typeof(CharacterDieComponent));
            heroGameObject.AddComponent(typeof(CharacterCollideWithEnemyComponent));
        }

        protected virtual void AddComponentsForEnemyGameObject(GameObject enemyGameObject)
        {
            enemyGameObject.AddComponent(typeof(CharacterInstanceComponent));
            enemyGameObject.AddComponent(typeof(CharacterAIBrainComponent));
            enemyGameObject.AddComponent(typeof(CharacterAIMovementComponent));
            enemyGameObject.AddComponent(typeof(CharacterAnimationComponent));
            enemyGameObject.AddComponent(typeof(CharacterIndicatorComponent));
            enemyGameObject.AddComponent(typeof(CharacterAttackComponent));
            enemyGameObject.AddComponent(typeof(CharacterGetHitComponent));
            enemyGameObject.AddComponent(typeof(CharacterHUDComponent));
            enemyGameObject.AddComponent(typeof(CharacterDieComponent));
            enemyGameObject.AddComponent(typeof(CharacterGiveTokenComponent));
        }

        #endregion Class Methods
    }
}
