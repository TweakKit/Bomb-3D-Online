using System.Collections.Generic;
using UnityEngine;
using ZB.Model;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    public class CharacterItemChangeColor : ItemChangeColor
    {
        #region Members

        public GameObject petItem;
        public GameObject armorItem;
        private List<Color> _characterColors;
        private int _mainItemColorsCount;
        private int _petItemColorsCount;
        private int _armorItemColorsCount;

        #endregion Members

        #region API Methods

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (Application.isPlaying)
                return;

            base.OnValidate();

            if (petItem != null && petItem.GetComponent<Renderer>() == null)
                Debug.LogError("A renderer is required for this pet item.");

            if (armorItem != null && armorItem.GetComponent<Renderer>() == null)
                Debug.LogError("A renderer is required for this armor item.");
        }
#endif

        #endregion API Methods

        #region Class Methods

        public override void SetColor(InventoryItem item, string _colors = null)
        {
            if (_colors == null && item == null)
                return;

            _characterColors = new List<Color>();
            _mainItemColorsCount = 0;
            _petItemColorsCount = 0;
            _armorItemColorsCount = 0;
            string[] colors = new string[1];
            if (_colors == null && item?.properties?.colors == null)
            {
                colors = GetRandomColor();
            }
            else
            {
                colors = (item?.properties.colors ?? _colors ?? "").Split(',');
            }
            if (colors.Length <= 1) return;
            if (mainItem != null)
            {
                _mainItemColorsCount = colors.Length + 1;
                SetColor(mainItem.GetComponent<Renderer>(), colors);
            }
            if (item != null && item.InventoryItemType == InventoryItemType.Hero)
            {
                InventoryItem petData = item.GetPet();
                if (petItem != null && petData != null)
                {
                    colors = (petData.properties.colors ?? _colors).Split(',');
                    _petItemColorsCount = colors.Length + 1;
                    SetColor(petItem.GetComponent<Renderer>(), colors);
                }
                InventoryItem armorData = item.GetArmor();
                if (armorItem != null && armorData != null)
                {
                    colors = (armorData.properties.colors ?? _colors).Split(',');
                    _armorItemColorsCount = colors.Length + 1;
                    SetColor(armorItem.GetComponent<Renderer>(), colors);
                }
            }
        }

        public override void SetColor(Renderer renderer, string[] colors)
        {
            _characterColors.Add(renderer.material.GetColor(ShaderColorIDFormat + "0"));

            for (int i = 0; i < colors.Length; i++)
            {
                Color color = Color.black;
                ColorUtility.TryParseHtmlString(colors[i], out color);
                renderer.material.SetColor(ShaderColorIDFormat + (i + 1), color);
                _characterColors.Add(color);
            }
        }

        public void ChangeColor(Color color)
        {
            if (mainItem != null)
            {
                var mainRenderer = mainItem.GetComponent<Renderer>();
                for (int i = 0; i < _mainItemColorsCount; i++)
                    mainRenderer.material.SetColor(ShaderColorIDFormat + i, color);
            }

            if (petItem != null)
            {
                var petRenderer = petItem.GetComponent<Renderer>();
                for (int i = 0; i < _petItemColorsCount; i++)
                    petRenderer.material.SetColor(ShaderColorIDFormat + i, color);
            }

            if (armorItem != null)
            {
                var armorRenderer = armorItem.GetComponent<Renderer>();
                for (int i = 0; i < _armorItemColorsCount; i++)
                    armorRenderer.material.SetColor(ShaderColorIDFormat + i, color);
            }
        }

        public void ResetColor()
        {
            if (mainItem != null)
            {
                var mainRenderer = mainItem.GetComponent<Renderer>();
                for (int i = 0; i < _mainItemColorsCount; i++)
                    mainRenderer.material.SetColor(ShaderColorIDFormat + i, _characterColors[i]);
            }

            if (petItem != null)
            {
                var petRenderer = petItem.GetComponent<Renderer>();
                for (int i = 0; i < _petItemColorsCount; i++)
                    petRenderer.material.SetColor(ShaderColorIDFormat + i, _characterColors[i + _mainItemColorsCount]);
            }

            if (armorItem != null)
            {
                var armorRenderer = armorItem.GetComponent<Renderer>();
                for (int i = 0; i < _armorItemColorsCount; i++)
                    armorRenderer.material.SetColor(ShaderColorIDFormat + i, _characterColors[i + _mainItemColorsCount + _petItemColorsCount]);
            }
        }

        #endregion Class Methods
    }
}