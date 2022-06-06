using UnityEngine;

public static class Constants
{
    #region Members

    public const int GiveUpFindPathToChestTimes = 3;
    public const int MaxTextureColors = 6;
    public const int InitialRandomTextureColors = 4;
    public const int threeStarsRewardTimeRate = 50;
    public const int twoStarsRewardTimeRate = 75;
    public const int oneStarsRewardTimeRate = 100;
    public const int MainMenuSceneAAValue = 8;
    public const int GameplaySceneAAValue = 2;
    public const int HeroLayerIndex = 6;
    public const int EnemyLayerIndex = 7;
    public const int BreakableLayerIndex = 8;
    public const int BombLayerIndex = 9;
    public const int ControlledHeroLayerIndex = 10;
    public const int PlayerLayerIndex = 12;
    public const int BoosterLayerIndex = 13;
    public static readonly LayerMask BombHitLayerMask = 1 << HeroLayerIndex | 1 << ControlledHeroLayerIndex | 1 << EnemyLayerIndex | 1 << BreakableLayerIndex | 1 << Constants.BombLayerIndex;
    public static readonly LayerMask NetworkedBombHitLayerMask = 1 << PlayerLayerIndex | 1 << BreakableLayerIndex | 1 << Constants.BombLayerIndex;
    public static readonly LayerMask BoosterLayerMask = 1 << BoosterLayerIndex;
    public const string LobbySceneName = "Lobby";
    public const string MatchSceneName = "Match";
    public const string MainMenuSceneName = "MainMenu";
    public const string GamePlaySceneName = "Gameplay";
    public const string GamePlayDebugSceneName = "GamePlayDebug";
    public const string EntityVisualName = "Visual";
    public const string CharacterShadowName = "Shadow";
    public const string CharacterInterpolationRootName = "InterpolationRoot";
    public const string BombTag = "Bomb";
    public const float CharacterBoundRadius = 0.8f;
    public const float CharacterWithPetBoundRadius = 1.0f;
    public const int CharacterSpawnSeparationDistanceTiles = 2;
    public const float NetworkConnectionTimeOut = 10.0f;
    public const int MaxLevelMapDetail = 10;

    #endregion Members
}