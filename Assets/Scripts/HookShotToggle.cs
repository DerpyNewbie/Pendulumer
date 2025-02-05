using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class HookShotToggle : MonoBehaviour
{
    private Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        _toggle.isOn = PlayerConfig.HookShot.ToggleShot;
        _toggle.onValueChanged.AddListener(value => { PlayerConfig.HookShot.ToggleShot = value; });
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveAllListeners();
    }
}