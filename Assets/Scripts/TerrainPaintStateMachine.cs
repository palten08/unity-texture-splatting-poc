using System;
using UnityEngine;

public enum TerrainPaintState
{
    Overview,
    Paint
}
public class TerrainPaintStateMachine : MonoBehaviour
{
    private static TerrainPaintStateMachine _instance;
    public static TerrainPaintStateMachine instance { get {return _instance; } }
    public TerrainPaintState terrainPaintState = TerrainPaintState.Overview;

    public static event Action<TerrainPaintStateMachine> TerrainPaintStateChangedEvent;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
    }
    public void ChangeTerrainPaintState(TerrainPaintState targetState)
    {
        if (terrainPaintState == targetState)
        {
            return;
        }

        terrainPaintState = targetState;
        TerrainPaintStateChangedEvent(this);
    }
}
