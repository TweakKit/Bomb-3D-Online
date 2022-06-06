using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Members

        protected static readonly Vector3 headOffset = new Vector3(0.0f, 2.5f, 0.0f);
        protected static readonly Vector3 petHeadOffset = new Vector3(0.0f, 3.25f, 0.0f);
        protected NetworkedPlayerHUD _playerHUD;

        #endregion Members

        #region Class Methods

        public virtual void RegisterHUD()
        {
            _playerHUD = gameObject.GetComponentInChildren<NetworkedPlayerHUD>();
            // DieEvent += OnDie_CallbackOnHUD;
        }

        protected virtual void OnDie_CallbackOnHUD()
        {
            Destroy(_playerHUD.gameObject);
            _playerHUD = null;
        }

        protected virtual void InitHUDOnAllClients()
        {
            _playerHUD = gameObject.GetComponentInChildren<NetworkedPlayerHUD>();
            _playerHUD.transform.localPosition = hasPet ? petHeadOffset : headOffset;
            _playerHUD.Init(name, 1);
        }

        protected virtual void OnCurrentHPChange(float oldValue, float newValue)
        {
            _playerHUD.UpdateHealthBar(currentHP, maxHP);
        }

        #endregion Class Methods
    }
}