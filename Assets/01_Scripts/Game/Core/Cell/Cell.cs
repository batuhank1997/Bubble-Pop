using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Item _item;
        int _x;
        int _y;
        bool isInFirstRow;
        
        Board _board;

        List<Cell> neighbors;


        public void PrepareCell()
        {
            // _item
        }
    }
}


