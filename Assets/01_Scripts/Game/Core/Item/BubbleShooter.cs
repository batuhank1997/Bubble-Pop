using _01_Scripts.Game.Core.Trajectory;
using _01_Scripts.Game.Managers;
using DG.Tweening;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class BubbleShooter : MonoBehaviour
    {
        [SerializeField] private Item itemPrefab;
        [SerializeField] private TrajectoryPrediction trajectory;
        [SerializeField] private float shootSpeed;


        private Item itemToShoot;
        private Item nextItemToShoot;
        private Vector3 shootPos = Vector3.zero;

        private Camera cam;
        private bool canShoot = true;

        private (Vector2, Vector2) pathPoints;

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
            if (CellManager.I.isInAction)
                return;

            if (Input.GetMouseButton(0))
            {
                shootPos = cam.ScreenToWorldPoint(Input.mousePosition);

                if (shootPos.y < -10f)
                {
                    Input.GetMouseButtonUp(0);
                    trajectory.DisableTrajectory();
                    return;
                }
                
                trajectory.EnableTrajectory();

                var originPos = transform.position;
                
                pathPoints = trajectory.Predict(transform.position, (shootPos - originPos).normalized);
                
            }
            if (Input.GetMouseButtonUp(0) && canShoot)
            {
                trajectory.DisableTrajectory();
                
                if (shootPos.y < -8.5f)
                    return;
                
                Shoot();
            }
        }

        void Shoot()
        {
            CellManager.I.isInAction = true;
            itemToShoot.Shot(shootSpeed, pathPoints);
            
            ReloadShooter();
        }

        void LoadItemToShoot()
        {
            itemToShoot = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            itemToShoot.PrepareItem(null);
        }

        void LoadNextItemToShoot()
        {
            nextItemToShoot = Instantiate(itemPrefab, transform.position + Vector3.left, Quaternion.identity);
            nextItemToShoot.PrepareItem(null);
            DOTween.Kill(nextItemToShoot);
            nextItemToShoot.transform.localScale *= 0.75f;
        }

        void ReloadShooter()
        {
            itemToShoot = nextItemToShoot;
            itemToShoot.transform.DOScale(1, 0.25f);
            itemToShoot.transform.DOMove(transform.position, 0.25f);

            LoadNextItemToShoot();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}