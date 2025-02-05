using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(Image))]
    public class ActionImageSwapper : MonoBehaviour
    {
        [SerializeField] private bool listenAction = true;
        [SerializeField] private Sprite actionSprite;
        [SerializeField] private string actionName;
        private Sprite _defaultSprite;

        private Image _image;
        private InputAction _targetAction;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _defaultSprite = _image.sprite;
            _targetAction = InputSystem.actions.FindAction(actionName);
        }

        private void OnEnable()
        {
            _defaultSprite = _image.sprite;
            _targetAction.performed += OnPerformed;
            _targetAction.canceled += OnCanceled;
        }

        private void OnDisable()
        {
            _image.sprite = _defaultSprite;
            _targetAction.performed -= OnPerformed;
            _targetAction.canceled -= OnCanceled;
        }

        private void OnPerformed(InputAction.CallbackContext ctx)
        {
            if (!listenAction) return;
            PressAction(true);
        }

        private void OnCanceled(InputAction.CallbackContext ctx)
        {
            if (!listenAction) return;
            PressAction(false);
        }

        public void PressAction(bool pressed)
        {
            _image.sprite = pressed ? actionSprite : _defaultSprite;
        }
    }
}