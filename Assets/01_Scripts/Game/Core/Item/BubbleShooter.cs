using _01_Scripts.Game.Managers;
using DG.Tweening;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class BubbleShooter : MonoBehaviour
    {
        [SerializeField] private Item itemPrefab;
        [SerializeField] private TrajactoryPrediction trajactory;
        [SerializeField] private float shootSpeed;


        private Item _itemToShoot;
        private Item _nextItemToShoot;
        private Vector3 _shootPos = Vector3.zero;

        private Camera cam;
        private bool canShoot = true;

        private void Start()
        {
            Subscribe();
            cam = Camera.main;

            LoadItemToShoot();
            LoadNextItemToShoot();
        }

        #region Subscriptions

        void Subscribe()
        {
            GameManager.OnGameFail += OnGameFail;
        }

        private void OnGameFail()
        {
            canShoot = false;
        }

        void Unsubscribe()
        {
            GameManager.OnGameFail -= OnGameFail;
        }

        #endregion

        private void Update()
        {
            GetInput();
        }

        private void GetInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                trajactory.EnableTrajectory();
            }
            if (Input.GetMouseButton(0))
            {
                _shootPos = cam.ScreenToWorldPoint(Input.mousePosition);
                var originPos = transform.position;
                
                trajactory.Predict(transform.position, (_shootPos - originPos).normalized);
            }
            if (Input.GetMouseButtonUp(0) && canShoot)
            {
                trajactory.DisableTrajectory();
                
                Shoot();
            }
        }

        void Shoot()
        {
            Transform bubble = _itemToShoot.transform;
            var dir = (new Vector3(_shootPos.x, _shootPos.y, 0) - bubble.position).normalized;
            _itemToShoot.Shot(shootSpeed, dir);

            bubble.DOMove(dir, shootSpeed).SetSpeedBased(true).SetRelative(true).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);

            ReloadShooter();
        }

        void LoadItemToShoot()
        {
            _itemToShoot = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            _itemToShoot.PrepareItem(null);
        }

        void LoadNextItemToShoot()
        {
            _nextItemToShoot = Instantiate(itemPrefab, transform.position + Vector3.left, Quaternion.identity);
            _nextItemToShoot.PrepareItem(null);
            DOTween.Kill(_nextItemToShoot);
            _nextItemToShoot.transform.localScale *= 0.75f;
        }

        void ReloadShooter()
        {
            _itemToShoot = _nextItemToShoot;
            _itemToShoot.transform.DOScale(1, 0.25f);
            _itemToShoot.transform.DOMove(transform.position, 0.25f);

            LoadNextItemToShoot();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}