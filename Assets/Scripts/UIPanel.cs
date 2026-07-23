using UnityEngine;

public class UIPanel : MonoBehaviour
{
    [SerializeField] TerrainPaintState assignedModeState;
    [SerializeField] Canvas canvasComponent;

    void Start()
    {
        if (!canvasComponent)
        {
            Debug.LogException(new System.Exception("Canvas component reference not set in UI panel instance"));
        }

        if (TerrainPaintStateMachine.instance.terrainPaintState == assignedModeState)
        {
            canvasComponent.enabled = true;
        } else
        {
            canvasComponent.enabled = false;
        }
        TerrainPaintStateMachine.OnTerrainPaintStateChangedEvent += ShouldShowPanel;        
    }

    private void ShouldShowPanel(TerrainPaintStateMachine paintModeStateMachine)
    {
        if (paintModeStateMachine.terrainPaintState == assignedModeState)
        {
            canvasComponent.enabled = true;
        } else
        {
            canvasComponent.enabled = false;
        }
    }
}
