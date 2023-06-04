using System.Collections.Generic;
using System.Linq;
using _01_Scripts.Game.Core;
using _Dev._Scripts.Game.Enums;
using _Dev._Scripts.Game.Managers;
using _Dev._Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Dev_Scripts.Game.Core
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private TextMeshPro _valueTMP;
        [SerializeField] private CircleCollider2D _col;
        [SerializeField] private int _value;
        
        private Cell _cell;
        private Rigidbody2D rb;

        private int _pow = 0;
        private bool _hasFilled = true;
        private float _speed = 0;
        private Vector3 _dir;

        public Color SpriteColor;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            _trailRenderer.enabled = false;
        }

        private void Start()
        {
            DoFirstScale();
        }

        void DoFirstScale()
        {
            if (!_cell)
                return;

            transform.localScale = Vector3.zero;
            DOVirtual.DelayedCall(Random.Range(0f, 0.25f), () => transform.DOScale(1, 0.2f));
        }

        public void PrepareItem(Cell cell)
        {
            _cell = cell;
            
            if (!cell)
                _col.enabled = false;
            
            SetValue();
            SetColor();
            SetText();
        }
        public void PrepareCalculatedItem(int baseNumber,int pow)
        {
            SetValue(baseNumber, pow);
            SetColor();
            SetText();
        }

        public void Shot(float speed, Vector3 dir, (Cell, Vector2, Vector2) trajectoryData)
        {
            _hasFilled = false;
            
            _speed = speed;
            _dir = dir;

            if (!trajectoryData.Item1)
                PhysicsMovement();
            else
                TweenMovement((trajectoryData.Item2, trajectoryData.Item3));
            
            _trailRenderer.enabled = true;
        }

        void PhysicsMovement()
        {
            Debug.Log("PHYSICS");
            _col.radius = 0.25f;
            _col.enabled = true;

            rb.velocity = _dir * _speed;
        }

        void TweenMovement((Vector2, Vector2) pathPoints)
        {
            Debug.Log("TWEEN");

            _col.enabled = false;
            
            transform.DOMove(pathPoints.Item1, _speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.DOMove(pathPoints.Item2, _speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() =>
                {
                    CellManager.I.IsInAction = false;
                    _col.enabled = true;
                    
                    FindNearestCellAndFill();
                });
            });
        }

        void SetValue()
        {
            _pow = CellManager.I.GetRandomNumberForItemValue();
            _value = (int)Mathf.Pow(2, _pow);
        }
        
        void SetValue(int baseNumber, int pow)
        {
            var total = baseNumber * Mathf.Pow(2, pow - 1);
            _pow = Mathf.Clamp((int)Mathf.Log(total, 2), 1, 11);

            if (total > 2048)
                total = 2048;
            
            _value = (int)total;
        }
        
        public int GetValue() => _value;
        public List<Cell> GetEmptyNeighbours() => FindEmptyCells();
        
        void SetText()
        {
            _valueTMP.text = _value.ToString();
        }

        void SetColor()
        {
            
            print(_pow);
            _spriteRenderer.color = CellManager.I.SetItemColor(_pow);
            SpriteColor = _spriteRenderer.color;
            
            _trailRenderer.startColor = SpriteColor;
            _trailRenderer.endColor = new Color(SpriteColor.a, SpriteColor.b, SpriteColor.b, 0);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out Cell cell) && !_hasFilled)
            {
                if (cell.HasItem)
                    FindNearestCellAndFill();
            }
            if (col.CompareTag(Keys.TAG_EDGE) && !_cell && !_hasFilled)
            {
                var touchPoint = col.ClosestPoint(transform.position);
                BounceFromEdge(touchPoint);
            }
        }

        private void BounceFromEdge(Vector2 touchPoint)
        {
            DOTween.Kill(transform);

            var normal = ((touchPoint - Vector2.right) - touchPoint).normalized;
            var bouncedDir = Vector3.Reflect(_dir.normalized, normal);
            
            transform.DOMove(bouncedDir, _speed).SetSpeedBased(true).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            
            _dir = bouncedDir;
        }

        private void PunchNeighbours(Cell targetCell)
        {
            for (var i = 0; i < targetCell.Neighbours.Count; i++)
            {
                var cell = targetCell.Neighbours[i];
                
                if (!cell.HasItem)
                    continue;

                var dir = (cell.transform.position - targetCell.transform.position).normalized * 0.075f;

                cell.transform.DOMove(dir, 0.05f).SetRelative(true).SetLoops(2, LoopType.Yoyo);
            }
        }

        void FindNearestCellAndFill()
        {
            if (_hasFilled)
                return;

            _hasFilled = true;
            
            DOTween.Kill(transform);
            rb.velocity = Vector2.zero;
            
            List<Cell> emptyCells = FindEmptyCells();

            var closest = GetClosestCell(emptyCells, transform.position);
            PunchNeighbours(closest);
            FillCell(closest);
        }

        void FillCell(Cell cell)
        {
            _cell = cell;
            _col.radius = 0.5f;
            cell.FillWithItem(this);
            _col.enabled = true;
        }

        public void Explode()
        {
            DOTween.Kill(transform);
            ParticleManager.I.PlayParticle(ParticleType.Destroy, transform.position, Quaternion.identity,
                SpriteColor);
            
            Destroy(gameObject);
        }

        public void MoveToMerge(Cell targetCell)
        {
            transform.SetParent(targetCell.transform);
            _valueTMP.DOFade(0, 0.25f);
            transform.DOLocalMove(Vector3.zero, 0.15f).OnComplete(Explode);
            CellManager.I.TraverseBoard();
        }

        public void Fall()
        {
            rb.gravityScale = 1.5f;
            rb.AddForce(new Vector2(Random.Range(-2.5f, 2.5f), 0), ForceMode2D.Impulse);
        }
        
        List<Cell> FindEmptyCells()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1.5f).ToArray();

            List<Cell> emptyCells = new List<Cell>();
            
            foreach (var col in cols)
            {
                if (col.TryGetComponent(out Cell cell))
                {
                    if (cell.HasItem)
                        continue;
                    
                    emptyCells.Add(cell);
                }
            }
            
            return emptyCells;
        }

        public Cell GetClosestCell(List<Cell> cells, Vector3 to)
        {
            Cell closestCell = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = to;
            foreach (Cell cell in cells)
            {
                float dist = Vector3.Distance(cell.transform.position, currentPos);
                if (dist < minDist)
                {
                    closestCell = cell;
                    minDist = dist;
                }
            }
            
            return closestCell;
        }
    }
}

