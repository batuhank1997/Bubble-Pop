using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Game.Settings
{
    [CreateAssetMenu(fileName = "ColorSettings", menuName = "Scriptables/Settings/ColorSettings")]
    public class ColorSettings : ScriptableObject
    {
        public Color[] itemColors;
    }
}

