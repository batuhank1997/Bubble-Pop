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

        public bool IsInAction;

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
        public void CheckCellPositions()
        {
            var shouldMoveUp = false;
            var shouldMoveDown = true;

            for (int i = 0; i < Board.Cols; i++)
            {
                if (_board.Cells[i, Board.Rows - 3].HasItem)
                    shouldMoveUp = true;
                else if (_board.Cells[i, 5].HasItem)
                {
                    shouldMoveDown = false;
                }
            }
            
            if (shouldMoveUp)
                MoveCellsUp();
            else if (shouldMoveDown)
                MoveCellsDown();
        }
        
        [Button]
        public void MoveCellsDown()
        {
            //move all index by one down
            for (int i = 0; i < _board.Cells.GetLength(0); i++)
            {
                Cell temp = _board.Cells[i, _board.Cells.GetLength(1) - 1];
                
                for (int j = _board.Cells.GetLength(1) - 1; j >= 1; j--)
                {
                    _board.Cells[i, j] = _board.Cells[i, j - 1];
                }
                _board.Cells[i, 0] = temp;
            }
            
            for (int i = 0; i < Board.Rows; i++)
            {
                for (int j = 0; j < Board.Cols; j++)
                {
                    _board.Cells[j, i].MoveCellDownwards();
                }
            }
        }

        [Button]
        public void MoveCellsUp()
        {
            for (int i = 0; i < _board.Cells.GetLength(0); i++)
            {
                Cell temp = _board.Cells[i, 0];
                
                for (int j = 0; j < _board.Cells.GetLength(1) - 1; j++)
                {
                    _board.Cells[i, j] = _board.Cells[i, j + 1];
                }
                
                _board.Cells[i, _board.Cells.GetLength(1) - 1] = temp;
            }
            
            for (int i = 0; i < Board.Rows; i++)
            {
                for (int j = 0; j < Board.Cols; j++)
                {
                    _board.Cells[j, i].MoveCellUpwards();
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