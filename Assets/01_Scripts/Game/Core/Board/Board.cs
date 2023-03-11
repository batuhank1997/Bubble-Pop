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

        private const int Rows = 4;
        private const int Cols = 6;

        public Cell[,] Cells = new Cell[Cols, Rows];

        public void Init()
        {
            CreateCells();
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
                    cell.PrepareCell(j, i, this);
                    cell.transform.SetParent(_cellParent);
                }
            }
        }

        Cell GetNeighbourWithDirection(Cell cell, Direction dir)
        {
            return new Cell();
        }
    }
}
