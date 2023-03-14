using System;
using _01_Scripts.Utils;
using UnityEngine;

namespace _01_Scripts.Game.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private GameObject failPanel;

        private void Start()
        {
            Subscribe();
        }

        private void Unsubcribe()
        {
            GameManager.OnGameFail -= OnGameFail;
        }

        private void Subscribe()
        {
            GameManager.OnGameFail += OnGameFail;
        }

        private void OnGameFail()
        {
            failPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            Unsubcribe();
        }
    }
}