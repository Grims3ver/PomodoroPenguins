using UnityEngine;

public class TwinkleLights : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] Light[] lights;
    [SerializeField] bool autoFindInChildren = true;
    [SerializeField] bool includeInactiveChildren = false;

    [Header("Twinkle")]
    [SerializeField] float intensityMultiplier = 1f; //Overall brightness scale
    [SerializeField] Vector2 twinkleSpeedRange = new Vector2(0.6f, 1.4f); //Cycles per second-ish
    [SerializeField] Vector2 amplitudeRange = new Vector2(0.15f, 0.45f); //Fraction of base intensity
    [SerializeField] Vector2 minIntensityFactorRange = new Vector2(0.2f, 0.6f); //How dim it gets at minimum

    [Header("Timing")]
    [SerializeField] bool useUnscaledTime = true;

    float[] _baseIntensities;
    float[] _speed;
    float[] _amplitude;
    float[] _minFactor;
    float[] _phase;

    void Reset()
    {
        if (autoFindInChildren)
            lights = GetComponentsInChildren<Light>(includeInactiveChildren);
    }

    void Awake()
    {
        if (autoFindInChildren && (lights == null || lights.Length == 0))
            lights = GetComponentsInChildren<Light>(includeInactiveChildren);

        if (lights == null) lights = new Light[0];

        int n = lights.Length;
        _baseIntensities = new float[n];
        _speed = new float[n];
        _amplitude = new float[n];
        _minFactor = new float[n];
        _phase = new float[n];

        for (int i = 0; i < n; i++)
        {
            Light l = lights[i];
            if (!l) continue;

            _baseIntensities[i] = Mathf.Max(0f, l.intensity);
            _speed[i] = Random.Range(twinkleSpeedRange.x, twinkleSpeedRange.y);
            _amplitude[i] = Random.Range(amplitudeRange.x, amplitudeRange.y);
            _minFactor[i] = Random.Range(minIntensityFactorRange.x, minIntensityFactorRange.y);
            _phase[i] = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void Update()
    {
        float t = useUnscaledTime ? Time.unscaledTime : Time.time;

        for (int i = 0; i < lights.Length; i++)
        {
            Light l = lights[i];
            if (!l) continue;

            float baseI = _baseIntensities[i] * intensityMultiplier;

            //0..1 smooth wave
            float wave01 = 0.5f + 0.5f * Mathf.Sin((t * _speed[i] * Mathf.PI * 2f) + _phase[i]);

            //Convert wave into a range: [minFactor .. 1], with per-light amplitude control
            float minI = baseI * _minFactor[i];
            float maxI = baseI;

            //Amplitude controls how much it travels toward minI
            float mix = Mathf.Lerp(1f, _amplitude[i], 1f - wave01); //When wave is low, mix leans into amplitude
            float target = Mathf.Lerp(maxI, minI, mix);

            l.intensity = target;
        }
    }
}