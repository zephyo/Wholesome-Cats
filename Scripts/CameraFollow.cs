using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform Target;
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update ()
	{
		transform.position = new Vector3(
			Target.transform.position.x,
			Target.transform.position.y+1.85f,
			0
		);
	}
}
