using System;
using UnityEngine;

namespace Utilities
{
    public class DestroyAfter : MonoBehaviour
    {
        public void Set(float duration)
        {
            Invoke(nameof(Trigger), duration);
        }

        private void Trigger()
        {
            Destroy(gameObject);
        }
    }
}