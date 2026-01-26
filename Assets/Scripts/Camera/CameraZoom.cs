using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{

    [SerializeField] CinemachineCamera cam;
    [SerializeField] InputActionReference zoom;

    [SerializeField] float minFov = 20f;
    [SerializeField] float maxFov = 55f;

    [Header("Feel")]
    [SerializeField] float degreesPerScrollUnit = 4f; //Try 2–10
    [SerializeField] float smoothness = 18f; //Try 12–30

    float _targetFov;

    void Awake() => _targetFov = cam.Lens.FieldOfView;

    void OnEnable()
    {
        zoom.action.Enable();
        zoom.action.performed += OnZoom;
    }

    void OnDisable()
    {
        zoom.action.performed -= OnZoom;
        zoom.action.Disable();
    }

    void OnZoom(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<float>();
        if (Mathf.Approximately(scroll, 0f)) return;

        _targetFov = Mathf.Clamp(_targetFov - scroll * degreesPerScrollUnit, minFov, maxFov);
    }

    void LateUpdate()
    {
        var lens = cam.Lens;
        float t = 1f - Mathf.Exp(-smoothness * Time.unscaledDeltaTime);
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, _targetFov, t);
        cam.Lens = lens;
    }
}