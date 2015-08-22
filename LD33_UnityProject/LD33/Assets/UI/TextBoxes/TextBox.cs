using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextBox : MonoBehaviour {

	[HideInInspector]
	public bool isAvailable = true;

	public float scaleSpeed;
	private float targetScale = 0;

	[Header("References")]
	public Transform self;
	public Image background;
	public Text title, content;

	public void Update()
	{
		if (targetScale == 0)
		{
			self.localScale = Vector3.one * (self.localScale.x - scaleSpeed * Time.deltaTime);
			if(self.localScale.x <= 0)
			{
				background.enabled = false;
				title.enabled = false;
				content.enabled = false;
				this.enabled = false;
			}
		}

		if(targetScale == 1 && self.localScale.x < 1)
		{
			self.localScale = Vector3.one * (self.localScale.x + scaleSpeed * Time.deltaTime);
			if(self.localScale.x > 1) self.localScale = Vector3.one;
		}
	}

	public void BeginSpawn()
	{
		this.enabled = true;
		background.enabled = true;
		title.enabled = true;
		content.enabled = true;
		targetScale = 1;
	}

	public void Unspawn()
	{
		targetScale = 0;
	}
}
