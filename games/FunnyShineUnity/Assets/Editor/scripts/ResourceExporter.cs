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
    [MenuItem("Asset/Build AssetBundle Withou MainAsset")]
    static void BuildAssetBundleWithoutMain()
    {
		string path = "Assets/prefabs/FlowCube.prefab"; 
        string savepath = EditorUtility.SaveFilePanel("Please Select Where to save the AssetBundle","", "", "bundle");
        Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
		Object obj = AssetDatabase.LoadMainAssetAtPath(path);

        BuildAsMainAssetBundleWithoutDep(obj, allAssets, savepath, BuildTarget.StandaloneWindows);     
        
    }

    [MenuItem("Asset/Build AssetBundle")]
    static void BuildAssetBundle()
    {
        string path = EditorUtility.OpenFilePanel("Please Select a Prefab to Build AssetBundle", "", "prefab");
        string savepath = EditorUtility.SaveFilePanel("Please Select Where to save the AssetBundle", "", "", "bundle");
   
        Object obj = AssetDatabase.LoadMainAssetAtPath(path);

        BuildAsMainAssetBundleWithoutDep(obj, null, savepath, BuildTarget.StandaloneWindows);

    }

    static bool BuildAsMainAssetBundleWithoutDep(Object mainAsset, Object[] allAsset, string savePath,BuildTarget buildTarget)
    {
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;

        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundle(mainAsset, allAsset, savePath, options, buildTarget);

        BuildPipeline.PopAssetDependencies();
        return true;
    }
}
