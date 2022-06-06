using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterGiveTokenComponent : CharacterComponent
    {
        #region Members

        protected static readonly Vector3 tokenDisplayHeadOffset = new Vector3(0.0f, 2.0f, 0.0f);
        protected static readonly Vector3 tokenDisplayPetHeadOffset = new Vector3(0.0f, 3.0f, 0.0f);
        protected float _token;

        #endregion Members

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);
            if (model is EnemyModel)
            {
                EnemyModel enemyModel = _model as EnemyModel;
                enemyModel.DieEvent += OnDie;
                _token = enemyModel.Token;
            }
            else Destroy(this);
        }

        private void OnDie()
        {
            if (_model.hitBy == HitBy.HeroBomb)
                EventManager.Invoke<float, Vector3>(GameEventType.GetToken, _token, _model.Position + (_model.hasPet ? tokenDisplayPetHeadOffset : tokenDisplayHeadOffset));
        }

        #endregion Class Methdos
    }
}