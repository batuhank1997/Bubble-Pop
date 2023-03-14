using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Managers;
using _01_Scripts.Game.Settings;
using _01_Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializeField] private ParticleSettings particleSettings;

    private GameObject comboTextObj;
    
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
    
    public void PlayComboTextFeedback(ParticleType particleType, int number)
    {
        if (comboTextObj)
            Destroy(comboTextObj);

        var particle = particleSettings.particles.First(_particle => _particle.particleType == particleType).prefab;
        var vfx = Instantiate(particle, new Vector3(2.75f, -5f, 0), Quaternion.identity);
        vfx.transform.localScale = Vector3.zero;
        
        comboTextObj = vfx;
        
        var tmp = vfx.GetComponent<TextMeshPro>();
        
        tmp.text = "X" + number;
        
        vfx.transform.DOScale(1, 0.4f).OnComplete(() =>
        {
            tmp.DOFade(0, 0.4f);
        });
        
        Destroy(vfx, 1f);
    }
}
