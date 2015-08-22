using UnityEngine;
using System.Collections;

public class BodypartIcon : MonoBehaviour {

	public Transform self;
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
		bool closeToSlot = false;

		if (closeToSlot) OnPlacedInSlot();
		else self.position = initPosition;
	}

	public void OnPlacedInSlot()
	{
		isPlacedInSlot = true;
		self.position = bodyPartButtons[bodyPart.slotNumber].self.position;
		BodypartIcon oldIcon = bodyPartButtons[bodyPart.slotNumber].currentIcon;
		oldIcon.bodyPart.OnJustLost();

		oldIcon.self.position = initPosition;
		oldIcon.initPosition = initPosition;
		bodyPartButtons[bodyPart.slotNumber].currentIcon = this;
		bodyPart.OnJustObtained();
    }
}
