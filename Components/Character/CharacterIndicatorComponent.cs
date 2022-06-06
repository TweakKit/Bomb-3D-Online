using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterIndicatorComponent : CharacterComponent
    {
        #region Members

        protected static readonly float characterIndicatorOffGroundHeight = 0.025f;
        protected GameObject _indicator;

        #endregion Members

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            CharacterIndicatorType characterIndicatorType;
            if (model.IsHero)
                characterIndicatorType = model.IsControlled ? CharacterIndicatorType.HeroControlIndicator : CharacterIndicatorType.HeroAutoPlayIndicator;
            else
                characterIndicatorType = CharacterIndicatorType.EnemyIndicator;
            _indicator = GameObjectsSpawner.Instance.GetCharacterIndicator(characterIndicatorType);
            _indicator.transform.SetParent(transform.FindChildByName(Constants.CharacterInterpolationRootName));
            _indicator.transform.localPosition = Vector3.up * characterIndicatorOffGroundHeight;
            _model.DieEvent += OnDie;
        }

        protected virtual void OnDie()
        {
            Destroy(_indicator);
        }

        #endregion Class Methods
    }
}