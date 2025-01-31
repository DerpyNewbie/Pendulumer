using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class RainbowTextUGUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField] private float speed = 0.1F;
        [SerializeField] private float saturation = 1;
        [SerializeField] private float value = 1;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            StartCoroutine(UpdateColor());
        }

        private IEnumerator UpdateColor()
        {
            while (true)
            {
                text.color = Color.HSVToRGB(Mathf.PingPong(Time.unscaledTime * speed, 1), saturation, value);
                yield return null;
            }
        }
    }
}