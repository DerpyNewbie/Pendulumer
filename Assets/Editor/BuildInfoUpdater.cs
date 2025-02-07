using DefaultNamespace;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildInfoUpdater : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var buildInfo = Resources.Load<BuildInfo>("BuildInfo");
        buildInfo.count++;
        EditorUtility.SetDirty(buildInfo);
    }
}