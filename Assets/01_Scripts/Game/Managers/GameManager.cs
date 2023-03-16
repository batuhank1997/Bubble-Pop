using System;
using _01_Scripts.Game.Core;
using _01_Scripts.Utils;
using UnityEngine;

namespace _01_Scripts.Game.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Board board;

        public static Action OnGameFail;

        private void Start()
        {
            InitGame();
        }

        void InitGame()
        {
            board.Init();
        }
    }
}