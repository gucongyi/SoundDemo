using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using EasyEditor;

namespace HYZ
{
	[Groups("Sound Basic Parameters", "Sound Customization", "Specific Source")]
	[CustomEditor(typeof(SoundInfo))]
	[CanEditMultipleObjects]
	public class SoundInfoEditor : EasyEditorBase
	{
		[MenuItem("Assets/Create/DPSound/SoundInfo", false, 2000)]
		public static void CreateSoundHolder()
		{
			SoundInfo asset = ScriptableObject.CreateInstance<SoundInfo> ();
			ProjectWindowUtil.CreateAsset(asset, "New " + typeof(SoundInfo).Name + ".asset");
		}

		[MenuItem("Assets/Create/DPSound/AudioSource", false, 2000)]
		public static void CreateAudioSource()
		{
			string path = GetCurrentPath ();
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/AudioSource.prefab");

			GameObject asset = new GameObject ("AudioSource", typeof(AudioSource));
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