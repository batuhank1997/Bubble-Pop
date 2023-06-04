using System;
using _Dev._Scripts.Utils;
using UnityEngine;

namespace _Dev._Scripts.Game.Managers
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