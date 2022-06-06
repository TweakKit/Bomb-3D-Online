using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Class Methods

        public virtual void RegisterUseItem() { }

        public void UseTNTExplosiveItem(NetworkedBombData bombData, GameObject bombTNTExplosivePrefab)
        {
            CmdUseTNTExplosiveItem(bombData, bombTNTExplosivePrefab);
        }

        [Command]
        protected virtual void CmdUseTNTExplosiveItem(NetworkedBombData bombData, GameObject bombTNTExplosivePrefab)
        {
            GameObject bombTNTExplosiveGameObject = Instantiate(bombTNTExplosivePrefab, MapManager.GetMapPosition(transform.position), Quaternion.identity);
            bombTNTExplosiveGameObject.GetComponent<NetworkedBomb>().Init(netId, bombData);
            NetworkServer.Spawn(bombTNTExplosiveGameObject, bombTNTExplosivePrefab.GetComponent<NetworkIdentity>().assetId);
        }

        public void UseSlowTrapItem(GameObject slowTrapItemObjectPrefab)
        {
            CmdUseSlowTrapItem(slowTrapItemObjectPrefab);
        }

        [Command]
        protected virtual void CmdUseSlowTrapItem(GameObject slowTrapItemObjectPrefab)
        {
            GameObject slowTrapItemObjectGameObject = Instantiate(slowTrapItemObjectPrefab, MapManager.GetMapPosition(transform.position), Quaternion.identity);
            NetworkServer.Spawn(slowTrapItemObjectGameObject, slowTrapItemObjectPrefab.GetComponent<NetworkIdentity>().assetId);
        }

        public void UseRocketLaunchItem(GameObject rocketLaunchItemObjectPrefab)
        {
            CmdUseRocketLaunchItem(rocketLaunchItemObjectPrefab);
        }

        [Command]
        protected virtual void CmdUseRocketLaunchItem(GameObject rocketLaunchItemObjectPrefab)
        {
            GameObject rocketLaunchItemObjectGameObject = Instantiate(rocketLaunchItemObjectPrefab, MapManager.GetMapPosition(transform.position), Quaternion.identity);
            NetworkServer.Spawn(rocketLaunchItemObjectGameObject, rocketLaunchItemObjectPrefab.GetComponent<NetworkIdentity>().assetId);
        }

        #endregion Class Methods
    }
}