using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CustomMenu : Editor {

	[MenuItem("LD33/Serialize Things")]
	public static void SerializeThings()
	{
		GameObject[] all = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

		// 1) donner la textboxpool aux bodyparticon
		// 2) donner les quatre BodyPartButton à BodyPartIcon
		// 3) donner tous les morceaux du joueur à BodyPart

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject playerGraphics = GameObject.FindGameObjectWithTag("PlayerGraphics");
		TouchManager touchManager = GameObject.FindGameObjectWithTag("TouchManager").GetComponent<TouchManager>();

		TextBoxPool tbp = null;
		List<BodypartButton> buttons = new List<BodypartButton>();
		foreach (GameObject go in all)
		{
			if (go.GetComponent<TextBoxPool>())
			{
				tbp = go.GetComponent<TextBoxPool>();
            }

			if(go.GetComponent<BodypartButton>())
			{
				buttons.Add(go.GetComponent<BodypartButton>());
			}
		}
		BodypartButton[] orderedButtons = new BodypartButton[4];
		foreach(BodypartButton bb in buttons)
		{
			Debug.Log(bb.slotNumber);
			orderedButtons[bb.slotNumber] = bb;
		}

		List<BodypartIcon> allIconsAsList = new List<BodypartIcon>();

		foreach (GameObject go in all)
		{
			if (go.GetComponent<BodypartButton>())
			{
				BodypartButton bb = go.GetComponent<BodypartButton>();
				bb.textBoxPool = tbp;
				EditorUtility.SetDirty(bb);
			}

			if (go.GetComponent<BodypartIcon>())
			{
				BodypartIcon bpi = go.GetComponent<BodypartIcon>();
				allIconsAsList.Add(bpi);

				bpi.textBoxPool = tbp;
				bpi.bodyPartButtons = orderedButtons;
				bpi.mainCam = Camera.main.GetComponent<Camera>();
				bpi.touchManager = touchManager;
				EditorUtility.SetDirty(bpi);
			}

			if(go.GetComponent<Bodypart>())
			{
				Bodypart bp = go.GetComponent<Bodypart>();
				bp.playerScript = player.GetComponent<PlatformingInputHandler>();
				bp.playerBattleStats = player.GetComponent<AttackableObject>();
				string tagToCheck = "";
				switch (bp.slotNumber)
				{
					case 0:
						tagToCheck = "PlayerGraphics_Blue";
						break;
					case 1:
						tagToCheck = "PlayerGraphics_Green";
						break;
					case 2:
						tagToCheck = "PlayerGraphics_Yellow";
						break;
					case 3:
						tagToCheck = "PlayerGraphics_Red";
						break;
                }

				GameObject graphics = GameObject.FindGameObjectWithTag(tagToCheck);
				bp.playerGraphics = graphics.GetComponent<SpriteRenderer>();
				bp.playerAnimator = graphics.GetComponent<Animator>();
				EditorUtility.SetDirty(bp);
			}
		}

		touchManager.allIcons = allIconsAsList.ToArray();
		EditorUtility.SetDirty(touchManager);

		VirtualJoystick vj = GameObject.FindGameObjectWithTag("VirtualJoystick").GetComponent<VirtualJoystick>();
		vj.touchManager = touchManager;
		vj.mainCam = Camera.main.GetComponent<Camera>();
		EditorUtility.SetDirty(vj);

		PlatformingInputHandler pih = player.GetComponent<PlatformingInputHandler>();
		pih.virtualJoystick = vj;
		EditorUtility.SetDirty(pih);

		MainCameraBehaviour mcb = Camera.main.GetComponent<MainCameraBehaviour>();
		mcb.player = player.transform;
		mcb.controller = pih;
		EditorUtility.SetDirty(mcb);

		Debug.Log("Done !");
	}
}
