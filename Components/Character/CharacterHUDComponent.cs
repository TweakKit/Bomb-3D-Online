using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterHUDComponent : CharacterComponent
    {
        #region Members

        protected static readonly Vector3 headOffset = new Vector3(0.0f, 2.5f, 0.0f);
        protected static readonly Vector3 petHeadOffset = new Vector3(0.0f, 3.25f, 0.0f);
        protected CharacterHUD _characterHUD;

        #endregion Members

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            _characterHUD = GameObjectsSpawner.Instance.GetCharacterHUD(_model.IsHero ? EntityHUDType.HeroHUD : EntityHUDType.EnemyHUD);
            _characterHUD.transform.SetParent(transform.FindChildByName(Constants.CharacterInterpolationRootName));
            _characterHUD.transform.localPosition = _model.hasPet ? petHeadOffset : headOffset;

            _model.GetDamagedEvent += OnGetDamaged;
            _model.DieEvent += OnDie;

            InitHUD();
            UpdateLevel();
            UpdateHealthBar();
        }

        protected virtual void InitHUD()
        {
            _characterHUD.Init(_model);
        }

        protected virtual void UpdateLevel()
        {
            _characterHUD.UpdateLevel(_model.Level);
        }

        protected virtual void UpdateHealthBar()
        {
            _characterHUD.UpdateHealthBar(_model.currentHP, _model.HP);
        }

        protected virtual void OnGetDamaged()
        {
            UpdateHealthBar();
        }

        protected virtual void OnDie()
        {
            Destroy(_characterHUD.gameObject);
            _characterHUD = null;
        }

        #endregion Class Methdos
    }
}