using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BodypartButton : MonoBehaviour {

	public int slotNumber; // 0 = head, 1 = arms, 2 = misc, 3 = legs

	// Cooldown-related vars
	public Image grayFilter;
	private float currentCooldown;

	// References
	public Bodypart currentBodypart;

	public void Start()
	{
		currentBodypart.OnJustObtained();
	}

	public void Update()
	{
		if (currentCooldown > 0)
		{
			currentCooldown -= Time.deltaTime;
			grayFilter.fillAmount = Mathf.Clamp01(currentCooldown / currentBodypart.cooldown);
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
		if (currentBodypart.cooldown > 0) currentCooldown = currentBodypart.cooldown;

		currentBodypart.OnUsingBodypart();
	}
}
