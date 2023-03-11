using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Game.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Item itemPrefab;
        public bool HasItem;
        
        private Item _item;
        public int X { get; private set; }
        public int Y { get; private set; }
        
        private bool isInFirstRow;
        
        private Board _board;

        private List<Cell> _neighbors = new List<Cell>();


        public void PrepareCell(int x, int y, Board board)
        {
            X = x;
            Y = y;
            _board = board;
            GetNeighbours();
            
            if (y >= Board.RowLimit)
                return;
            
            _item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            _item.PrepareItem();
            HasItem = true;
        }
        
        [Button]
        void GetNeighbours()
        {
            var upRightCell = _board.GetNeighbourWithDirection(this, Direction.UpRight);
            var upLeftCell = _board.GetNeighbourWithDirection(this, Direction.UpLeft);
            var lowRightCell = _board.GetNeighbourWithDirection(this, Direction.DownRight);
            var lowLeftCell = _board.GetNeighbourWithDirection(this, Direction.DownLeft);
            var rightCell = _board.GetNeighbourWithDirection(this, Direction.Right);
            var leftCell = _board.GetNeighbourWithDirection(this, Direction.Left);

            if (upRightCell) _neighbors.Add(upRightCell);
            if (upLeftCell) _neighbors.Add(upLeftCell);
            if (lowRightCell) _neighbors.Add(lowRightCell);
            if (lowLeftCell) _neighbors.Add(lowLeftCell);
            if (rightCell) _neighbors.Add(rightCell);
            if (leftCell) _neighbors.Add(leftCell);
        }
    }
}


