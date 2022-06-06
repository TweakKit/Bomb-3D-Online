using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterAttackComponent : CharacterComponent
    {
        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            _model.AttackEvent += OnAttackEvent;
            if (_model.IsHero)
                _model.AttackEvent += OnPlaySoundFX;
        }

        protected virtual void OnAttackEvent()
        {
            _model.Attack();
            GameObject bombGameObject = PoolManager.GetObject(_model.bombData.Type);
            bombGameObject.GetComponent<BombInstanceComponent>().Build(_model.bombData, MapManager.GetMapPosition(_model.Position));
        }

        protected virtual void OnPlaySoundFX()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.SFX_ingame_hero_put_bom);
        }

        #endregion Class Methods
    }
}