using UnityEditor;
using UnityEngine;
using System.Collections;
using EasyEditor;
using System.IO;

namespace HYZ
{
	[Groups("Standard Audio Clips", "Custom Audio Clips", "Random Audio Clips")]
	[CustomEditor(typeof(SoundList))]
	public class SoundListEditor : EasyEditorBase
	{
        [MenuItem("Assets/Create/DPSound/SoundList", false, 2000)]
        public static void CreateSoundList()
        {
            string path = GetCurrentPath ();
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/SoundList.prefab");
            
            GameObject asset = new GameObject ("AudioSource", typeof(SoundList));
            PrefabUtility.CreatePrefab (assetPathAndName, asset);
            DestroyImmediate (asset);
        }

        static private string GetCurrentPath()
        {
            string path = AssetDatabase.GetAssetPath (Selection.activeObject);
            if (path == "") 
            {
                path = "Assets";
            } 
            else if (Path.GetExtension (path) != "") 
            {
                path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
            }
            
            return path;
        }
	}
}