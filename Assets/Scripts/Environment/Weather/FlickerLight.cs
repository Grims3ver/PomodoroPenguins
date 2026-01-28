using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [SerializeField] float baseIntensity = 2.5f;
    [SerializeField] float flickerAmount = 0.4f;
    [SerializeField] float flickerSpeed = 2.0f;

    Light _light;

    void Awake()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        float n = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        float offset = (n - 0.5f) * 2f * flickerAmount;
        _light.intensity = Mathf.Max(0f, baseIntensity + offset);
    }
}