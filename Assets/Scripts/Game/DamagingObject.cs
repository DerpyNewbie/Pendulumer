using UnityEngine;

namespace Game
{
    public class DamagingObject : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            other.gameObject.SendMessage("OnDamaged", this, SendMessageOptions.DontRequireReceiver);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            other.gameObject.SendMessage("OnDamaged", this, SendMessageOptions.DontRequireReceiver);
        }
    }
}