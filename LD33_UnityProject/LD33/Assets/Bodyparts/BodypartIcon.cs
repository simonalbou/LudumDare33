using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BodypartIcon : MonoBehaviour {

	[HideInInspector]
	public Vector3 initPosition;
	public Bodypart bodyPart;
	public float dragRadius = 0.7f; // basically, a circle collider size

	// vars used for popping a textbox
	private float moveTimestamp;
	[Header("References")]
	public TextBoxPool textBoxPool;
	private TextBox currentTextBox = null;

	// Serializing the buttons makes getting them much easier
	public Transform self;
	public SpriteRenderer selfImage;
	public BodypartButton[] bodyPartButtons;
	public TouchManager touchManager;
	public Camera mainCam;
	private int indexInManager;

	public void Start()
	{
		for(int i=0; i<touchManager.allIcons.Length; i++)
		{
			if(touchManager.allIcons[i] == this)
			{
				indexInManager = i;
				break;
			}
		}
	}

	public void Update()
	{
		int tapCount = Input.touchCount;
		if (tapCount == 0) return;

		for(int i=0; i<tapCount; i++)
		{
			Touch tch = Input.GetTouch(i);

			if (tch.phase == TouchPhase.Began)
			{
				// The drag input only begins if close enough to the icon
				if (Vector2.Distance(mainCam.ScreenToWorldPoint(tch.position), self.position) > dragRadius) continue;

				// If a finger is already registered for this drag, we don't take this one
				if (touchManager.iconTouchIds[indexInManager] != -1) continue;

				// If this finger already manages the joystick or a BodyPartIcon, we don't take it either
				if (!touchManager.IsFingerAvailableForIcon(tch.fingerId, indexInManager)) continue;

				touchManager.iconTouchIds[indexInManager] = tch.fingerId;
				moveTimestamp = Time.time;
				OnClicked();
				continue;
			}

			// After the first frame for a given input, we only use the right fingerId.
			if (tch.fingerId != touchManager.iconTouchIds[indexInManager]) break;

			// Following the touch for a drag'n'drop input :
			if (tch.phase == TouchPhase.Moved)
			{
				OnReleasedOrMoved();
				Vector3 worldTouch = mainCam.ScreenToWorldPoint(tch.position);
				self.position = new Vector3(worldTouch.x, worldTouch.y, self.position.z);
				//self.Translate(tch.deltaPosition/100);
				//Debug.Log(tch.deltaPosition / 100);
				if(tch.deltaPosition.sqrMagnitude > 0.3f) moveTimestamp = Time.time;
			}

			if (tch.phase == TouchPhase.Stationary)
			{
				if(Time.time - moveTimestamp > 0.8f && currentTextBox == null)
					currentTextBox = textBoxPool.Spawn(self.position, bodyPart.bodypartName, bodyPart.flavorText);
			}

			if (tch.phase == TouchPhase.Ended)
			{
				OnReleasedOrMoved();
				touchManager.iconTouchIds[indexInManager] = -1;
				OnReleased();
			}
		}
	}

	public void OnClicked()
	{
		gameObject.layer = LayerMask.NameToLayer("UI");
	}

	public void OnReleasedOrMoved()
	{
		if (currentTextBox) currentTextBox.Unspawn();
		currentTextBox = null;
	}

	public void OnReleased()
	{
		// vérifier la distance avec bodyPartButtons[bodyPart.slotNumber].self.position
        bool closeToSlot = Vector2.Distance(self.position, bodyPartButtons[bodyPart.slotNumber].self.position) < 0.7f;

		if (closeToSlot) OnPlacedInSlot();
		else
		{
			initPosition = self.position;
			gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	public void OnPlacedInSlot()
	{
		BodypartButton button = bodyPartButtons[bodyPart.slotNumber];

		this.enabled = false;
		self.position = button.self.position;
		BodypartIcon oldIcon = button.currentIcon;
		oldIcon.selfImage.enabled = true;
		oldIcon.bodyPart.OnJustLost();

		oldIcon.enabled = true;
		oldIcon.self.position = initPosition;
		oldIcon.initPosition = initPosition;
		oldIcon.gameObject.layer = LayerMask.NameToLayer("Default");
		button.selfImage.sprite = selfImage.sprite;
		selfImage.enabled = false;
		bodyPartButtons[bodyPart.slotNumber].currentIcon = this;
		bodyPart.OnJustObtained();
    }
}
