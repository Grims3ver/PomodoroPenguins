using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LightFadeToggle : MonoBehaviour, Clickable
{
    [Header("Lights")]
    [SerializeField] Light[] targetLights;
    [SerializeField] bool autoFindLightsInChildren = true;
    [SerializeField] bool includeInactiveChildren = false;

    [Header("Fade")]
    [SerializeField] float fadeDuration = 0.35f;
    [SerializeField] bool disableLightComponentAtZero = true;

    float[] _onIntensities;
    bool _isOn;
    Coroutine _fadeRoutine;

    void Reset()
    {
        if (autoFindLightsInChildren)
            targetLights = GetComponentsInChildren<Light>(includeInactiveChildren);
    }

    void Awake()
    {
        if (autoFindLightsInChildren && (targetLights == null || targetLights.Length == 0))
            targetLights = GetComponentsInChildren<Light>(includeInactiveChildren);

        if (targetLights == null) targetLights = new Light[0];

        _onIntensities = new float[targetLights.Length];

        bool anyOn = false;
        for (int i = 0; i < targetLights.Length; i++)
        {
            Light l = targetLights[i];
            if (!l) continue;

            _onIntensities[i] = Mathf.Max(0f, l.intensity);

            if (l.enabled && l.intensity > 0.001f)
                anyOn = true;
        }

        _isOn = anyOn;
    }

    //IClickable
    public void OnClick()
    {
        Toggle();
    }

    public void Toggle()
    {
        SetOn(!_isOn);
    }

    public void SetOn(bool on)
    {
        _isOn = on;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeRoutine(on));
    }

    IEnumerator FadeRoutine(bool turnOn)
    {
        if (targetLights == null || targetLights.Length == 0)
        {
            _fadeRoutine = null;
            yield break;
        }

        if (disableLightComponentAtZero && turnOn)
        {
            for (int i = 0; i < targetLights.Length; i++)
            {
                if (targetLights[i]) targetLights[i].enabled = true;
            }
        }

        float[] startIntensities = new float[targetLights.Length];
        float[] endIntensities = new float[targetLights.Length];

        for (int i = 0; i < targetLights.Length; i++)
        {
            Light l = targetLights[i];
            if (!l) continue;

            startIntensities[i] = l.intensity;
            endIntensities[i] = turnOn ? _onIntensities[i] : 0f;
        }

        float t = 0f;
        float dur = Mathf.Max(0.0001f, fadeDuration);

        while (t < dur)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / dur);

            //Smoothstep easing
            a = a * a * (3f - 2f * a);

            for (int i = 0; i < targetLights.Length; i++)
            {
                Light l = targetLights[i];
                if (!l) continue;

                l.intensity = Mathf.Lerp(startIntensities[i], endIntensities[i], a);
            }

            yield return null;
        }

        for (int i = 0; i < targetLights.Length; i++)
        {
            Light l = targetLights[i];
            if (!l) continue;

            l.intensity = endIntensities[i];

            if (disableLightComponentAtZero && !turnOn)
                l.enabled = false;
        }

        _fadeRoutine = null;
    }
}