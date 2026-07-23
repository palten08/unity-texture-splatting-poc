using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BrushSettingsSlider : MonoBehaviour
{
    [SerializeField] TerrainPainter terrainPainter;
    [SerializeField] Slider brushSizeSliderComponent;
    [SerializeField] Slider brushStrengthSliderComponent;
    [SerializeField] Canvas strengthValueTooltipCanvasComponent;
    [SerializeField] Canvas sizeValueTooltipCanvasComponent;
    [SerializeField] TMP_Text strengthValueTooltipText;
    [SerializeField] TMP_Text sizeValueTooltipText;
    public void SetBrushSize()
    {
        terrainPainter.SetBrushSize(brushSizeSliderComponent.value);
        sizeValueTooltipText.text = brushSizeSliderComponent.value.ToString("F2");
    }

    public void SetBrushStrength()
    {
        terrainPainter.SetPaintStrength(brushStrengthSliderComponent.value);
        strengthValueTooltipText.text = brushStrengthSliderComponent.value.ToString("F2");
    }

    void Start()
    {
        strengthValueTooltipCanvasComponent.enabled = true;
        sizeValueTooltipCanvasComponent.enabled = true;
        terrainPainter.SetBrushSize(brushSizeSliderComponent.value);
        terrainPainter.SetPaintStrength(brushStrengthSliderComponent.value);
    }
}
