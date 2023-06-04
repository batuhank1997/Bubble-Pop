using System;
using _Dev._Scripts.Game.Enums;
using UnityEngine;

namespace _Dev._Scripts.Game.ScriptableObjects.Scriptables
{
    [CreateAssetMenu(fileName = "ParticleSettings", menuName = "Scriptables/Settings/ParticleSettings")]
    public class ParticleSettings : ScriptableObject
    {
        public Particle[] particles;
    }

    [Serializable]
    public class Particle
    {
        public GameObject prefab;
        public ParticleType particleType;
    }
}
