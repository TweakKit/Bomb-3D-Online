using System;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// Every pool object has this component attached.
    /// </summary>
    public class PoolObjectElement : MonoBehaviour
    {
        #region Properties

        public Enum ObjectType { get; set; }

        #endregion Properties
    }
}