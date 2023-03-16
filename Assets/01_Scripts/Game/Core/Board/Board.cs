using System;
using UnityEngine;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Mechanics;
using Sirenix.OdinInspector;

namespace _01_Scripts.Game.Core
{
    public class Board : MonoBehaviour
    {
        [SerializeField] Cell _cellPrefab;
        [SerializeField] Transform _cellParent;
        
        readonly MatchFinder _matchFinder = new MatchFinder();

        public const int Rows = 12;
        public const int RowLimit = 4;
        public const int ColMinLimit = 5;
        public const int ColMaxLimit = 9;
        public const int Cols = 6;

        public Cell[,] Cells = new Cell[Cols, Rows];


        public void Init()
        {
            CreateCells();
            PrepareCells();
        }

        void CreateCells()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var rowOffset = i % 2 == 0 ? 0 : 0.5f;
                    
                    var cell = Instantiate(_cellPrefab, new Vector3(j + rowOffset, -i * 0.875f, 0), Quaternion.identity);
                    
                    Cells[j, i] = cell;
                    cell.transform.SetParent(_cellParent);
                }
            }
        }

        void PrepareCells()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var rowOffset = i % 2 != 0;

                    if (Cells[j, i].HasItem)
                        return;

                    Cells[j, i].PrepareCell(j, i, this, rowOffset);
                }
            }
        }
        
        
        public bool TryMergeMatchingCells(Cell cell)
        {
            var cells = _matchFinder.FindMatches(cell, cell.Item.GetValue());
            
            if (cells.Count < 2) return false;
            
            Cell lastCell = cells[cells.Count - 1];
            Item lastCellItem = lastCell.Item;
            
            for (var i = 0; i < cells.Count; i++)
            {
                var mergeableCell = cells[i];

                mergeableCell.Merge(lastCell);
            }
            
            lastCell.FillWithCalculatedItem(lastCellItem.GetValue(), Mathf.Clamp(cells.Count, 1, 11));

            return true;
        }

        public Cell GetNeighbourWithDirection(Cell cell, Direction dir)
        {
            switch (dir)
            {
                case Direction.UpRight:
                    return (cell.Y > 0 && cell.X < Cols) ? Cells[cell.X + ((cell.IsOffsetLine && cell.X + 1 < Cols) ? 1 : 0), cell.Y - 1] : null;
                case Direction.UpLeft:
                    return cell.Y > 0 ? Cells[cell.X + ((!cell.IsOffsetLine && cell.X > 0) ? -1 : 0), cell.Y - 1] : null;
                case Direction.DownRight:
                    return ((cell.Y + 1) < Rows && cell.X < Cols) ? Cells[cell.X + ((cell.IsOffsetLine && cell.X + 1 < Cols) ? 1 : 0), cell.Y + 1] : null;
                case Direction.DownLeft:
                    return (cell.Y + 1 < Rows) ? Cells[cell.X + ( (!cell.IsOffsetLine && cell.X > 0) ? -1 : 0), cell.Y + 1] : null;
                case Direction.Right:
                    return cell.X + 1 < Cols ? Cells[cell.X + 1, cell.Y] : null;
                case Direction.Left:
                    return cell.X > 0 ? Cells[cell.X - 1, cell.Y] : null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
    }
}
