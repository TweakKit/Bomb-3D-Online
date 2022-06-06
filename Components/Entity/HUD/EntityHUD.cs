using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    public class EntityHUD : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private Image _heathBarSlider;
        [SerializeField]
        private TextMeshProUGUI _levelText;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            Canvas HUDWorldCanvas = gameObject.GetComponent<Canvas>();
            HUDWorldCanvas.worldCamera = Camera.main;
        }

        #endregion API Methods

        #region Class Methods

        public void UpdateLevel(int level)
        {
            _levelText.text = level.ToString();
        }

        public void UpdateHealthBar(float currentHP, float maxHP)
        {
            _heathBarSlider.fillAmount = currentHP / maxHP;
        }

        #endregion Class Methods
    }
}