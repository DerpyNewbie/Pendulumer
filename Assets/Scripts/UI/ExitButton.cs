using UnityEditor;
using UnityEngine;

namespace UI
{
    public class ExitButton : MonoBehaviour
    {
        public void OnButtonClicked()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}