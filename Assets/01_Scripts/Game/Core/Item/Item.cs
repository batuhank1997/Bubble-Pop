using _01_Scripts.Game.Settings;
using UnityEngine;

namespace _01_Scripts.Game.Core
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private ColorSettings _colorSettings;
        [SerializeField] private int _value;
        
        Cell _cell;

        private int pow = 0;

        void PrepareItem()
        {
            SetValue();
            SetColor();
        }

        void SetValue()
        {
            pow = Random.Range(1, 11);
            _value = (int)Mathf.Pow(2, pow);
        }

        void SetColor()
        {
            _spriteRenderer.color = _colorSettings.itemColors[pow];
        }
    }
}

