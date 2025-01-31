using UnityEngine;

namespace UI
{
    public class SpriteRendererColorChanger : MonoBehaviour
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