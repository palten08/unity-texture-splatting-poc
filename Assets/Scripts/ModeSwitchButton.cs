using UnityEngine;

public class ModeSwitchButton : MonoBehaviour
{
    public void EnterOverviewMode()
    {
        TerrainPaintStateMachine.instance.ChangeTerrainPaintState(TerrainPaintState.Overview);
    }

    public void EnterPaintMode()
    {
        TerrainPaintStateMachine.instance.ChangeTerrainPaintState(TerrainPaintState.Paint);
    }
}
