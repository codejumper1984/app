using UnityEditor;
using UnityEngine;


public class ResourceExporter
{
    static string uiSaveRoot =  Application.dataPath + "/../data/ui";

    [MenuItem("Asset/Build UI For Windows And AutoSave")]
    static void BuildUI(string uiPrefabPath)
    {
        int suffixDotIdx = uiPrefabPath.LastIndexOf(".");
        string filename = uiPrefabPath.Substring(suffixDotIdx);
        string uiSavePath = uiSaveRoot + "/" + filename + ".ui";

        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;
        BuildPipeline.PushAssetDependencies();
        //接下来BuildAssetBundle的GameObject可以被之后的引用

        Object obj = AssetDatabase.LoadMainAssetAtPath(uiPrefabPath);
        BuildPipeline.BuildAssetBundle(obj, null, uiSavePath, options,BuildTarget.StandaloneWindows);
        BuildPipeline.PopAssetDependencies();
    }
}
