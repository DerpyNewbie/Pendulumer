using UnityEngine;

namespace UI
{
    public class PlayButton : MonoBehaviour
    {
        public void OnButtonClicked()
        {
            SceneLoader.BeginTransitionToScene(SceneLoader.SceneType.Game);
        }
    }
}
