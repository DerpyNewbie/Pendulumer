using System.Collections;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class RainbowTextView : MonoBehaviour
    {
        [SerializeField] private float speed = 0.1F;
        [SerializeField] private float saturation = 1;
        [SerializeField] private float value = 1;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            StartCoroutine(UpdateColor());
        }

        private void OnDisable()
        {
            StopCoroutine(UpdateColor());
        }

        private IEnumerator UpdateColor()
        {
            while (true)
            {
                _text.color = Color.HSVToRGB(Mathf.PingPong(Time.unscaledTime * speed, 1), saturation, value);
                yield return null;
            }
        }
    }
}