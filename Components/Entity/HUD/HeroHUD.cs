using UnityEngine;
using UnityEngine.UI;

namespace ZB.Gameplay
{
    public class HeroHUD : CharacterHUD
    {
        #region Members

        [SerializeField]
        private Image _zodiacSignImage;

        [SerializeField]
        private Image _controlSignImage;

        #endregion Members

        #region Class Methods

        public override void Init(CharacterModel characterModel)
        {
            _controlSignImage.enabled = characterModel.IsControlled;
            _zodiacSignImage.sprite = DataManager.Instance.GetZodiacSign(characterModel.CharacterType);
        }

        #endregion Class Methods
    }
}