using NUnit.Framework;
using UnityEngine;

public class SetWindRate : MonoBehaviour
{
    public static float windyRate = 2.0f; //emission rate over time setting in Particle System

    private ParticleSystem[] particleSystems;

    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void SetWindiness(float w)
    {
        foreach (var particle in particleSystems)
        {
            var emission = particle.emission;
            emission.rateOverTime = windyRate;
        }

    }
}
