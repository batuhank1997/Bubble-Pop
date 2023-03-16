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
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private TextMeshPro valueTMP;
        [SerializeField] private CircleCollider2D col;
        [SerializeField] private int value;
        
        public Color spriteColor;
        
        private Cell cell;
        private Rigidbody2D rb;

        private int pow = 0;
        private bool hasFilled = true;
        private float speed = 0;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            trailRenderer.enabled = false;
        }

        private void Start()
        {
            DoFirstScale();
        }

        void DoFirstScale()
        {
            if (!cell)
                return;

            transform.localScale = Vector3.zero;
            DOVirtual.DelayedCall(Random.Range(0f, 0.25f), () => transform.DOScale(1, 0.2f));
        }

        public void PrepareItem(Cell cell)
        {
            this.cell = cell;
            
            if (!cell)
                col.enabled = false;
            
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

        public void Shot(float speed, (Vector2, Vector2) pathPoints)
        {
            this.speed = speed;
             MoveSequence(pathPoints);
             
            hasFilled = false;
            
            col.enabled = true;
            col.radius = 0.5f;
            trailRenderer.enabled = true;
        }

        void MoveSequence((Vector2, Vector2) pathPoints)
        {
            CellManager.I.isInAction = true;

            transform.DOMove(pathPoints.Item1, speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.DOMove(pathPoints.Item2, speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() =>
                {
                    CellManager.I.isInAction = false;
                    FindNearestCellAndFill();
                });
            });
        }
        
        void SetValue()
        {
            pow = CellManager.I.GetRandomNumberForItemValue();
            value = (int)Mathf.Pow(2, pow);
        }
        
        void SetValue(int baseNumber, int pow)
        {
            var total = baseNumber * Mathf.Pow(2, pow - 1);
            this.pow = (int)Mathf.Log(total, 2);

            if (total > 2048)
                total = 2048;
            
            value = (int)total;
        }
        
        public int GetValue() => value;
        public List<Cell> GetEmptyNeighbours() => FindEmptyCells();
        
        void SetText()
        {
            valueTMP.text = value.ToString();
        }

        void SetColor()
        {
            spriteRenderer.color = CellManager.I.SetItemColor(pow);
            spriteColor = spriteRenderer.color;
            
            trailRenderer.startColor = spriteColor;
            trailRenderer.endColor = new Color(spriteColor.a, spriteColor.b, spriteColor.b, 0);
        }

        private void PunchNeighbours(Cell targetCell)
        {
            for (var i = 0; i < targetCell.neighbours.Count; i++)
            {
                var cell = targetCell.neighbours[i];
                
                if (!cell.hasItem)
                    continue;

                var dir = (cell.transform.position - targetCell.transform.position).normalized * 0.075f;

                cell.transform.DOMove(dir, 0.05f).SetRelative(true).SetLoops(2, LoopType.Yoyo);
            }
        }

        void FindNearestCellAndFill()
        {
            if (hasFilled)
                return;

            hasFilled = true;
            
            DOTween.Kill(transform);
            
            List<Cell> emptyCells = FindEmptyCells();

            var closest = GetClosestCell(emptyCells, transform.position);
            
            if (closest.y == Board.ColMaxLimit)
                CellManager.I.MoveCellsUp();
            
            PunchNeighbours(closest);
            FillCell(closest);
        }

        void FillCell(Cell cell)
        {
            this.cell = cell;
            col.radius = 0.5f;
            cell.FillWithItem(this);
            col.enabled = true;
        }

        public void Explode()
        {
            DOTween.Kill(transform);
            ParticleManager.I.PlayParticle(ParticleType.Destroy, transform.position, Quaternion.identity,
                spriteColor);
            
            Destroy(gameObject);
        }

        public void MoveToMerge(Cell targetCell)
        {
            transform.SetParent(targetCell.transform);
            valueTMP.DOFade(0, 0.25f);
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
                    if (cell.hasItem)
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

