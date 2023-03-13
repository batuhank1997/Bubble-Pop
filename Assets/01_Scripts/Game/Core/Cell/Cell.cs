using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Managers;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Item itemPrefab;
        public List<Cell> Neighbours = new List<Cell>();
        public bool HasItem;

        public Item Item;

        public int X;
        public int Y;

        public bool IsVisited;
        public bool IsOffsetLine;
        private bool isInFirstRow;

        private Board _board;

        private Cell downLeftNeighbour;
        private Cell downRightNeighbour;
        
        public void PrepareCell(int x, int y, Board board, bool hasOffset)
        {
            X = x;
            Y = y;
            IsOffsetLine = hasOffset;
            isInFirstRow = Y == 0;
            _board = board;

            UpdateNeighbours();
            UpdateName();

            if (y >= Board.RowLimit)
                return;

            Item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            Item.PrepareItem(this);
            HasItem = true;
        }

        void UpdateName()
        {
            name = "Cell (X: " + X + " Y: " + Y + ")";
        }

        public void MoveCellDownwards()
        {
            Y++;

            if (Y + 1 == Board.Rows)
            {
                Neighbours.Remove(downLeftNeighbour);
                Neighbours.Remove(downRightNeighbour);
            }

            if (Y == Board.Rows)
            {
                ResetCell();
                Y = 0;
                transform.DOMoveY(0, 0f);
                Item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
                Item.PrepareItem(this);
                HasItem = true;
            }
            else
            {
                transform.DOMove(Vector3.up * -0.875f, 0.35f).SetRelative(true);
            }

            UpdateNeighbours();
            UpdateName();
        }

        void ResetCell()
        {
            if (Item)
                Destroy(Item.gameObject);

            DOTween.Kill(transform);
            Neighbours.Clear();
        }

        public void KillItem()
        {
            if (Y == 0)
                return;
            
            var temp = Item;
            Item = null;
            HasItem = false;

            temp.transform.SetParent(null);
            temp.Fall();
        }

        [Button]
        public void ExplodeNeighbours()
        {
            for (int i = 0; i < Neighbours.Count; i++)
                Neighbours[i].ExplodeCell();

            ExplodeCell();
            CameraManager.I.CamShake();
            CellManager.I.TraverseBoard();
            
            ParticleManager.I.PlayParticle(ParticleType.Blast, transform.position, Quaternion.identity,
                Item.SpriteColor);
            
            _board.GetAllCellsDown();
        }

        public void ExplodeCell()
        {
            if (Item)
                Item.Explode();
            
            HasItem = false;
        }

        public void Merge(Cell cell)
        {
            if (Item)
            {
                Item.MoveToMerge(cell);;
                Item = null;

                HasItem = false;
            }
        }

        public void FillWithCalculatedItem(int baseNumber, int pow)
        {
            Item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            Item.PrepareCalculatedItem(baseNumber, pow);

            HasItem = true;
            
            ParticleManager.I.PlayParticle(ParticleType.Destroy, transform.position, Quaternion.identity,
                Item.SpriteColor);

            StartCoroutine(TryMergeAgain());
        }

        IEnumerator TryMergeAgain()
        {
            yield return new WaitForSeconds(0.25f);
            
            ParticleManager.I.PlayTextFeedback(ParticleType.TextFeedback, transform.position, Quaternion.identity,
                Item.GetValue());

            if (Item.GetValue() == 2048)
            {
                ExplodeNeighbours();
            }
            else if (!_board.TryMergeMatchingCells(this))
            {
                CellManager.I.TraverseBoard();
                _board.GetAllCellsDown();
            }
        }

        public void FillWithRandomItem(Item item)
        {
            Item = item;
            HasItem = true;
            item.transform.SetParent(transform);
            item.transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() => _board.TryMergeMatchingCells(this));
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
        void UpdateNeighbours()
        {
            var upRightCell = _board.GetNeighbourWithDirection(this, Direction.UpRight);
            var upLeftCell = _board.GetNeighbourWithDirection(this, Direction.UpLeft);
            var lowRightCell = _board.GetNeighbourWithDirection(this, Direction.DownRight);
            var lowLeftCell = _board.GetNeighbourWithDirection(this, Direction.DownLeft);
            var rightCell = _board.GetNeighbourWithDirection(this, Direction.Right);
            var leftCell = _board.GetNeighbourWithDirection(this, Direction.Left);

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