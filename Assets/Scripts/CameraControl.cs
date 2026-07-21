using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    InputActionMap cameraControlActionMap;
    InputAction moveCameraAction;
    InputAction rotateCameraAction;
    InputAction zoomCameraAction;

    [SerializeField] Transform pivotPointTransform;

    [SerializeField] Transform cameraTransform;
    [SerializeField] Camera mainCamera;

    [SerializeField] float movementSpeed = 10.0f;
    [SerializeField] float rotationSpeed = 5.0f;
    [SerializeField] float zoomSpeed = 10.0f;
    [SerializeField] float maxZoomOut = 20.0f;
    [SerializeField] float maxZoomIn = 5.0f;

    Vector3 currentpivotPointTransformEulerAngles;

    void Start()
    {
        cameraControlActionMap = InputSystem.actions.FindActionMap("Camera Control", true);
        cameraControlActionMap.Enable();
        moveCameraAction = cameraControlActionMap.FindAction("Move", true);
        rotateCameraAction = cameraControlActionMap.FindAction("Rotate", true);
        zoomCameraAction = cameraControlActionMap.FindAction("Zoom", true);

        if (!pivotPointTransform)
        {
            Debug.LogException(new System.Exception("No pivot point transform assigned in camera controller"));
        }
        if (!cameraTransform)
        {
            Debug.LogException(new System.Exception("No camera transform assigned in camera controller"));
        }
        if (!mainCamera)
        {
            Debug.LogException(new System.Exception("No camera assigned in camera controller"));
        }
    }

    void LateUpdate()
    {
        // Move the entire CameraRig transform via WASD
        if (Mathf.Abs(moveCameraAction.ReadValue<Vector2>().x) > 0.001f || Mathf.Abs(moveCameraAction.ReadValue<Vector2>().y) > 0.001f)
        {
            Vector3 currentPosition = transform.position;
            Vector3 movementDirection = moveCameraAction.ReadValue<Vector2>().y * movementSpeed * pivotPointTransform.forward + moveCameraAction.ReadValue<Vector2>().x * movementSpeed * pivotPointTransform.right;
            movementDirection.y = 0;
            currentPosition += movementDirection * Time.deltaTime;
            transform.position = currentPosition;
        }

        // Rotate only the interior pivot point transform
        if (Mathf.Abs(rotateCameraAction.ReadValue<Vector2>().x) > 0.001f || Mathf.Abs(rotateCameraAction.ReadValue<Vector2>().y) > 0.001f)
        {
            currentpivotPointTransformEulerAngles -= new Vector3(rotateCameraAction.ReadValue<Vector2>().y, -1 * rotateCameraAction.ReadValue<Vector2>().x, 0) * Time.deltaTime * rotationSpeed;
            pivotPointTransform.localEulerAngles = currentpivotPointTransformEulerAngles;
        }

        // Zoom just the camera along its forward vector towards / from the pivot point
        if (Mathf.Abs(zoomCameraAction.ReadValue<Vector2>().y) > 0.001f)
        {
            Vector3 targetZoomPosition = Vector3.MoveTowards(cameraTransform.position, pivotPointTransform.position, zoomCameraAction.ReadValue<Vector2>().y * zoomSpeed * Time.deltaTime);
            Vector3 offset = targetZoomPosition - pivotPointTransform.position;
            float distance = offset.magnitude;
            float clampedDistance = Mathf.Clamp(distance, maxZoomIn, maxZoomOut);
            cameraTransform.position = pivotPointTransform.position + (offset.normalized * clampedDistance);
        }
    }

    public Ray CameraRayCast(Vector3 inputPosition)
    {
        Ray resultantRay = mainCamera.ScreenPointToRay(inputPosition);
        return resultantRay;
    }
}
