using _Dev._Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Dev._Scripts.Game.Managers
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private int score;
        
        public void IncreaseScore(int value)
        {
            score += value;
            scoreText.text = score.ToString();
            
            if (!DOTween.IsTweening(scoreText))
                scoreText.transform.DOPunchScale(Vector3.one * 0.25f, 0.2f);
        }
    }
}

