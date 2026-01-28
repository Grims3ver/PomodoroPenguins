using UnityEngine;
using System.Collections;

public class Wiggleable : MonoBehaviour, Clickable
{
    [Header("Pivot")]
    [SerializeField] Transform wigglePivot;

    [Header("Wiggle Feel")]
    [SerializeField] float amplitudeDegrees = 6f;
    [SerializeField] float duration = 0.25f;
    [SerializeField] float frequency = 18f;
    [SerializeField] float damping = 10f;

    [Header("Direction")]
    [SerializeField] bool swayRelativeToCamera = true;
    [SerializeField] Camera cam;
    [SerializeField] Vector3 swayDirectionWorld = Vector3.right; //Used if not relative to camera
    [SerializeField] Vector3 upAxisWorld = Vector3.up;

    [Header("Randomness")]
    [SerializeField] bool randomizeLeftRight = true;

    Coroutine _routine;
    Quaternion _restLocalRot;

    void Reset()
    {
        //Auto-setup
        wigglePivot = transform;
        cam = Camera.main;
    }

    void Awake()
    {
        if (!wigglePivot) wigglePivot = transform;
        if (!cam) cam = Camera.main;

        _restLocalRot = wigglePivot.localRotation;
    }

    public void OnClick()
    {
        Plink();
    }

    public void Plink()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(WiggleRoutine());
    }

    IEnumerator WiggleRoutine()
    {
        _restLocalRot = wigglePivot.localRotation;

        //Pick the direction the top should move (left-right), then compute the rotation axis
        Vector3 swayDir = swayRelativeToCamera && cam ? cam.transform.right : swayDirectionWorld;

        //Keep sway parallel to ground
        swayDir.y = 0f;
        if (swayDir.sqrMagnitude < 0.0001f) swayDir = Vector3.right;
        swayDir.Normalize();

        Vector3 up = upAxisWorld;
        up.Normalize();

        //Rotation axis that produces sway in 'swayDir' while staying upright
        Vector3 axisWorld = Vector3.Cross(up, swayDir);
        if (axisWorld.sqrMagnitude < 0.0001f) axisWorld = Vector3.forward;
        axisWorld.Normalize();

        //Convert world axis to local axis for localRotation
        Vector3 axisLocal = wigglePivot.InverseTransformDirection(axisWorld).normalized;

        float sign = 1f;
        if (randomizeLeftRight) sign = (Random.value < 0.5f) ? -1f : 1f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float n = t / Mathf.Max(0.0001f, duration);

            float decay = Mathf.Exp(-damping * n);
            float angle = Mathf.Sin(n * frequency) * amplitudeDegrees * decay * sign;

            wigglePivot.localRotation = _restLocalRot * Quaternion.AngleAxis(angle, axisLocal);
            yield return null;
        }

        wigglePivot.localRotation = _restLocalRot;
        _routine = null;
    }
}