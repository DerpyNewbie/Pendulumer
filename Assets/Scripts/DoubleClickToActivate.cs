using Game;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleClickToActivate : MonoBehaviour
{
    [SerializeField] private MonoBehaviour monoBehaviour;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private float doubleClickTime = 0.2F;
    private InputAction _clickAction;
    private InputAction _jumpAction;
    private float _lastClickTime;

    private void Awake()
    {
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _clickAction = InputSystem.actions.FindAction("Attack");
    }

    private void Update()
    {
        if (!_jumpAction.IsPressed() || !_clickAction.WasPerformedThisFrame()) return;

        if (Time.time - _lastClickTime < doubleClickTime)
        {
            ActivateAndDestroy();
        }
        else
        {
            _lastClickTime = Time.time;
        }
    }

    private void ActivateAndDestroy()
    {
        monoBehaviour.enabled = true;
        if (monoBehaviour is HookShotAction action)
        {
            action.Controllable = true;
        }

        foreach (var obj in objects)
        {
            obj.SetActive(true);
        }

        Destroy(this);
    }
}