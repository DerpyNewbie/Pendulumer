using UnityEngine;

namespace UI.Toggle
{
    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public class HookShotToggle : MonoBehaviour
    {
        private UnityEngine.UI.Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<UnityEngine.UI.Toggle>();
        }

        private void OnEnable()
        {
            _toggle.isOn = PlayerConfig.ToggleHookShot;
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            PlayerConfig.ToggleHookShot = value;
        }
    }
}