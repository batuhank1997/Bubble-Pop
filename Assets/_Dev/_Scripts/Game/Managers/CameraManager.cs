using _Dev._Scripts.Utils;
using DG.Tweening;
using UnityEngine;

namespace _Dev._Scripts.Game.Managers
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