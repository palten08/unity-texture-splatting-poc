using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintTextureDropdown : MonoBehaviour
{
    [SerializeField] TerrainPainter terrainPainter;
    [SerializeField] TMP_Dropdown textureDropdown;
    public void SetPaintTexture()
    {
        if (textureDropdown.value == 0)
        {
            terrainPainter.SetPaintValue(1.0f);
        } else if (textureDropdown.value == 1)
        {
            terrainPainter.SetPaintValue(0.0f);
        }
    }
    void Start()
    {
        SetPaintTexture();
    }
}
