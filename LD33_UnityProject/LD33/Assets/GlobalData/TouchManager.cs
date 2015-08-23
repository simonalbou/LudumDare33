using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour {

	[HideInInspector]
	public int joystickTouchId = -1;
	[HideInInspector]
	public int[] iconTouchIds;
	public BodypartIcon[] allIcons;

	public void Awake()
	{
		iconTouchIds = new int[allIcons.Length];
		for(int i=0; i<allIcons.Length; i++)
		{
			iconTouchIds[i] = -1;
		}
	}

	public bool IsFingerAvailableForJoystick(int fingerId)
	{
		for(int i=0; i<iconTouchIds.Length; i++)
		{
			if (iconTouchIds[i] == fingerId) return false;
		}
		return true;
	}

	public bool IsFingerAvailableForIcon(int fingerId, int iconId)
	{
		if (joystickTouchId == iconId) return false;
		for (int i = 0; i < iconTouchIds.Length; i++)
		{
			if (i == iconId) continue;
			if (iconTouchIds[i] == fingerId) return false;
		}
		return true;
	}
}
