using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class TerrainPainter : MonoBehaviour
{
    InputActionMap uiActionMap;
    InputActionMap terrainPaintActionMap;
    InputAction paintAction;
    InputAction uiPointAction;
    [SerializeField] Camera camera;
    [SerializeField] LayerMask layerMask;
    Ray lastRaycast;
    [SerializeField] RenderTexture terrainSplatMaskRenderTexture;

    [SerializeField] DecalProjector paintBrushCursor;

    [SerializeField] EventSystem uiEventSystem;

    [SerializeField] Texture2D brushTexture;
    [SerializeField] Material paintBrushMaterial;
    [SerializeField] float paintBrushSize = 1.0f;
    [SerializeField] float paintStrength = 0.10f;
    [SerializeField] MeshRenderer terrainMeshRenderer;
    Vector2 _terrainSize;
    int _cursorProjectionDepth = 1500;
    Vector3 _cursorSize;

    void Start()
    {
        uiActionMap = InputSystem.actions.FindActionMap("UI", true);
        terrainPaintActionMap = InputSystem.actions.FindActionMap("Terrain Painting", true);
        terrainPaintActionMap.Enable();
        uiActionMap.Enable();

        paintAction = terrainPaintActionMap.FindAction("Paint", true);
        paintAction.performed += OnPaintEvent;
        uiPointAction = uiActionMap.FindAction("Point", true);
        if (!camera)
        {
            Debug.LogException(new System.Exception("Camera reference not assigned in terrain painter"));
        }
        if (!terrainMeshRenderer)
        {
            Debug.LogException(new System.Exception("Terrain mesh renderer reference not assigned in terrain painter"));
        }
        _terrainSize.x = terrainMeshRenderer.bounds.size.x;
        _terrainSize.y = terrainMeshRenderer.bounds.size.z;

        paintBrushMaterial.SetTexture("_Brush_Texture", brushTexture);

        paintBrushMaterial.SetVector("_Terrain_Size", _terrainSize);

        RenderTexture temporaryRenderTexture = RenderTexture.active;
        RenderTexture.active = terrainSplatMaskRenderTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = temporaryRenderTexture;

        _cursorSize = new Vector3(paintBrushSize, paintBrushSize, _cursorProjectionDepth);
    }

    void OnPaintEvent(InputAction.CallbackContext inputSystemCallbackContext)
    {
        if (TerrainPaintStateMachine.instance.terrainPaintState != TerrainPaintState.Paint)
        {
            return;
        }
        if (uiEventSystem.IsPointerOverGameObject())
        {
            return;
        }

        AttemptPaint();
    }

    void AttemptPaint()
    {
        Ray screenPointRay = camera.ScreenPointToRay(uiPointAction.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(screenPointRay.origin, screenPointRay.direction, out hit, Mathf.Infinity, layerMask))
        {
            if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Terrain")
            {
                Vector2 raycastTexelCoordinates = hit.textureCoord;

                paintBrushMaterial.SetFloat("_Brush_Size", paintBrushSize);

                paintBrushMaterial.SetVector("_Brush_Coordinates", raycastTexelCoordinates);

                paintBrushMaterial.SetFloat("_Strength", paintStrength);

                RenderTexture temporaryRT = RenderTexture.GetTemporary(terrainSplatMaskRenderTexture.width, terrainSplatMaskRenderTexture.height, 0, terrainSplatMaskRenderTexture.format);

                paintBrushMaterial.SetTexture("_MainTex", temporaryRT);

                Graphics.Blit(terrainSplatMaskRenderTexture, temporaryRT, paintBrushMaterial, 0);
                Graphics.Blit(temporaryRT, terrainSplatMaskRenderTexture);

                RenderTexture.ReleaseTemporary(temporaryRT);
            }
        }
    }

    void Update()
    {
        if (TerrainPaintStateMachine.instance.terrainPaintState != TerrainPaintState.Paint)
        {
            return;
        }
        if (uiEventSystem.IsPointerOverGameObject())
        {
            return;
        }

        if (paintAction.IsPressed())
        {
            AttemptPaint();
        }

        //Vector3 _cursorSize = new Vector3(paintBrushSize, paintBrushSize, _cursorProjectionDepth);

        if (_cursorSize.x != paintBrushSize || _cursorSize.y != paintBrushSize)
        {
            _cursorSize.x = paintBrushSize;
            _cursorSize.y = paintBrushSize;
        }

        Ray screenPointRay = camera.ScreenPointToRay(uiPointAction.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(screenPointRay.origin, screenPointRay.direction, out hit, Mathf.Infinity, layerMask))
        {
            if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Terrain")
            {
                Vector3 paintBrushCursorPosition = paintBrushCursor.transform.position;
                paintBrushCursorPosition.x = hit.point.x;
                paintBrushCursorPosition.z = hit.point.z;
                paintBrushCursor.transform.position = paintBrushCursorPosition;

            paintBrushCursor.size = _cursorSize;
            }
        }
    }
}
