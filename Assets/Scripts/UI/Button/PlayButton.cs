using UnityEngine;

namespace UI.Button
{
    public class PlayButton : MonoBehaviour
    {
        public void OnButtonClicked()
        {
            SceneLoader.BeginTransitionToScene(SceneLoader.SceneType.Game);
        }
    }
}