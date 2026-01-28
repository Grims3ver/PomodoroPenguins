using UnityEngine;
using UnityEngine.InputSystem;
using static Clickable;

public class ClickDispatcher : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] InputActionReference clickAction;
    [SerializeField] LayerMask hitMask = ~0;
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

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitMask))
        {
            var behaviours = hit.collider.GetComponentsInParent<MonoBehaviour>(true);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is Clickable clickable)
                {
                    clickable.OnClick();
                    return; //Stop after first match
                }

            }
        }
    }
}