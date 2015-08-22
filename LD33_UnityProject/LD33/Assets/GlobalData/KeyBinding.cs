using UnityEngine;
using System.Collections;

public static class KeyBinding
{
	public static KeyCode[] shootKeys = new KeyCode[] { KeyCode.Space, KeyCode.W, KeyCode.LeftShift, KeyCode.JoystickButton0 };
	public static KeyCode[] jumpKeys = new KeyCode[] { KeyCode.Z, KeyCode.UpArrow, KeyCode.Space, KeyCode.W, KeyCode.JoystickButton0 };

	public static KeyCode[] downKeys = new KeyCode[] { KeyCode.S, KeyCode.DownArrow };
	public static KeyCode[] leftKeys = new KeyCode[] { KeyCode.Q, KeyCode.A, KeyCode.LeftArrow };
	public static KeyCode[] rightKeys = new KeyCode[] { KeyCode.D, KeyCode.RightArrow };
	public static KeyCode[] upperKeys = new KeyCode[] { KeyCode.Z, KeyCode.UpArrow };
}