using UnityEngine;
using System.Collections;

public class Wiggleable : ClickableBehaviour
{
    [Header("Pivot")]
    [SerializeField] Transform wigglePivot;

    [Header("Wiggle Feel")]
    [SerializeField] float amplitudeDegrees = 3f;
    [SerializeField] float duration = 0.25f;
    [SerializeField] float oscillations = 2.5f; //How many back-and-forths within duration
    [SerializeField] float damping = 8f; //Higher = settles faster

    [Header("Direction")]
    [SerializeField] bool randomizeLeftRight = true;

    private Coroutine _routine;

    void Reset()
    {
        //Auto-setup
        wigglePivot = transform;
    }

    void Awake()
    {
        if (!wigglePivot) wigglePivot = transform;
    }

    public override void OnClick()
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
        Quaternion startRot = wigglePivot.localRotation;

        //Camera doesn't rotate, so world right is stable
        Vector3 swayDir = Vector3.right;

        //Rotation axis that produces sway in swayDir while staying upright
        Vector3 up = Vector3.up;
        Vector3 axisWorld = Vector3.Cross(up, swayDir).normalized; //Usually -forward

        //Convert world axis into the pivot's local space (once, at rest)
        Vector3 axisLocal = wigglePivot.InverseTransformDirection(axisWorld).normalized;

        float sign = 1f;
        if (randomizeLeftRight) sign = (Random.value < 0.5f) ? -1f : 1f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / Mathf.Max(0.0001f, duration));

            //Damped sinusoid
            float decay = Mathf.Exp(-damping * n);
            float angle = Mathf.Sin(n * oscillations * Mathf.PI * 2f) * amplitudeDegrees * decay * sign;

            //Critical fix: apply rotation relative to the start rotation (no accumulation)
            wigglePivot.localRotation = startRot * Quaternion.AngleAxis(angle, axisLocal);

            yield return null;
        }

        //Reset
        wigglePivot.localRotation = startRot;
        _routine = null;
    }
}