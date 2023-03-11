using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Game.Enums;
using UnityEngine;

namespace _01_Scripts.Game.Settings
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
