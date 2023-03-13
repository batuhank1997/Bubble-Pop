using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Settings;
using _01_Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializeField] private ParticleSettings particleSettings;
    
    public void PlayParticle(ParticleType particleType, Vector3 pos, Quaternion rot, Color color)
    {
        var particle = particleSettings.particles.First(_particle => _particle.particleType == particleType).prefab;
        var vfx = Instantiate(particle, pos, rot);
        
        var renderer = vfx.GetComponent<Renderer>();
        renderer.material.color = color;

        vfx.GetComponent<Renderer>().material = renderer.material;
        Destroy(vfx, 1.5f);
    }
    
    public void PlayTextFeedback(ParticleType particleType, Vector3 pos, Quaternion rot, int number)
    {
        var particle = particleSettings.particles.First(_particle => _particle.particleType == particleType).prefab;
        var vfx = Instantiate(particle, pos, rot);

        var tmp = vfx.GetComponent<TextMeshPro>();
        
        tmp.text = number.ToString();
        vfx.transform.DOMoveY(.75f, 0.35f).SetRelative(true).OnComplete(() =>
        {
            tmp.DOFade(0, 0.2f);
        });
        
        Destroy(vfx, 1f);
    }
}
