using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDispatcher : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] InputActionReference clickAction;
    [SerializeField] float maxDistance = 200f;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void OnEnable()
    {
        clickAction.action.Enable();
        clickAction.action.performed += OnClick;
    }

    void OnDisable()
    {
        clickAction.action.performed -= OnClick;
        clickAction.action.Disable();
    }

    void OnClick(InputAction.CallbackContext ctx)
    {
        if (!cam) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            Transform t = hit.collider.transform;
            while (t != null)
            {
                ClickableBehaviour clickable = t.GetComponent<ClickableBehaviour>();
                if (clickable)
                {
                    clickable.OnClick();
                    return;
                }

                t = t.parent;
            }
        }
    }
}