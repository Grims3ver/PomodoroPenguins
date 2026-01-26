using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCameraFocus : MonoBehaviour
{
    private Transform focusTargetPosition;

    [SerializeField] float cameraSpeed;

    [SerializeField] CinemachineCamera cinemachineCam;

    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference zoomAction;


    private void Awake()
    {
        if (!focusTargetPosition)
        {
            focusTargetPosition = this.transform;
        }
    }
    void Update()
    {
        if (!focusTargetPosition|| !cinemachineCam || moveAction == null || moveAction.action == null || zoomAction == null || zoomAction.action == null) return;

        //Read Move (Vector2)
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        float zoomZoom = zoomAction.action.ReadValue<float>();

        Vector3 inputDir = new Vector3(move.x, 0f, move.y);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        Vector3 moveDir = inputDir;

        if (Camera.main != null)
        {
            //Project camera forward/right onto XZ plane
            Vector3 camFwd = Camera.main.transform.forward;
            camFwd.y = 0f;
            camFwd.Normalize();

            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            moveDir = camRight * inputDir.x + camFwd * inputDir.z;
        }

        Vector3 next = focusTargetPosition.position + moveDir * cameraSpeed * Time.deltaTime;

        focusTargetPosition.position = next; 
    }
    
}
