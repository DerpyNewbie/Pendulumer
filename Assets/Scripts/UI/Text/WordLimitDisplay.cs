using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Text
{
    public class WordLimitDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI wordLimitText;

        [FormerlySerializedAs("wordLimitDescripitonText")] [SerializeField]
        private TextMeshProUGUI wordLimitDescriptionText;

        [SerializeField] private int minChars = 3;
        [SerializeField] private int maxChars = 16;
        [SerializeField] private Color okColor = Color.white;
        [SerializeField] private Color badColor = Color.red;

        private TMP_InputField _inputField;

        public bool IsValid { get; private set; }

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
        }

        private void OnEnable()
        {
            _inputField.characterLimit = maxChars;
            _inputField.textComponent.color = okColor;
            _inputField.onValueChanged.AddListener(s =>
            {
                IsValid = minChars <= s.Length && maxChars >= s.Length;
                var color = IsValid ? okColor : badColor;
                wordLimitText.text = $"{s.Length}";
                wordLimitText.color = color;
                wordLimitDescriptionText.text = s.Length < minChars
                    ? "At least 3 characters are required!"
                    : s.Length > maxChars
                        ? "No more than 16 characters!"
                        : "You're good to go!";
                wordLimitDescriptionText.color = color;

                OnValidated?.Invoke(IsValid);
            });

            _inputField.onValueChanged.Invoke("");
        }

        private void OnDisable()
        {
            _inputField.onValueChanged.RemoveAllListeners();
        }

        public event Action<bool> OnValidated;
    }
}