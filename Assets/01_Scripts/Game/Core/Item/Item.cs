using _01_Scripts.Game.Managers;
using _01_Scripts.Game.Settings;
using TMPro;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TextMeshPro _valueTMP;
        [SerializeField] private int _value;
        
        private Cell _cell;

        private int pow = 0;

        public void PrepareItem()
        {
            SetValue();
            SetColor();
            SetText();
        }

        void SetValue()
        {
            pow = Random.Range(1, 10);
            _value = (int)Mathf.Pow(2, pow);
        }
        
        void SetText()
        {
            _valueTMP.text = _value.ToString();
        }

        void SetColor()
        {
            _spriteRenderer.color = CellManager.I.SetItemColor(pow - 1);
        }
    }
}

