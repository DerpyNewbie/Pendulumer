using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ActionImageSwapper movementLeftSwapper;
    [SerializeField] private ActionImageSwapper movementRightSwapper;
    [SerializeField] private ActionImageSwapper hookShotSwapper;
    [SerializeField] private TextMeshProUGUI hookShotText;
    [SerializeField] private HookShotAction hookShotAction;
    [SerializeField] private string hookShotActivateName = "Shoot Hook Shot";
    [SerializeField] private string hookShotDeactivateName = "Release Hook Shot";

    private InputAction _moveAction;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Start()
    {
        // TODO: consider for toggle hookshot control

        hookShotAction.OnActivated += () => hookShotText.text = hookShotDeactivateName;
        hookShotAction.OnDeactivated += () => hookShotText.text = hookShotActivateName;
        gameManager.OnStateChanged +=
            (_, newState) => gameObject.SetActive(newState == GameManager.GameState.InGame);
    }

    private void Update()
    {
        var move = _moveAction.ReadValue<Vector2>();

        movementLeftSwapper.PressAction(!Mathf.Approximately(move.x, 0) && move.x < 0);
        movementRightSwapper.PressAction(!Mathf.Approximately(move.x, 0) && move.x > 0);
    }
}