public interface INetworkedPlayer
{
    #region Interface Methods

    /// <summary>
    /// Allow the player to receive input from the user.
    /// </summary>
    void RegisterInput();
    void UpdateInput();

    /// <summary>
    /// Allow the player to move.
    /// </summary>
    void RegisterMovement();
    void UpdateMovement();

    /// <summary>
    /// Allow the player to run affect actions.
    /// </summary>
    void RegisterAffectAction();
    void UpdateAffectAction();

    /// <summary>
    /// Allow the player to animate.
    /// </summary>
    void RegisterAnimation();

    /// <summary>
    /// Allow the player to attack.
    /// </summary>
    void RegisterAttack();

    /// <summary>
    /// Allow the player to get hit.
    /// </summary>
    void RegisterGetHit();

    /// <summary>
    /// Allow the player to have a HUD panel shown above their head.
    /// </summary>
    void RegisterHUD();

    /// <summary>
    /// Allow the player to die.
    /// </summary>
    void RegisterDie();

    /// <summary>
    /// Allow the player to give token after dying.
    /// </summary>
    void RegisterGiveToken();

    /// <summary>
    /// Allow the player to collect boosters.
    /// </summary>
    void RegisterCollectBooster();

    /// <summary>
    /// Allow the player to use items.
    /// </summary>
    void RegisterUseItem();

    #endregion Interface Methods
}