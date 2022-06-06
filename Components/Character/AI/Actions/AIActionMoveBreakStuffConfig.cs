using UnityEngine;

namespace ZB.Gameplay
{
    public class AIActionMoveBreakStuffConfig : AIActionConfig
    {
        #region Members

        [SerializeField]
        private AIActionMoveBreakStuff _AIActionMoveBreakStuff;

        #endregion Members

        #region Properties

        public override AIAction AIAction => _AIActionMoveBreakStuff;

        #endregion Properties
    }
}