using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class NetworkedPlayerHUD : MonoBehaviour
{
    #region Members

    [SerializeField]
    private Image _heathBarSlider;
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _levelText;
    [SerializeField]
    private Image _zodiacSignImage;
    [SerializeField]
    private Image _controlSignImage;

    #endregion Members

    #region API Methods

    private void Awake()
    {
        Canvas HUDWorldCanvas = gameObject.GetComponentInChildren<Canvas>();
        HUDWorldCanvas.worldCamera = Camera.main;
    }

    #endregion API Methods

    #region Class Methods

    public virtual void Init(string name, int level)
    {
        _nameText.text = name;
        _levelText.text = level.ToString();
    }

    public virtual void UpdateHealthBar(float currentHP, float maxHP)
    {
        _heathBarSlider.fillAmount = currentHP / maxHP;
    }

    #endregion Class Methods
}