using System;
using System.Collections.Generic;
using System.Linq;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Managers;
using _01_Scripts.Game.Settings;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Scripts.Game.Core
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TextMeshPro _valueTMP;
        [SerializeField] private Collider2D _col;
        [SerializeField] private int _value;
        
        private Cell _cell;

        private int _pow = 0;
        private bool _hasFilled;

        public Color SpriteColor;
        
        private void Awake()
        {
            _col.enabled = false;
        }

        private void Start()
        {
            DoFirstScale();
        }

        void DoFirstScale()
        {
            transform.localScale = Vector3.zero;
            DOVirtual.DelayedCall(Random.Range(0f, 0.25f),()=>transform.DOScale(1, 0.2f));
        }

        public void PrepareItem(Cell cell)
        {
            _cell = cell;
            
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

        public void SetReadyToShoot()
        {
            _col.enabled = true;
        }

        void SetValue()
        {
            _pow = Random.Range(1, 10);
            _value = (int)Mathf.Pow(2, _pow);
        }
        
        void SetValue(int baseNumber, int pow)
        {
            var total = baseNumber * Mathf.Pow(2, pow - 1);
            _pow = pow;

            if (total > 2048)
                total = 2048;
            
            _value = (int)total;
        }

        public int GetValue() => _value;
        
        void SetText()
        {
            _valueTMP.text = _value.ToString();
        }

        void SetColor()
        {
            _spriteRenderer.color = CellManager.I.SetItemColor(_pow - 1);
            SpriteColor = _spriteRenderer.color;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out Cell cell))
            {
                if (cell.HasItem)
                {
                    FillNearestCell();
                }
            }
        }

        void FillNearestCell()
        {
            if (_hasFilled)
                return;

            _hasFilled = true;
            
            DOTween.Kill(transform);
            List<Cell> emptyCells = FindEmptyCells();

            var closest = GetClosestCell(emptyCells);
            FillCell(closest);
        }

        void FillCell(Cell cell)
        {
            _cell = cell;
            cell.FillWithRandomItem(this);
        }

        public void Explode()
        {
            if (DOTween.IsTweening(transform))
            {
                return;
            }
            ParticleManager.I.PlayParticle(ParticleType.Destroy, transform.position, Quaternion.identity,
                SpriteColor);
            Destroy(gameObject);
        }

        public void Fall()
        {
            GetComponent<Rigidbody2D>().gravityScale = 1.5f;
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
            
            _col.enabled = false;
            return emptyCells;
        }

        Cell GetClosestCell(List<Cell> cells)
        {
            Cell closestCell = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
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

