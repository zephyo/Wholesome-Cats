using UnityEngine;
using UnityEditor;
 
public class StageAssetHelper
{
	[MenuItem("Assets/Stage Asset")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<StageAsset>();
	}
}