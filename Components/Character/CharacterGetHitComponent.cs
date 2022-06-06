using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterGetHitComponent : CharacterComponent, IEntityGetHit
    {
        #region Members

        protected static readonly float changeBackColorDelay = 0.25f;
        protected static readonly Color hitColor = Color.red;
        protected int _changeColorLTUniqueID;
        protected CharacterItemChangeColor _characterItemChangeColor;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            _characterItemChangeColor = gameObject.GetComponentInChildren<CharacterItemChangeColor>();
        }

        #endregion API Methods

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            if (_model.IsHero)
                _model.GetDamagedEvent += OnPlaySoundFX;
        }

        public virtual void GetHit(float damageValue, HitBy hitBy)
        {
            if (_model.isActivated && !_model.IsDead)
            {
                ShowHitEffect();

                _model.hitBy = hitBy;
                _model.currentHP -= damageValue * (1 - _model.Defense);
                _model.GetDamagedEvent.Invoke();

                if (_model.currentHP <= 0)
                    _model.DieEvent.Invoke();
            }
        }

        protected virtual void ShowHitEffect()
        {
            LeanTween.cancel(_changeColorLTUniqueID);
            ChangeHitColor();
            _changeColorLTUniqueID = LeanTween.delayedCall(changeBackColorDelay, ResetColor).uniqueId;
        }

        protected virtual void ChangeHitColor()
        {
            _characterItemChangeColor.ChangeColor(hitColor);
        }

        protected virtual void ResetColor()
        {
            _characterItemChangeColor.ResetColor();
        }

        protected virtual void OnPlaySoundFX()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.SFX_ingame_hero_hit);
        }

        #endregion Class Methods
    }
}