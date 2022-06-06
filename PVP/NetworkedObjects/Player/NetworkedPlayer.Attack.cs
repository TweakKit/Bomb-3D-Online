using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Members

        protected int _currentBombsPlaced;

        #endregion Members

        #region Class Methods

        public virtual void RegisterAttack()
        {
            _currentBombsPlaced = 0;
            ClickAttackEvent += OnClickAttack_CallbackOnAttack;
        }

        protected virtual void OnClickAttack_CallbackOnAttack()
        {
            CmdAttack(transform.position);
        }

        [Command]
        protected virtual void CmdAttack(Vector3 placeBombPosition)
        {
            // For prevent hacks.
            if (Vector3.SqrMagnitude(placeBombPosition - transform.position) > MapSetting.MapSquareSize)
                placeBombPosition = transform.position;

            if (_currentBombsPlaced < BombNumber && MapManager.IsEmptyPosition(placeBombPosition))
            {
                _currentBombsPlaced++;
                GameObject bombGameObject = NetworkPoolManager.Spawn(_bombPrefab);
                bombGameObject.transform.position = MapManager.GetMapPosition(placeBombPosition);
                bombGameObject.transform.rotation = Quaternion.identity;
                bombGameObject.GetComponent<NetworkedBomb>().Init(netId, _bombData);
                NetworkServer.Spawn(bombGameObject, bombGameObject.GetComponent<NetworkIdentity>().assetId);
            }
        }

        [Server]
        public void BombExploded()
        {
            _currentBombsPlaced--;
        }

        #endregion Class Methods
    }
}