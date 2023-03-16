using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Game.Settings
{
    [CreateAssetMenu(fileName = "CellConfig", menuName = "Scriptables/Settings/CellConfig")]
    public class CellConfig : ScriptableObject
    {
        public Color[] itemColors;
        public int[] numberChanceWeights;
    }
}

