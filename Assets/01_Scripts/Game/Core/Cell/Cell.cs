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
        
        public int X;
        public int Y;
        
        private bool isInFirstRow;
        public bool IsOffsetLine;
        
        private Board _board;

        public List<Cell> Neighbours = new List<Cell>();

        private Cell downLeftNeighbour;
        private Cell downRightNeighbour;

        public void PrepareCell(int x, int y, Board board, bool hasOffset)
        {
            X = x;
            Y = y;
            IsOffsetLine = hasOffset;
            isInFirstRow = Y == 0;
            _board = board;
            
            AddNeighbours();
            
            if (y >= Board.RowLimit)
                return;
            
            Item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            Item.PrepareItem();
            HasItem = true;
        }

        public void MoveCellDownwards()
        {
            Y++;
            
            if (Y == Board.Rows)
            {
                ResetCell();
                Y = 0;
                transform.DOMoveY(0, 0f);
                AddNeighbours();
                Item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
                Item.PrepareItem();
                HasItem = true;
            }
            else
            {
                transform.DOMove(Vector3.up * -0.875f, 0.35f).SetRelative(true);
            }
            
            /*if (Y + 1 != Board.Rows)
            {
                Y++;
                transform.DOMove(Vector3.up * -0.875f, 0.35f).SetRelative(true);
            }
            else
            {
                ResetCell();
                Y = 0;
                transform.DOMoveY(0, 0f);
                AddNeighbours();
                Item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
                Item.PrepareItem();
                HasItem = true;
            }*/
        }

        void ResetCell()
        {
            if (Item)
                Destroy(Item.gameObject);
            
            DOTween.Kill(transform);
            Neighbours.Clear();
        }
        
        public void Merge(Cell cell)
        {
            if (Item)
            {
                var temp = Item;
                Item = null;
                
                temp.transform.SetParent(cell.transform);
                temp.transform.DOLocalMove(Vector3.zero, 0.15f).OnComplete(() => Destroy(temp.gameObject));
                HasItem = false;
            }
        }
        
        public void FillWithCalculatedItem(int baseNumber,int pow)
        {
            Item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            Item.PrepareCalculatedItem(baseNumber, pow);
            HasItem = true;
            ParticleManager.I.PlayParticle(ParticleType.Destroy, transform.position, Quaternion.identity, Item.SpriteColor);
            StartCoroutine(TryMergeAgain());
        }

        IEnumerator TryMergeAgain()
        {
            yield return new WaitForSeconds(0.25f);
            
            if (!_board.TryMergeMatchingCells(this))
                _board.GetAllCellsDown();
        }

        public void FillWithRandomItem(Item item)
        {
            Item = item;
            HasItem = true;
            item.transform.SetParent(transform);
            item.transform.DOLocalMove(Vector3.zero,0.2f).OnComplete(()=> _board.TryMergeMatchingCells(this));
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
        
        [Button]
        void AddNeighbours()
        {
            var upRightCell = _board.GetNeighbourWithDirection(this, Direction.UpRight, IsOffsetLine);
            var upLeftCell = _board.GetNeighbourWithDirection(this, Direction.UpLeft, IsOffsetLine);
            var lowRightCell = _board.GetNeighbourWithDirection(this, Direction.DownRight, IsOffsetLine);
            var lowLeftCell = _board.GetNeighbourWithDirection(this, Direction.DownLeft, IsOffsetLine);
            var rightCell = _board.GetNeighbourWithDirection(this, Direction.Right, IsOffsetLine);
            var leftCell = _board.GetNeighbourWithDirection(this, Direction.Left, IsOffsetLine);
            
            downLeftNeighbour = lowLeftCell;
            downRightNeighbour = lowRightCell;

            if (upRightCell && !Neighbours.Contains(upRightCell)) Neighbours.Add(upRightCell);
            if (upLeftCell && !Neighbours.Contains(upLeftCell)) Neighbours.Add(upLeftCell);
            if (lowRightCell && !Neighbours.Contains(lowRightCell)) Neighbours.Add(lowRightCell);
            if (lowLeftCell && !Neighbours.Contains(lowLeftCell)) Neighbours.Add(lowLeftCell);
            if (rightCell && !Neighbours.Contains(rightCell)) Neighbours.Add(rightCell);
            if (leftCell && !Neighbours.Contains(leftCell)) Neighbours.Add(leftCell);
        }
    }
}


