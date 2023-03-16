using System.Collections.Generic;
using System.Linq;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Scripts.Game.Core
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
            // _col.enabled = false;

            SetValue(baseNumber, pow);
            SetColor();
            SetText();
        }

        public void Shot(float speed, Vector3 dir, (Vector2, Vector2) pathPoints)
        {
            _speed = speed;
             MoveSequence(pathPoints);
             
            _hasFilled = false;
            
            _col.enabled = true;
            _col.radius = 0.5f;
            _trailRenderer.enabled = true;
        }

        void MoveSequence((Vector2, Vector2) pathPoints)
        {
            var seq = DOTween.Sequence();
            transform.DOMove(pathPoints.Item1, _speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.DOMove(pathPoints.Item2, _speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(FindNearestCellAndFill);
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
            _pow = (int)Mathf.Log(total, 2);

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
            _spriteRenderer.color = CellManager.I.SetItemColor(_pow);
            SpriteColor = _spriteRenderer.color;
            
            _trailRenderer.startColor = SpriteColor;
            _trailRenderer.endColor = new Color(SpriteColor.a, SpriteColor.b, SpriteColor.b, 0);
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
            
            List<Cell> emptyCells = FindEmptyCells();

            var closest = GetClosestCell(emptyCells, transform.position);
            
            if (closest.Y == Board.Rows - 3)
                CellManager.I.MoveCellsUp();

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
            transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(Explode);
            _valueTMP.DOFade(0, 0.2f);
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

