using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ZB.Model;
using ZB.Server;

namespace ZB.Gameplay
{
    [Serializable]
    public struct ZodiacSign
    {
        public CharacterType characterType;
        public Sprite signSprite;
    }

    public class DataManager : Singleton<DataManager>
    {
        #region Members

        [SerializeField]
        private AIGroupStatesConfig _autoPlayHeroAIGroupStatesConfig;
        [SerializeField]
        private AIGroupStatesConfig[] _enemyAIGroupStatesConfigs;
        [SerializeField]
        private ZodiacSign[] _zodiacSigns;

        #endregion Members

        #region Properties

        public static InventoryItem ControlledCharacterData { get; private set; }
        public static List<InventoryItem> AutoPlayCharactersData { get; private set; }

        #endregion Properties

        #region API Methods

        private void OnValidate()
        {
            Array.Resize(ref _zodiacSigns, (int)CharacterType.Count);
        }

        #endregion API Methods

        #region Class Methods

        public static void SetControlledCharacterData(InventoryItem controlledCharacterData)
        {
            ControlledCharacterData = controlledCharacterData;
        }

        public static void SetAutoPlayCharactersData(List<InventoryItem> autoPlayCharactersData)
        {
            AutoPlayCharactersData = autoPlayCharactersData;
        }

        public virtual List<AIState> GetAutoPlayHeroAIStates()
        {
            List<AIState> sampleAutoPlayHeroAIStates = _autoPlayHeroAIGroupStatesConfig.GetStates();
            List<AIState> clonedAutoPlayHeroAIStates = sampleAutoPlayHeroAIStates.Select(x => x.Clone()).ToList();
            return clonedAutoPlayHeroAIStates;
        }

        public virtual List<AIState> GetEnemyAIStates(int level)
        {
            List<AIState> sampleEnemyAIStates = _enemyAIGroupStatesConfigs[level - 1].GetStates();
            List<AIState> clonedEnemyAIStates = sampleEnemyAIStates.Select(x => x.Clone()).ToList();
            return clonedEnemyAIStates;
        }

        public virtual Sprite GetZodiacSign(CharacterType characterType)
        {
            ZodiacSign zodiacSign = _zodiacSigns.First(x => x.characterType == characterType);
            return zodiacSign.signSprite;
        }

        public virtual EnemyStat GetEnemyData(int level)
        {
            return ServerConfigData.GetEnemyStat(level);
        }

        public virtual ChestStat GetChestData(int level)
        {
            return ServerConfigData.GetChestStat(level);
        }

        public virtual InventoryItem GetBombDefaultData(string rarity)
        {
            return ServerConfigData.GetBombDefault(rarity);
        }

        public virtual BigBombChestStat GetBigBombChestStat()
        {
            return ServerConfigData.GetBigBombChestStat();
        }

        #endregion Class Methods
    }
}