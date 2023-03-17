using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Managers;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Item itemPrefab;
        [SerializeField] private Transform predictedItem;
        public List<Cell> Neighbours = new List<Cell>();
        public bool HasItem;

        public Item item;

        public int x;
        public int y;

        public bool isVisited;
        public bool isOffsetLine;

        private Board board;

        private Cell downLeftNeighbour;
        private Cell downRightNeighbour;

        private Cell upRightNeighbour;
        private Cell upLeftNeighbour;

        public void PrepareCell(int x, int y, Board board, bool hasOffset)
        {
            this.x = x;
            this.y = y;
            isOffsetLine = hasOffset;
            this.board = board;

            UpdateNeighbours();
            UpdateName();

            if (y >= Board.RowLimit)
                return;

            item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            item.PrepareItem(this);
            HasItem = true;
        }

        void UpdateName()
        {
            name = "Cell (X: " + x + " Y: " + y + ")";
        }

        public void MoveCellDownwards()
        {
            CellManager.I.IsInAction = true;
            y++;

            if (y + 1 == Board.Rows)
            {
                Neighbours.Remove(downLeftNeighbour);
                Neighbours.Remove(downRightNeighbour);
            }

            if (y == Board.Rows)
            {
                ResetCell();
                y = 0;
                transform.DOMoveY(0, 0f);
                item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
                item.PrepareItem(this);
                HasItem = true;
            }
            else
            {
                transform.DOMove(Vector3.up * -0.875f, 0.35f).SetRelative(true)
                    .OnComplete(() => CellManager.I.IsInAction = false);
            }

            UpdateNeighbours();
            UpdateName();
        }

        public void MoveCellUpwards()
        {
            CellManager.I.IsInAction = true;
            y--;

            if (y + 1 == 1 || y + 1 == 2)
            {
                Neighbours.Remove(upLeftNeighbour);
                Neighbours.Remove(upRightNeighbour);
            }

            if (y + 1 == 0)
            {
                y = Board.Rows - 1;

                ResetCell();
                transform.DOMoveY((-9.625f), 0f);
                HasItem = false;
            }
            else
            {
                transform.DOMove(Vector3.up * 0.875f, 0.35f).SetRelative(true)
                    .OnComplete(() => CellManager.I.IsInAction = false);
            }

            UpdateNeighbours();
            UpdateName();
        }

        void ResetCell()
        {
            if (item)
                Destroy(item.gameObject);

            DOTween.Kill(transform);
            Neighbours.Clear();
        }

        public void PredictItem()
        {
            if (DOTween.IsTweening(predictedItem))
                return;

            predictedItem.DOScale(Vector3.one, 0.2f);
        }

        public void StopPredictingItem()
        {
            DOTween.Kill(predictedItem);
            predictedItem.localScale = Vector3.zero;
        }

        public void KillItem()
        {
            if (y == 0)
                return;

            var temp = item;
            item = null;
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

            ParticleManager.I.PlayParticle(ParticleType.Blast, transform.position, Quaternion.identity,
                item.SpriteColor);
        }

        public void ExplodeCell()
        {
            if (item)
                item.Explode();

            HasItem = false;
        }

        public void Merge(Cell cell)
        {
            if (item)
            {
                item.MoveToMerge(cell);
                ;

                HasItem = false;
            }
        }

        public void FillWithCalculatedItem(int baseNumber, int pow)
        {
            item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            item.PrepareCalculatedItem(baseNumber, pow);

            HasItem = true;

            ParticleManager.I.PlayParticle(ParticleType.Destroy, transform.position, Quaternion.identity,
                item.SpriteColor);

            ComboCounter.IncreaseComboCount(this);
            StartCoroutine(TryMergeAgain());
        }

        IEnumerator TryMergeAgain()
        {
            yield return new WaitForSeconds(0.25f);

            if (!item)
                yield return null;

            ParticleManager.I.PlayTextFeedback(ParticleType.TextFeedback, transform.position, Quaternion.identity,
                item.GetValue());

            if (item.GetValue() == 2048)
                ExplodeNeighbours();

            if (!board.TryMergeMatchingCells(this))
            {
                CellManager.I.IsInAction = false;
                CellManager.I.CheckCellPositions();
                CellManager.I.TraverseBoard();
            }
        }

        public void FillWithItem(Item item)
        {
            this.item = item;
            HasItem = true;
            item.transform.SetParent(transform);
            item.transform.DOLocalMove(Vector3.zero, 0.05f).OnComplete(() =>
            {
                if (!board.TryMergeMatchingCells(this))
                    CellManager.I.IsInAction = false;
            });
        }

        //For Debugging
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
            var upRightCell = board.GetNeighbourWithDirection(this, Direction.UpRight);
            var upLeftCell = board.GetNeighbourWithDirection(this, Direction.UpLeft);
            var lowRightCell = board.GetNeighbourWithDirection(this, Direction.DownRight);
            var lowLeftCell = board.GetNeighbourWithDirection(this, Direction.DownLeft);
            var rightCell = board.GetNeighbourWithDirection(this, Direction.Right);
            var leftCell = board.GetNeighbourWithDirection(this, Direction.Left);

            downLeftNeighbour = lowLeftCell;
            downRightNeighbour = lowRightCell;
            upRightNeighbour = upRightCell;
            upLeftNeighbour = upLeftCell;

            if (lowRightCell && !Neighbours.Contains(lowRightCell)) Neighbours.Add(lowRightCell);
            if (lowLeftCell && !Neighbours.Contains(lowLeftCell)) Neighbours.Add(lowLeftCell);
            if (rightCell && !Neighbours.Contains(rightCell)) Neighbours.Add(rightCell);
            if (leftCell && !Neighbours.Contains(leftCell)) Neighbours.Add(leftCell);

            if (y == 1)
                return;
            if (upRightCell && !Neighbours.Contains(upRightCell)) Neighbours.Add(upRightCell);
            if (upLeftCell && !Neighbours.Contains(upLeftCell)) Neighbours.Add(upLeftCell);
        }
    }
}