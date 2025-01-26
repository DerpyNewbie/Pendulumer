using UnityEngine;

namespace Title
{
    public class PlayButton : MonoBehaviour
    {
        public void OnButtonClicked()
        {
            SceneLoader.BeginTransitionToScene(SceneLoader.SceneType.Game);
        }
    }
}
