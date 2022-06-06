using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MapItemInstanceComponent))]
    public class MapItemSpawnChestComponent : MapItemComponent
    {
        #region Class Methods

        public override void InitModel(MapItemModel model)
        {
            base.InitModel(model);
            _model.DieEvent += OnDie;
        }

        private void OnDie()
        {
            EventManager.Invoke<Vector3>(GameEventType.ExposeHiddenItem, _model.Position);
        }
    }

    #endregion Class Methods
}