using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Game.Core;
using UnityEngine;

public class FallItemDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out Item itemToDestroy))
        {
            itemToDestroy.Explode();
        }
    }
}
