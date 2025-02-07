using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "buildinfo", order = 0)]
    public class BuildInfo : ScriptableObject
    {
        public string version;
        public int count;
    }
}