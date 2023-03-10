using _01_Scripts.Game.Settings;
using _01_Scripts.Utils;
using UnityEngine;

namespace _01_Scripts.Game.Managers
{
    public class CellManager : Singleton<CellManager>
    {
        [SerializeField] public ColorSettings _colorSettings;

        public Color SetItemColor(int colorIndex)
        {
           return _colorSettings.itemColors[colorIndex];
        }
    }
}