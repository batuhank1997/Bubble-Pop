using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Game.Core;
using _01_Scripts.Game.Enums;
using _01_Scripts.Game.Managers;
using UnityEngine;

public static class ComboCounter
{
    private static int comboCount;
    
    public static void IncreaseComboCount(Cell cell)
    {
        cell.StopCoroutine(ResetCounter());

        comboCount++;

        if (comboCount >= 2)
            ParticleManager.I.PlayComboTextFeedback(ParticleType.ComboTextFeedback, comboCount);

        cell.StartCoroutine(ResetCounter());
    }

    static IEnumerator ResetCounter()
    {
        yield return new WaitForSeconds(2.5f);
        comboCount = 0;
    } 
}
