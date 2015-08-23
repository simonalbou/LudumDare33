using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BodypartButton : MonoBehaviour {

	public int slotNumber; // 0 = head, 1 = arms, 2 = misc, 3 = legs

	// Button-related vars
	public Image grayFilter;
	private float currentCooldown;
	private float clickTimestamp;
	private bool buttonDown;

	// References
	public BodypartIcon currentIcon;
	public Transform self;
	public Image selfImage;
	public TextBoxPool textBoxPool;
	private TextBox currentTextBox = null;

	public void Start()
	{
		currentIcon.enabled = false;
		currentIcon.bodyPart.OnJustObtained();
	}

	public void Update()
	{
		if (currentCooldown > 0)
		{
			currentCooldown -= Time.deltaTime;
			grayFilter.fillAmount = Mathf.Clamp01(currentCooldown / currentIcon.bodyPart.cooldown);
		}

		if (currentCooldown < 0)
		{
			currentCooldown = 0;
			grayFilter.fillAmount = 0;
		}

		if(buttonDown && currentTextBox == null && Time.time - clickTimestamp > 0.7f)
		{
			Debug.Log("text spawns");
			currentTextBox = textBoxPool.Spawn(self.position, currentIcon.bodyPart.bodypartName, currentIcon.bodyPart.flavorText);
		}
	}

	public void OnClick()
	{
		buttonDown = true;
		clickTimestamp = Time.time;
	}
	
	public void OnRelease()
	{
		buttonDown = false;
		if (currentTextBox != null) currentTextBox.Unspawn();
		else UseBodyPart();
		currentTextBox = null;
	}

	public void UseBodyPart()
	{
		if (currentCooldown > 0) return;
		if (currentIcon.bodyPart.cooldown > 0) currentCooldown = currentIcon.bodyPart.cooldown;

		currentIcon.bodyPart.OnUsingBodypart();
	}
}
