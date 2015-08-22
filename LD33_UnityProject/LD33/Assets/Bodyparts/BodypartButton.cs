using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BodypartButton : MonoBehaviour {

	public int slotNumber; // 0 = head, 1 = arms, 2 = misc, 3 = legs

	// Cooldown-related vars
	public Image grayFilter;
	private float currentCooldown;

	// References
	public BodypartIcon currentIcon;
	public Transform self;
	public Image selfImage;

	public void Start()
	{
		currentIcon.isPlacedInSlot = true;
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
	}

	public void UseBodyPart()
	{
		if (currentCooldown > 0) return;
		if (currentIcon.bodyPart.cooldown > 0) currentCooldown = currentIcon.bodyPart.cooldown;

		currentIcon.bodyPart.OnUsingBodypart();
	}
}
