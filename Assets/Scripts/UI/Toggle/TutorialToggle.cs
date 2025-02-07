using System;
using Game;
using UnityEngine;

namespace UI.Toggle
{
    public class TutorialToggle : MonoBehaviour
    {
        [SerializeField]
        private TutorialHandler tutorialHandler;

        private UnityEngine.UI.Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<UnityEngine.UI.Toggle>();
        }

        private void OnEnable()
        {
            _toggle.isOn = PlayerConfig.ShowTutorial;
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            PlayerConfig.ShowTutorial = value;
            if (tutorialHandler) tutorialHandler.UpdateVisibility(value);
        }
    }
}