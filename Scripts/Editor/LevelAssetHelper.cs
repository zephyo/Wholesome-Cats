using UnityEngine;
using UnityEditor;
 
public class LevelAssetHelper
{
	[MenuItem("Assets/Level Asset")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<LevelAsset>();
	}
}