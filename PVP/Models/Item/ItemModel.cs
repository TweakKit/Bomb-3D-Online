using System;
using System.Text;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public abstract class ItemModel
    {
        #region Members

        public string name;
        public string description;
        public float duration;
        public float cooldown;
        public int usageTimes;
        public Sprite itemAvatar;

        [NonSerialized]
        public float currentDuration;
        [NonSerialized]
        public float currentCooldown;
        [NonSerialized]
        public bool isPlaying;
        [NonSerialized]
        public NetworkedPlayer Owner;

        #endregion Members

        #region Properties

        public float Duration { get { return duration; } }
        public float Cooldown { get { return cooldown; } }
        public float CurrentDuration { get { return currentDuration; } }
        public float CurrentCooldown { get { return currentCooldown; } }
        public bool IsCooldown { get { return currentCooldown > 0; } }

        #endregion Properties

        #region Class Methods

        public ItemModel(ItemModel other)
        {
            name = other.name;
            description = other.description;
            duration = other.duration;
            cooldown = other.cooldown;
            usageTimes = other.usageTimes;
            itemAvatar = other.itemAvatar;
            Owner = other.Owner;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat($"\n<b>{name}</b> \n{description}");
            return stringBuilder.ToString();
        }

        public static implicit operator bool(ItemModel itemModel) => itemModel != null;

#if UNITY_EDITOR
        public virtual void Validate() { }
#endif

        public abstract ItemModel Clone();
        public abstract void Operate();

        #endregion Class Methods
    }
}