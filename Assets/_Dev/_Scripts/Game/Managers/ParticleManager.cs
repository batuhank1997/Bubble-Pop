using System.Linq;
using _Dev._Scripts.Game.Enums;
using _Dev._Scripts.Game.ScriptableObjects.Scriptables;
using _Dev._Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Dev._Scripts.Game.Managers
{
    public class ParticleManager : Singleton<ParticleManager>
    {
        [SerializeField] private ParticleSettings particleSettings;

        private GameObject comboTextObj;

        public void PlayParticle(ParticleType particleType, Vector3 pos, Quaternion rot, Color color)
        {
            var destroyParticle = particleSettings.particles.First(_particle => _particle.particleType == particleType)
                .prefab;
            var vfx = Lean.Pool.LeanPool.Spawn(destroyParticle);

            vfx.transform.position = pos;
            vfx.transform.rotation = rot;
            var renderer = vfx.GetComponent<Renderer>();
            renderer.material.color = color;

            vfx.GetComponent<Renderer>().material = renderer.material;
            Lean.Pool.LeanPool.Despawn(vfx, 1f);
        }

        public void PlayTextFeedback(ParticleType particleType, Vector3 pos, Quaternion rot, int number)
        {
            var textParticle = particleSettings.particles.First(_particle => _particle.particleType == particleType)
                .prefab;
            var vfx = Lean.Pool.LeanPool.Spawn(textParticle);

            var tmp = vfx.GetComponent<TextMeshPro>();
            tmp.DOFade(1, 0f);

            vfx.transform.position = pos;
            vfx.transform.rotation = rot;

            tmp.text = number.ToString();
            vfx.transform.DOMoveY(.75f, 0.35f).SetRelative(true).OnComplete(() =>
            {
                tmp.DOFade(0, 0.2f);
                Lean.Pool.LeanPool.Despawn(vfx, 0.2f);
            });
        }

        public void PlayComboTextFeedback(ParticleType particleType, int number)
        {
            if (comboTextObj)
                Lean.Pool.LeanPool.DespawnAll();

            var particle = particleSettings.particles.First(_particle => _particle.particleType == particleType).prefab;
            var vfx = Lean.Pool.LeanPool.Spawn(particle);

            vfx.transform.localScale = Vector3.zero;
            vfx.transform.position = new Vector2(2.75f, -4.75f);

            comboTextObj = vfx;

            var tmp = vfx.GetComponent<TextMeshPro>();
            tmp.DOFade(1, 0f);

            tmp.text = "X" + number;

            vfx.transform.DOScale(2, 0.4f).OnComplete(() =>
            {
                tmp.DOFade(0, 0.4f).OnComplete(() =>
                {
                    vfx.transform.localScale = Vector3.zero;
                    Lean.Pool.LeanPool.Despawn(vfx, 1f);
                });
            });
        }
    }
}