using _01_Scripts.Game.Core;
using _01_Scripts.Game.Settings;
using _01_Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _01_Scripts.Game.Managers
{
    public class CellManager : Singleton<CellManager>
    {
        [SerializeField] public ColorSettings _colorSettings;
        [SerializeField] public Board _board;

        private BoardTraverser _boardTraverser = new BoardTraverser();

        public Color SetItemColor(int colorIndex)
        {
           return _colorSettings.itemColors[colorIndex];
        }

        [Button]
        public void TraverseBoard()
        {
            _boardTraverser.TraverseBoardAndKill(_board);
        }
    }
}