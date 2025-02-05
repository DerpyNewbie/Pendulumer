using UnityEngine;

namespace UI.Button
{
    public class TransitionButton : MonoBehaviour
    {
        [SerializeField] private SceneLoader.SceneType sceneType;

        private UnityEngine.UI.Button _button;

        private void Awake()
        {
            _button = GetComponent<UnityEngine.UI.Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void OnButtonClicked()
        {
            SceneLoader.BeginTransitionToScene(sceneType);
        }
    }
}