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

    [SerializeField] DecalProjector brushDecal;

    [SerializeField] EventSystem uiEventSystem;

    [SerializeField] Texture2D brushTexture;
    [SerializeField] Material paintBrushMaterial;
    [SerializeField] float paintBrushSize = 0.5f;

    bool _isPainting = false;

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
        paintBrushMaterial.SetTexture("_Brush_Texture", brushTexture);

        RenderTexture temporaryRenderTexture = RenderTexture.active;
        RenderTexture.active = terrainSplatMaskRenderTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = temporaryRenderTexture;
    }

    void OnPaintEvent(InputAction.CallbackContext inputSystemCallbackContext)
    {
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

                RenderTexture temporaryRT = RenderTexture.GetTemporary(terrainSplatMaskRenderTexture.width, terrainSplatMaskRenderTexture.height, 0, terrainSplatMaskRenderTexture.format);

                paintBrushMaterial.SetTexture("_MainTex", temporaryRT);

                Graphics.Blit(terrainSplatMaskRenderTexture, temporaryRT);
                Graphics.Blit(temporaryRT, terrainSplatMaskRenderTexture, paintBrushMaterial, 0);

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
    }
}
        /*
        shouldPaint = paintAction.WasPerformedThisFrame() && paintAction.IsPressed() && !uiEventSystem.IsPointerOverGameObject();
        Ray rayFromScreenPoint = camera.ScreenPointToRay(uiPointAction.ReadValue<Vector2>());
        RaycastHit raycastHit;
        if (Physics.Raycast(rayFromScreenPoint.origin, rayFromScreenPoint.direction, out raycastHit, Mathf.Infinity, layerMask))
        {
            if (!brushDecal.enabled)
            {
               brushDecal.enabled = true; 
            }
            Vector3 brushDecalPosition = brushDecal.transform.position;
            brushDecalPosition.x = raycastHit.point.x;
            brushDecalPosition.z = raycastHit.point.z;
            brushDecal.transform.position = brushDecalPosition;
            Debug.DrawRay(rayFromScreenPoint.origin, rayFromScreenPoint.direction * 1000, Color.yellowNice);
            Debug.DrawLine(raycastHit.point, Vector3.up * 50, Color.aliceBlue);
        } else
        {
            brushDecal.enabled = false;
        }
        if (drawDebugRay)
        {
            Debug.DrawRay(lastRaycast.origin, lastRaycast.direction * 1000, Color.yellowNice);
        }
        */


    // void FixedUpdate()
    // {
    //     if (shouldPaint)
    //     {
    //         Debug.Log("Beans");
    //         Ray rayFromScreenPoint = camera.ScreenPointToRay(uiPointAction.ReadValue<Vector2>());
    //         lastRaycast = rayFromScreenPoint;
    //         RaycastHit raycastHit;
    //         if (Physics.Raycast(rayFromScreenPoint.origin, rayFromScreenPoint.direction, out raycastHit, Mathf.Infinity, layerMask))
    //         {
    //             if (LayerMask.LayerToName(raycastHit.collider.gameObject.layer) == "Terrain") {
    //                 Vector2 pixelUV = raycastHit.textureCoord;
    //                 paintBrushMaterial.SetVector("Brush Coordinates", pixelUV);

    //                 RenderTexture temporaryRenderTexture = RenderTexture.GetTemporary(terrainSplatMaskRenderTexture.width, terrainSplatMaskRenderTexture.height, 0, terrainSplatMaskRenderTexture.format);

    //                 paintBrushMaterial.SetTexture("Target Texture", temporaryRenderTexture);

    //                 Graphics.Blit(terrainSplatMaskRenderTexture, temporaryRenderTexture, paintBrushMaterial);

    //                 Graphics.Blit(temporaryRenderTexture, terrainSplatMaskRenderTexture);

    //                 RenderTexture.ReleaseTemporary(temporaryRenderTexture);
    //             }
    //         }
    //     }
    // }

