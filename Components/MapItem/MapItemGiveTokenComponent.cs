using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MapItemInstanceComponent))]
    public class MapItemGiveTokenComponent : MapItemComponent
    {
        #region Members

        private static readonly Vector3 tokenDisplayHeadOffset = new Vector3(0.0f, 1.5f, 0.0f);
        private float _token;

        #endregion Members

        #region Class Methods

        public override void InitModel(MapItemModel model)
        {
            base.InitModel(model);
            if (model is ChestModel)
            {
                ChestModel chestModel = model as ChestModel;
                chestModel.DieEvent += OnDie;
                _token = chestModel.Token;
            }
            else Destroy(this);
        }

        private void OnDie()
        {
            if (_model.hitBy == HitBy.HeroBomb)
                EventManager.Invoke<float, Vector3>(GameEventType.GetToken, _token, _model.Position + tokenDisplayHeadOffset);
        }

        #endregion Class Methdos
    }
}