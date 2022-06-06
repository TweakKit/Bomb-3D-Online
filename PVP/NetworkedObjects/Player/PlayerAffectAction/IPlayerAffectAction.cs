namespace ZB.Gameplay.PVP
{
    public interface IPlayerAffectAction
    {
        #region Properties

        bool HasFinished { get; set; }

        #endregion Properties

        #region Interface Methods

        /// <summary>
        /// Apply the affect action to the owner.
        /// </summary>
        /// <param name="owner">The owner of this affect action.</param>
        void Apply(NetworkedPlayer owner);

        /// <summary>
        /// Update the affect action every frame. 
        /// An affect action is an action that makes the owner do something. In this function we can
        /// make the owner do whatever we want, such as: Move fast in 5 seconds, or drop traps around the map in 10 seconds,...
        /// </summary>
        void Update();

        /// <summary>
        /// Stop the affect action to make the owner no longer run the action anymore.
        /// </summary>
        void Stop();

        #endregion Interface Methods
    }
}