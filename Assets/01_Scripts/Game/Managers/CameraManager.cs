using System;
using _01_Scripts.Utils;
using DG.Tweening;
using UnityEngine;

namespace _01_Scripts.Game.Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] private Camera mainCam;

        public void CamShake()
        {
            mainCam.transform.DOPunchPosition(Vector3.one * 0.2f, 0.35f);
        }
    }
}