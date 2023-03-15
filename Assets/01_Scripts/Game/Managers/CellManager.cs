using _01_Scripts.Game.Core;
using _01_Scripts.Game.Settings;
using _01_Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _01_Scripts.Game.Managers
{
    public class CellManager : Singleton<CellManager>
    {
        [SerializeField] public CellConfig cellConfig;
        [SerializeField] public Board _board;

        private BoardTraverser _boardTraverser = new BoardTraverser();

        public Color SetItemColor(int colorIndex)
        {
           return cellConfig.itemColors[colorIndex];
        }

        [Button]
        public void TraverseBoard()
        {
            _boardTraverser.TraverseBoardAndKill(_board, _board.Cells[0, 0]);
        }

        [Button]
        public void MoveCellsUpIfLastRow()
        {
            bool hasReached = false;

            for (int i = 0; i < Board.Cols; i++)
                hasReached = _board.Cells[i, Board.Rows - 1].HasItem;
            
            print(_board.Cells[0, Board.Rows - 1].name);

            if (hasReached)
            {
                for (int i = 0; i < Board.Rows; i++)
                {
                    for (int j = 0; j < Board.Cols; j++)
                    {
                        _board.Cells[j, i].MoveCellUpwards();
                    }
                }
            }
        }
        
        public int GetRandomNumberForItemValue()
        {
            var weights = cellConfig.numberChanceWeights;
            
            int totalWeight = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                totalWeight += weights[i];
            }

            int randomNumber = Random.Range(1, totalWeight); // Get a random number between 0 and total weight

            for (int i = 1; i < weights.Length; i++)
            {
                if (randomNumber <= weights[i])
                {
                    return i;
                }
                randomNumber -= weights[i];
            }

            return 1;
        }
    }
}