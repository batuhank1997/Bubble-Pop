using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class Cell : MonoBehaviour
    {
        int _x;
        int _y;
        Board _board;
        Item _item;
        bool isInFirstRow;

        List<Cell> neighbors;


        void PrepareCell()
        {
            
        }
    }
}


