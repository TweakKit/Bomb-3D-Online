using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    public class CharacterInstanceComponent : EntityInstanceComponent<CharacterModel>
    {
        #region Class Methods

        public override void Build(CharacterModel model, Vector3 worldPosition)
        {
            base.Build(model, worldPosition);

            CharacterComponent[] characterComponents = gameObject.GetComponentsInChildren<CharacterComponent>();
            foreach (var characterComponent in characterComponents)
                characterComponent.InitModel(model);
        }

        protected override void Init(CharacterModel model)
        {
            base.Init(model);
            SetRotation(model);
            SetLayer(model);
            SetCollider(model);
        }

        protected virtual void SetRotation(CharacterModel model)
        {
            model.visualTransform.rotation = Quaternion.Euler(Vector3.up * 180);
        }

        protected virtual void SetLayer(CharacterModel model)
        {
            if (model.IsHero)
            {
                if (model.IsControlled)
                    gameObject.layer = Constants.ControlledHeroLayerIndex;
                else
                    gameObject.layer = Constants.HeroLayerIndex;
            }
            else gameObject.layer = Constants.EnemyLayerIndex;
        }

        protected virtual void SetCollider(CharacterModel model)
        {
            if (model.IsControlled)
            {
                var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.center = new Vector3(0.0f, 1.0f, 0.0f);
                capsuleCollider.radius = 0.5f;
                capsuleCollider.height = 2.0f;
            }
            else
            {
                var boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.center = new Vector3(0.0f, 0.5f, 0.0f);
                boxCollider.size = new Vector3(0.5f, 1.0f, 0.5f);
            }
        }

        #endregion Class Methods
    }
}