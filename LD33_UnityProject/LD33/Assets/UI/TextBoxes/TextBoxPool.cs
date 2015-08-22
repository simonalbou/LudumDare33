using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextBoxPool : MonoBehaviour {

	public TextBox[] textBoxes;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	TextBox GetAvailableItem()
	{
		TextBox result = null;

		for(int i=0; i<textBoxes.Length; i++)
		{
			if (!textBoxes[i].enabled)
			{
				result = textBoxes[i];
				break;
			}
		}

		return result;
	}

	public TextBox Spawn(Vector3 spawnPos, string title, string content)
	{
		TextBox spawned = GetAvailableItem();

		spawned.title.text = title;
		spawned.content.text = content;
		spawned.self.position = spawnPos;
		spawned.BeginSpawn();

		return spawned;
	}
}
