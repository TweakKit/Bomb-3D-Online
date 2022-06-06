using UnityEngine;
using ZB.Model;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    public class ItemChangeColor : MonoBehaviour
    {
        protected static readonly string ShaderColorIDFormat = "_Color0";
        [SerializeField] public GameObject mainItem;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (mainItem != null && mainItem.GetComponent<Renderer>() == null)
                Debug.LogError("A renderer is required for this main item.");
        }
#endif

        public virtual string[] GetRandomColor()
        {
            string[] COLORS = new string[] { "#ffffff", "#ff0000", "#ff00ff", "#0000ff", "#00ffff", "#00ff00", "#ffff00", "#ff5500", "#ffaa00", "#448800", "#0088aa" };
            string[] colorArray = new string[Constants.MaxTextureColors];

            for (int i = 0; i < Constants.InitialRandomTextureColors; i++)
            {
                string randomColor = COLORS[Extensions.systemRandom.Next(0, 100) % COLORS.Length];
                colorArray[i] = randomColor;
            }

            return colorArray;
        }

        public virtual void SetColor(InventoryItem item, string _colors = null)
        {
            if (_colors == null && item == null) 
                return;

            var mainRenderer = mainItem?.GetComponent<Renderer>();
            string[] colors = new string[1];
            if (_colors == null && item?.properties?.colors == null)
                colors = GetRandomColor();
            else
                colors = (item?.properties.colors ?? _colors ?? "").Split(',');

            if (colors.Length <= 1) 
                return;
                
            if (mainRenderer != null)
                SetColor(mainRenderer, colors);
        }

        public virtual void SetColor(Renderer renderer, string[] colors)
        {
#if !UNITY_SERVER
            for (int i = 0; i < colors.Length; i++)
            {
                Color color = Color.black;
                ColorUtility.TryParseHtmlString(colors[i], out color);
                renderer.material.SetColor(ShaderColorIDFormat + (i + 1), color);
            }
#endif
        }
    }
}