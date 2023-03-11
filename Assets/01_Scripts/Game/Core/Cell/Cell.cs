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
        
        public Item Item;
        public int X { get; private set; }
        public int Y { get; private set; }
        
        private bool isInFirstRow;
        public bool IsOffsetLine;
        
        private Board _board;

        public List<Cell> Neighbours = new List<Cell>();


        public void PrepareCell(int x, int y, Board board)
        {
            X = x;
            Y = y;
            IsOffsetLine = Y % 2 != 0;
            isInFirstRow = Y == 0;
            _board = board;
            
            GetNeighbours();
            
            if (y >= Board.RowLimit)
                return;
            
            Item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            Item.PrepareItem();
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
            if (Item)
                Destroy(Item.gameObject);
            
            Neighbours.Clear();
        }
        
        public void TryMerge(Cell cell)
        {
            if (Item)
            {
                var temp = Item;
                Item = null;
                
                temp.transform.SetParent(cell.transform);
                temp.transform.DOLocalMove(Vector3.zero, 0.35f).OnComplete(() => Destroy(temp.gameObject));
                HasItem = false;
            }
        }

        public void FillWithItem(Item item)
        {
            Item = item;
            HasItem = true;
            item.transform.SetParent(transform);
            item.transform.DOLocalMove(Vector3.zero,0.2f).OnComplete(()=> _board.ExplodeMatchingCells(this));
        }

        [Button]
        void HighLightNeigbours()
        {
            foreach (var n in Neighbours)
            {
                if (!DOTween.IsTweening(n.transform))
                {
                    n.transform.DOPunchScale(Vector3.one, 0.25f);
                }
            }
        }
        
        void GetNeighbours()
        {
            var upRightCell = _board.GetNeighbourWithDirection(this, Direction.UpRight);
            var upLeftCell = _board.GetNeighbourWithDirection(this, Direction.UpLeft);
            var lowRightCell = _board.GetNeighbourWithDirection(this, Direction.DownRight);
            var lowLeftCell = _board.GetNeighbourWithDirection(this, Direction.DownLeft);
            var rightCell = _board.GetNeighbourWithDirection(this, Direction.Right);
            var leftCell = _board.GetNeighbourWithDirection(this, Direction.Left);

            if (upRightCell) Neighbours.Add(upRightCell);
            if (upLeftCell) Neighbours.Add(upLeftCell);
            if (lowRightCell) Neighbours.Add(lowRightCell);
            if (lowLeftCell) Neighbours.Add(lowLeftCell);
            if (rightCell) Neighbours.Add(rightCell);
            if (leftCell) Neighbours.Add(leftCell);
        }
    }
}


