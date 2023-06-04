using _Dev_Scripts.Game.Core;
using UnityEngine;

namespace _Dev._Scripts.Game.Core.Board
{
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
}