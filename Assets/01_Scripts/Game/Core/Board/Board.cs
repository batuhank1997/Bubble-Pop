using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _01_Scripts.Game.Enums;

namespace _01_Scripts.Game.Core
{
    public class Board : MonoBehaviour
    {
        [SerializeField] Cell _cellPrefab;
        [SerializeField] Transform _cellParent;

        private const int Rows = 8;
        public const int RowLimit = 4;
        private const int Cols = 6;

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
                    Cells[j, i].PrepareCell(j, i, this);
                }
            }
        }

        public Cell GetNeighbourWithDirection(Cell cell, Direction dir)
        {
            switch (dir)
            {
                case Direction.UpRight:
                    return (cell.Y > 0 && cell.X + 1 < Cols) ? Cells[cell.X + 1, cell.Y - 1] : null;
                case Direction.UpLeft:
                    return cell.Y > 0 ? Cells[cell.X, cell.Y - 1] : null;
                case Direction.DownRight:
                    return ((cell.Y + 1) < Rows && cell.X + 1 < Cols) ? Cells[cell.X + 1, cell.Y + 1] : null;
                case Direction.DownLeft:
                    return cell.Y + 1 < Rows ? Cells[cell.X, cell.Y + 1] : null;
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
