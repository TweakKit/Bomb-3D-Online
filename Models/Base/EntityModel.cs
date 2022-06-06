using System;
using UnityEngine;

namespace ZB.Gameplay
{
    public abstract class EntityModel
    {
        #region Members

        protected string _name;
        protected string _description;

        [NonSerialized]
        public Transform ownerTransform;
        [NonSerialized]
        public Transform visualTransform;

        #endregion Members

        #region Properties

        public string Name => _name;
        public string Description => _description;
        public abstract Enum Type { get; }

        public string Info
        {
            get
            {
                string info = _name + " - " + _description;
                return info;
            }
        }

        public Vector3 Position { get { return ownerTransform.position; } }
        public Quaternion Rotation { get { return visualTransform.rotation; } }
        public Vector3 Direction { get { return visualTransform.forward; } }

        #endregion Properties

        #region Class Methods

        public EntityModel(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public static implicit operator bool(EntityModel entityModel) => entityModel != null;

        #endregion Class Methods
    }
}