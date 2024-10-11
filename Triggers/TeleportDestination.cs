using UnityEngine;

public class TeleportDestination : MonoBehaviour
{
	private void Start()
	{
		gameObject.layer = LayerMask.NameToLayer("Triggers");
	}
}
