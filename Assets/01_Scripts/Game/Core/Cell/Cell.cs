using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Game.Enums;
using DG.Tweening;
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
            isInFirstRow = Y == 0;
            _board = board;
            
            GetNeighbours();
            
            if (y >= Board.RowLimit)
                return;
            
            _item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            _item.PrepareItem();
            HasItem = true;
        }

        public void MoveCellDownwards()
        {
            Y++;
            isInFirstRow = false;

            if (Y == Board.Rows)
            {
                ResetCell();
                transform.DOMoveY(0, 0f);
                PrepareCell(X, 0, _board);
            }
            else
            {
                transform.DOMove(Vector3.up * -0.875f, 0.35f).SetRelative(true);
            }
        }

        void ResetCell()
        {
            if (_item)
                Destroy(_item.gameObject);
            
            _neighbors.Clear();
        }

        public void FillWithItem(Item item)
        {
            _item = item;
            HasItem = true;
            item.transform.SetParent(transform);
            item.transform.DOLocalMove(Vector3.zero,0.2f);
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


