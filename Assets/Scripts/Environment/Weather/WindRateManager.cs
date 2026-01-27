using UnityEngine;

public class SetWindRate : MonoBehaviour
{
    public static float windyRate = 2.0f; //emission rate over time setting in Particle System

    [SerializeField] ParticleSystem softWindSystem;
    [SerializeField] ParticleSystem stormyWindSystem;

    void Start()
    {
        if (softWindSystem == null || stormyWindSystem == null)
        {
            print("You forgot to set a Particle System reference on the Wind Rate Manager.");
            return;
        } else
        {
            SetWindiness(windyRate);
        }
    }

    void SetWindiness(float w)
    {
        var softWind = softWindSystem.emission;
        var stormyWind = stormyWindSystem.emission;

        softWind.rateOverTime = windyRate;
        stormyWind.rateOverTime = windyRate;
    }

}
