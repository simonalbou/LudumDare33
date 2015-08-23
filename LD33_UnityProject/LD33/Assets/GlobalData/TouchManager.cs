using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour {

	[HideInInspector]
	public int joystickTouchId = -1;
	[HideInInspector]
	public int iconTouchId = -1;
	
	public void Update()
	{
		int tapCount = Input.touchCount;
		if (tapCount == 0)
		{
			return;
		}
	}
}
