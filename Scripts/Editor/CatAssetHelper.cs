using UnityEngine;
using UnityEditor;
 
public class CatAssetHelper
{
	[MenuItem("Assets/Cat Asset")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<CatAsset>();
	}
}