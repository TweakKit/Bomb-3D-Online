using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    public class MapItemInstanceComponent : EntityInstanceComponent<MapItemModel>
    {
        #region Class Methods

        public override void Build(MapItemModel model, Vector3 worldPosition)
        {
            base.Build(model, worldPosition);

            MapItemComponent[] mapItemComponents = gameObject.GetComponentsInChildren<MapItemComponent>();
            foreach (var mapItemComponent in mapItemComponents)
                mapItemComponent.InitModel(model);
        }

        #endregion Class Methods
    }
}