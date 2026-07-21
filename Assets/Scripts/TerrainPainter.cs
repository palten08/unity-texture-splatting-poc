using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerrainPainter : MonoBehaviour
{
    InputActionMap uiActionMap;
    InputActionMap terrainPaintActionMap;
    InputAction paintAction;
    InputAction uiPointAction;
    [SerializeField] Camera camera;
    [SerializeField] LayerMask layerMask;

    bool shouldPaint = false;

    bool drawDebugRay = false;
    int debugRayDrawTimeAccumulator = 0;
    Ray lastRaycast;

    Renderer terrainRenderer;
    MeshCollider terrainMeshCollider;
    [SerializeField] Texture2D terrainSplatMaskTexture;

    void Start()
    {
        uiActionMap = InputSystem.actions.FindActionMap("UI", true);
        terrainPaintActionMap = InputSystem.actions.FindActionMap("Terrain Painting", true);
        terrainPaintActionMap.Enable();
        uiActionMap.Enable();

        paintAction = terrainPaintActionMap.FindAction("Paint", true);
        uiPointAction = uiActionMap.FindAction("Point", true);
        if (!camera)
        {
            Debug.LogException(new System.Exception("Camera reference not assigned in terrain painter"));
        }
    }

    void Update()
    {
        shouldPaint = paintAction.IsPressed();
        if (drawDebugRay)
        {
            if (debugRayDrawTimeAccumulator < 500)
            {
                Debug.DrawRay(lastRaycast.origin, lastRaycast.direction * 1000, Color.yellowNice);
                debugRayDrawTimeAccumulator++;
            } else
            {
                drawDebugRay = false;
                debugRayDrawTimeAccumulator = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if (shouldPaint)
        {
            drawDebugRay = true;
            Ray rayFromScreenPoint = camera.ScreenPointToRay(uiPointAction.ReadValue<Vector2>());
            lastRaycast = rayFromScreenPoint;
            RaycastHit raycastHit;
            if (Physics.Raycast(rayFromScreenPoint.origin, rayFromScreenPoint.direction, out raycastHit, Mathf.Infinity, layerMask))
            {
                if (LayerMask.LayerToName(raycastHit.collider.gameObject.layer) == "Terrain") {
                    terrainRenderer = raycastHit.transform.GetComponent<MeshRenderer>();
                    terrainMeshCollider = raycastHit.collider as MeshCollider;
                    Debug.Log(terrainRenderer.sharedMaterial);
                    if (terrainRenderer == null || terrainMeshCollider == null)
                    {
                        return;
                    }
                    Vector2 pixelUV = raycastHit.textureCoord;
                    pixelUV.x *= terrainSplatMaskTexture.width;
                    pixelUV.y *= terrainSplatMaskTexture.height;

                    terrainSplatMaskTexture.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
                    terrainSplatMaskTexture.Apply();
                }
            }
        }
    }
}
