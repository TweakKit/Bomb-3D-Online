using System;
using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    public class BombInstanceComponent : EntityInstanceComponent<BombModel>
    {
        #region Class Methods

        public override void Build(BombModel model, Vector3 worldPosition)
        {
            base.Build(model, worldPosition);

            MapManager.UpdateMap(worldPosition, MapSlotType.Bomb);

            BombComponent[] bombComponents = gameObject.GetComponentsInChildren<BombComponent>();
            foreach (var bombComponent in bombComponents)
                bombComponent.InitModel(model);
            try
            {
                var scriptObj = gameObject.GetComponentInChildren<ItemChangeColor>();
                if (scriptObj != null)
                {
                    scriptObj.SetColor(null, model.Colors);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        #endregion Class Methods
    }
}