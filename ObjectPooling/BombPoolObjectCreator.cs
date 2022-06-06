using System;
using UnityEngine;

namespace ZB.Gameplay
{
    public class BombPoolObjectCreator : PoolObjectCreator
    {
        #region Members

        [SerializeField]
        private BombType _bombType;

        #endregion Members

        #region Properties

        public override Enum ObjectType => _bombType;

        #endregion Properties
    }
}