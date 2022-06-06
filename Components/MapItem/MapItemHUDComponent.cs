using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MapItemInstanceComponent))]
    public class MapItemHUDComponent : MapItemComponent
    {
        #region Members

        private static readonly Vector3 headOffset = new Vector3(0.0f, 1.75f, 0.0f);
        private EntityHUD _mapItemHUD;

        #endregion Members

        #region Class Methods

        public override void InitModel(MapItemModel model)
        {
            base.InitModel(model);

            _mapItemHUD = GameObjectsSpawner.Instance.GetEntityHUD(EntityHUDType.MapItemHUD);
            _mapItemHUD.transform.SetParent(transform);
            _mapItemHUD.transform.localPosition = headOffset;

            _model.GetDamagedEvent += OnGetDamaged;
            _model.DieEvent += OnDie;

            UpdateLevel();
            UpdateHealthBar();
        }

        private void UpdateLevel()
        {
            _mapItemHUD.UpdateLevel(_model.Level);
        }

        private void UpdateHealthBar()
        {
            _mapItemHUD.UpdateHealthBar(_model.currentHP, _model.HP);
        }

        private void OnGetDamaged()
        {
            UpdateHealthBar();
        }

        private void OnDie()
        {
            Destroy(_mapItemHUD.gameObject);
            _mapItemHUD = null;
        }

        #endregion Class Methdos
    }
}