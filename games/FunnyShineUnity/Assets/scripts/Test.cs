using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	GameObject obj1 = null;
	GameObject obj2 = null;
	bool bLoaded = false;
	Texture tex = null;
    IEnumerator LoadAssetBundle()
    {
        WWW www = new WWW("file://" + Application.dataPath + "/../Data/t.bundle");
        yield return www;
        if (www.assetBundle != null)
        {
            Object[] allAssets = www.assetBundle.LoadAll();
			obj1 = Instantiate(allAssets[0], new Vector3(0, 0, 0), new Quaternion()) as GameObject;
			obj2 = Instantiate(allAssets[0], new Vector3(0, 0, 100), new Quaternion()) as GameObject;
			tex = obj2.renderer.sharedMaterial.mainTexture;
			www.assetBundle.Unload(false);
        }

    }
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI()
	{
		if (GUI.Button (new Rect (0, 0, 100, 100),"Destroy")) {
			if( null != obj1)
				Destroy (obj1);
		}

		if (GUI.Button (new Rect (0, 100, 100, 100),"Destroy")) {
			if( null != obj2)
				Destroy (obj2);
		}

		if (GUI.Button (new Rect (0, 200, 100, 100),"UnLoad")) {
	
				Resources.UnloadUnusedAssets();
		}

		if(!bLoaded)
		{
			if (GUI.Button (new Rect (0, 300, 100, 100),"Load")) {
				StartCoroutine("LoadAssetBundle");
				bLoaded = true;
			}
		}
	}

}
