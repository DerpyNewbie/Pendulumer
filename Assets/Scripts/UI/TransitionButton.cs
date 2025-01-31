using UnityEngine;

namespace UI
{
    public class TransitionButton : MonoBehaviour
    {
        [SerializeField] private SceneLoader.SceneType sceneType;
        
        public void OnButtonClicked()
        {
            SceneLoader.BeginTransitionToScene(sceneType);
        }
    }
}