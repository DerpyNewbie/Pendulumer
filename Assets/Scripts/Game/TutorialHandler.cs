using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
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
            UpdateVisibility(PlayerConfig.ShowTutorial);
        }

        private void Start()
        {
            hookShotAction.OnActivated += () => hookShotText.text = hookShotDeactivateName;
            hookShotAction.OnDeactivated += () => hookShotText.text = hookShotActivateName;
        }

        private void Update()
        {
            var move = _moveAction.ReadValue<Vector2>();

            movementLeftSwapper?.PressAction(!Mathf.Approximately(move.x, 0) && move.x < 0);
            movementRightSwapper?.PressAction(!Mathf.Approximately(move.x, 0) && move.x > 0);
        }

        public void UpdateVisibility(bool showTutorial)
        {
            gameObject.SetActive(showTutorial);
        }
    }
}