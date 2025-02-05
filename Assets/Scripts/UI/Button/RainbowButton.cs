using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Button
{
    public class RainbowButton : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button button;

        [SerializeField] private float speed = 0.1F;
        [SerializeField] private float saturation = 1;
        [SerializeField] private float value = 1;

        private ColorBlock _colorBlock;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _colorBlock = button.colors;
            StartCoroutine(UpdateColor());
        }

        private IEnumerator UpdateColor()
        {
            while (true)
            {
                _colorBlock.normalColor =
                    Color.HSVToRGB(Mathf.PingPong(Time.unscaledTime * speed, 1), saturation, value);
                button.colors = _colorBlock;
                yield return null;
            }
        }
    }
}