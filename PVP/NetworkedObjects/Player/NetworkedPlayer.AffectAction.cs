using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Members

        protected List<IPlayerAffectAction> _playerAffectActions;

        #endregion Members

        #region Class Methods

        public virtual void RegisterAffectAction()
        {
            _playerAffectActions = new List<IPlayerAffectAction>();
        }

        public virtual void ApplyAffectAction(IPlayerAffectAction playerAffectAction)
        {
            playerAffectAction.Apply(this);
            _playerAffectActions.Add(playerAffectAction);
        }

        public virtual void StopAffectAction(IPlayerAffectAction playerAffectAction)
        {
            playerAffectAction.Stop();
            _playerAffectActions.Remove(playerAffectAction);
        }

        public virtual void UpdateAffectAction()
        {
            for (int i = _playerAffectActions.Count - 1; i >= 0; i--)
            {
                _playerAffectActions[i].Update();
                if (_playerAffectActions[i].HasFinished)
                    _playerAffectActions.RemoveAt(i);
            }
        }

        public void AddBombDamageValue(float bombDamageValue)
        {
            _bombData.bonusDamageValue += bombDamageValue;
        }

        public void SubtractBombDamageValue(float bombDamageValue)
        {
            _bombData.bonusDamageValue -= bombDamageValue;
        }

        public void AddSpeedValue(float speedValue)
        {
            bonusMoveSpeedValue += speedValue;
        }

        public void SubtractSpeedValue(float speedValue)
        {
            bonusMoveSpeedValue -= speedValue;
        }

        public void AddSpeedPercent(float speedPercent)
        {
            bonusMoveSpeedPercent += speedPercent;
        }

        public void SubtractSpeedPercent(float speedPercent)
        {
            bonusMoveSpeedPercent -= speedPercent;
        }

        public void AddBombDamageRadius(int bombDamageRadius)
        {
            _bombData.bonusDamageRadius += bombDamageRadius;
        }

        public void SubtractBombDamageRadius(int bombDamageRadius)
        {
            _bombData.bonusDamageRadius -= bombDamageRadius;
        }

        public void AddBombNumber(int bombNumber)
        {
            bonusBombNumber += bombNumber;
        }

        public void SubtractBombNumber(int bombNumber)
        {
            bonusBombNumber -= bombNumber;
        }

        public void AddCurrentHP(float hp)
        {
            currentHP = Mathf.Clamp(currentHP += hp, 0, maxHP);
        }

        public void TriggerSelfDefend()
        {
            isSelfDefend = true;
        }

        public void ResetSelfDefend()
        {
            isSelfDefend = true;
        }

        #endregion Class Methods
    }
}