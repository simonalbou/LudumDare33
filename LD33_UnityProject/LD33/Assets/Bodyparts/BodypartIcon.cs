using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BodypartIcon : MonoBehaviour {

	public Transform self;
	public Image selfImage;
	[HideInInspector]
	public Vector3 initPosition;
	public TextBoxPool textBoxPool;
	private TextBox currentTextBox;
	public Bodypart bodyPart;

	// Serializing the buttons makes getting them much easier
	public BodypartButton[] bodyPartButtons;

	[HideInInspector]
	public bool isPlacedInSlot = false;

	public void OnClicked()
	{
		if (isPlacedInSlot) return;
		currentTextBox = textBoxPool.Spawn(self.position, bodyPart.bodypartName, bodyPart.flavorText);
		gameObject.layer = LayerMask.NameToLayer("UI");
	}

	public void OnReleasedOrMoved()
	{
		if (isPlacedInSlot) return;
		currentTextBox.Unspawn();
	}

	public void OnReleased()
	{
		if (isPlacedInSlot) return;

		// vérifier la distance avec bodyPartButtons[bodyPart.slotNumber].self.position
        bool closeToSlot = Vector2.Distance(self.position, bodyPartButtons[bodyPart.slotNumber].self.position) < 0.5f;

		if (closeToSlot) OnPlacedInSlot();
		else
		{
			self.position = initPosition;
			gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	public void OnPlacedInSlot()
	{
		BodypartButton button = bodyPartButtons[bodyPart.slotNumber];

		isPlacedInSlot = true;
		self.position = button.self.position;
		BodypartIcon oldIcon = button.currentIcon;
		oldIcon.selfImage.enabled = true;
		oldIcon.bodyPart.OnJustLost();

		oldIcon.self.position = initPosition;
		oldIcon.initPosition = initPosition;
		button.selfImage.sprite = selfImage.sprite;
		selfImage.enabled = false;
		bodyPartButtons[bodyPart.slotNumber].currentIcon = this;
		bodyPart.OnJustObtained();
    }
}
