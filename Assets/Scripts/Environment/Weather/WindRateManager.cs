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
            if (particle.gameObject.tag == "Wind")
            {
                var emission = particle.emission;
                //to do: when you add a UI slide for "windiness" set the visual to also
                //edit windyrate and rate over time and speed
                //it should also enable soft wind for the first half (increase speed) at 50, 
                // active stormy wind and increase speed until 100% (under velocity)

                //as wind increases, regardless, Precipitation should also increase linear X + with wind
                emission.rateOverTime = windyRate;
            }
        }

    }

    void SetStorminess(float s)
    {
        foreach(var particle in particleSystems)
        {
            if (particle.gameObject.tag == "Precipitation")
            {
            //todo: set a UI slider for "storminess", set visual to also adjust 
            //linear x (make it positive to move with wind), activate Noise, increase Speed
            }
        }
    }
}
