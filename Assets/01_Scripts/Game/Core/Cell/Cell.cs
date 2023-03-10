using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Item _item;
        
        private int _x;
        private int _y;
        private bool isInFirstRow;
        
        private Board _board;

        private List<Cell> neighbors;


        public void PrepareCell(int x, int y, Board board)
        {
            _x = x;
            _y = y;
            _board = board;
            
            _item.PrepareItem();
        }
    }
}


