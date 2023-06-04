using System.Collections;
using _01_Scripts.Game.Core;
using _Dev._Scripts.Game.Enums;
using _Dev._Scripts.Game.Managers;
using UnityEngine;

namespace _Dev._Scripts.Game.Mechanics
{
    public static class ComboCounter
    {
        private static int _comboCount;
    
        public static void IncreaseComboCount(Cell cell)
        {
            cell.StopCoroutine(ResetCounter());

            _comboCount++;

            if (_comboCount >= 2)
                ParticleManager.I.PlayComboTextFeedback(ParticleType.ComboTextFeedback, _comboCount);

            cell.StartCoroutine(ResetCounter());
        }

        static IEnumerator ResetCounter()
        {
            yield return new WaitForSeconds(2.5f);
            _comboCount = 0;
        } 
    }
}

