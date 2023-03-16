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
        public List<Cell> neighbours = new List<Cell>();
        public bool hasItem;

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
            hasItem = true;
        }

        void UpdateName()
        {
            name = "Cell (X: " + x + " Y: " + y + ")";
        }

        public void MoveCellDownwards()
        {
            CellManager.I.isInAction = true;
            y++;

            if (y + 1 == Board.Rows)
            {
                neighbours.Remove(downLeftNeighbour);
                neighbours.Remove(downRightNeighbour);
            }

            if (y == Board.Rows)
            {
                ResetCell();
                y = 0;
                transform.DOMoveY(0, 0f);
                item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
                item.PrepareItem(this);
                hasItem = true;
            }
            else
            {
                transform.DOMove(Vector3.up * -0.875f, 0.35f).SetRelative(true)
                    .OnComplete(() => CellManager.I.isInAction = false);
            }

            UpdateNeighbours();
            UpdateName();
        }

        public void MoveCellUpwards()
        {
            CellManager.I.isInAction = true;
            y--;

            if (y + 1 == 1 || y + 1 == 2)
            {
                neighbours.Remove(upLeftNeighbour);
                neighbours.Remove(upRightNeighbour);
            }

            if (y + 1 == 0)
            {
                y = Board.Rows - 1;

                ResetCell();
                transform.DOMoveY((-9.625f), 0f);
                hasItem = false;
            }
            else
            {
                transform.DOMove(Vector3.up * 0.875f, 0.35f).SetRelative(true)
                    .OnComplete(() => CellManager.I.isInAction = false);
            }

            UpdateNeighbours();
            UpdateName();
        }

        void ResetCell()
        {
            if (item)
                Destroy(item.gameObject);

            DOTween.Kill(transform);
            neighbours.Clear();
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
            hasItem = false;

            temp.transform.SetParent(null);
            temp.Fall();
        }

        [Button]
        public void ExplodeNeighbours()
        {
            for (int i = 0; i < neighbours.Count; i++)
                neighbours[i].ExplodeCell();

            ExplodeCell();
            CameraManager.I.CamShake();

            ParticleManager.I.PlayParticle(ParticleType.Blast, transform.position, Quaternion.identity,
                item.spriteColor);
        }

        public void ExplodeCell()
        {
            if (item)
                item.Explode();

            hasItem = false;
        }

        public void Merge(Cell cell)
        {
            if (item)
            {
                item.MoveToMerge(cell);
                ;

                hasItem = false;
            }
        }

        public void FillWithCalculatedItem(int baseNumber, int pow)
        {
            item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);
            item.PrepareCalculatedItem(baseNumber, pow);

            hasItem = true;

            ParticleManager.I.PlayParticle(ParticleType.Destroy, transform.position, Quaternion.identity,
                item.spriteColor);

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
                CellManager.I.isInAction = false;
                CellManager.I.CheckCellPositions();
                CellManager.I.TraverseBoard();
            }
        }

        public void FillWithItem(Item item)
        {
            this.item = item;
            hasItem = true;
            item.transform.SetParent(transform);
            item.transform.DOLocalMove(Vector3.zero, 0.05f).OnComplete(() =>
            {
                if (!board.TryMergeMatchingCells(this))
                    CellManager.I.isInAction = false;
            });
        }

        //For Debugging
        [Button]
        void HighLightNeigbours()
        {
            foreach (var n in neighbours)
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

            if (lowRightCell && !neighbours.Contains(lowRightCell)) neighbours.Add(lowRightCell);
            if (lowLeftCell && !neighbours.Contains(lowLeftCell)) neighbours.Add(lowLeftCell);
            if (rightCell && !neighbours.Contains(rightCell)) neighbours.Add(rightCell);
            if (leftCell && !neighbours.Contains(leftCell)) neighbours.Add(leftCell);

            if (y == 1)
                return;
            if (upRightCell && !neighbours.Contains(upRightCell)) neighbours.Add(upRightCell);
            if (upLeftCell && !neighbours.Contains(upLeftCell)) neighbours.Add(upLeftCell);
        }
    }
}