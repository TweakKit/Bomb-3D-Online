using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity, IEntityGetHit
    {
        #region Members

        [HideInInspector]
        public new Collider collider;

        #endregion Members

        #region Class Methods

        public virtual void RegisterGetHit()
        {
            collider = gameObject.GetComponent<Collider>();
            collider.enabled = true;
            maxHP = currentHP;
        }

        [Server]
        public virtual void GetHit(float damageValue)
        {
            if (!isDead)
            {
                currentHP -= damageValue * (1 - defense);
                GetDamagedEvent.Invoke();

                if (currentHP <= 0)
                {
                    isDead = true;
                    collider.enabled = false;
                    DieEvent.Invoke();
                }
            }
        }

        #endregion Class Methods
    }
}