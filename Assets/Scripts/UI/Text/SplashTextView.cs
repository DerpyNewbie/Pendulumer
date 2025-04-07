using System.Collections;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class SplashTextView : MonoBehaviour
    {
        [SerializeField] private float speed = 0.1F;
        [SerializeField] private float minFontSize = 24;
        [SerializeField] private float maxFontSize = 40;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            StartCoroutine(UpdateSize());
        }

        private void OnDisable()
        {
            StopCoroutine(UpdateSize());
        }

        private IEnumerator UpdateSize()
        {
            while (true)
            {
                _text.fontSize = minFontSize +
                                 ((maxFontSize - minFontSize) * Mathf.PingPong(Time.unscaledTime * speed, 1));
                yield return null;
            }
        }
    }
}