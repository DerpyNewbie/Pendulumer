using UnityEngine;

namespace Game
{
    public class CrosshairColor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;
    
        public void SetColor(Color color)
        {
            foreach (var r in spriteRenderers)
            {
                r.color = color;
            }
        }
    }
}