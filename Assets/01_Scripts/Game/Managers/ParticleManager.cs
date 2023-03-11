using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Settings;
using _01_Scripts.Utils;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializeField] private ParticleSettings particleSettings;
    
    public void PlayParticle(ParticleType particleType, Vector3 pos, Quaternion rot, Color color)
    {
        var particle = particleSettings.particles.First(_particle => _particle.particleType == particleType).prefab;
        var vfx = Instantiate(particle, pos, rot);
        
        var main = vfx.GetComponent<ParticleSystem>().main;
        main.startColor = color;
        
        Destroy(vfx, 1.5f);
    }
}