using UnityEngine;

namespace DefaultNamespace
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