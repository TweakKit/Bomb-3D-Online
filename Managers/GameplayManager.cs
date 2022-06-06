using System.Collections;
using UnityEngine;
using ZB.Server;

namespace ZB.Gameplay
{
    public class GameplayManager : Singleton<GameplayManager>
    {
        #region Properties

        public int LevelPlayTime { get; private set; }
        public int Killed { get; private set; }
        public float RemainingLevelPlayTime { get; private set; }
        public float CurrentToken { get; private set; }
        public bool IsGameOver { get; private set; } = false;

        #endregion Properties

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            QualitySettings.antiAliasing = Constants.GameplaySceneAAValue;
        }

        #endregion API Methods

        #region Class Methods

        public void SetLevelPlayTime(double levelPlayTime)
        {
            LevelPlayTime = (int)levelPlayTime;
            RemainingLevelPlayTime = (float)levelPlayTime;
        }

        public void UpdateCurrentLevelPlayTime()
        {
            RemainingLevelPlayTime -= Time.deltaTime;
        }

        public void UpdateToken(float token)
        {
            CurrentToken += token;
        }

        public void UpdateKilled(int killed)
        {
            Killed = killed;
        }

        public void WinGame()
        {
            if (!IsGameOver)
            {
                IsGameOver = true;
                StartCoroutine(UpdateAfterWinGame());
                EventManager.Invoke(GameEventType.WinGame);
            }
        }

        public void LoseGame()
        {
            if (!IsGameOver)
            {
                IsGameOver = true;
                StartCoroutine(UpdateAfterLoseGame());
                EventManager.Invoke(GameEventType.LoseGame);
            }
        }

        private IEnumerator UpdateAfterWinGame()
        {
            yield return null;
            ServerApi.PlayerWinPVE(CurrentToken, LevelPlayTime - (int)RemainingLevelPlayTime, Killed);
        }

        private IEnumerator UpdateAfterLoseGame()
        {
            yield return null;
            ServerApi.PlayerLosePVE(true, LevelPlayTime - (int)RemainingLevelPlayTime);
        }

        #endregion Class Methods
    }
}