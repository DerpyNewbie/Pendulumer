using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    public class VersionView : MonoBehaviour
    {
        private void Awake()
        {
            var buildInfo = Resources.Load<BuildInfo>("BuildInfo");
            var txt = GetComponent<TMP_Text>();
            txt.text = $"v{buildInfo.version} Build {buildInfo.count}";
        }
    }
}