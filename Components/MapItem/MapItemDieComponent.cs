using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MapItemInstanceComponent))]
    public class MapItemDieComponent : MapItemComponent
    {
        #region Class Methods

        public override void InitModel(MapItemModel model)
        {
            base.InitModel(model);

            if (_model.IsBrick)
                _model.DieEvent += OnBrickDie;
            else if (_model.IsChest)
                _model.DieEvent += OnChestDie;
        }

        protected virtual void OnBrickDie()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.SFX_ingame_brick_explosion);
            MapManager.UpdateMap(transform.position, MapSlotType.Empty);
            DestroySelf();
        }

        protected virtual void OnChestDie()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.SFX_ingame_chest_broken);
            MapManager.UpdateMap(transform.position, MapSlotType.Empty);
            EventManager.Invoke<Vector3>(GameEventType.DestroyChest, _model.Position);
            DestroySelf();
        }

        protected virtual void DestroySelf()
        {
            CreateExplosionEffect();
            Destroy(gameObject);
        }

        protected virtual void CreateExplosionEffect()
        {
            GameObject explosionEffect = PoolManager.GetObject(_model.ExplosionEffectType);
            explosionEffect.transform.position = transform.position + Vector3.up * MapSetting.EffectGroundOffset;
        }

        #endregion Class Methods
    }
}