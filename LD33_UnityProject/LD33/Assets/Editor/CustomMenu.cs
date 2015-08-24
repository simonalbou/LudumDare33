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
		GameObject playerHitbox = GameObject.FindGameObjectWithTag("PlayerHitbox");
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
				bp.playerBattleStats = playerHitbox.GetComponent<AttackableObject>();
				string graphicsTagToCheck = "";
				switch (bp.slotNumber)
				{
					case 0:
						graphicsTagToCheck = "PlayerGraphics_Blue";
						break;
					case 1:
						graphicsTagToCheck = "PlayerGraphics_Green";
						break;
					case 2:
						graphicsTagToCheck = "PlayerGraphics_Yellow";
						break;
					case 3:
						graphicsTagToCheck = "PlayerGraphics_Red";
						break;
                }

				GameObject graphicsFront = GameObject.FindGameObjectWithTag(graphicsTagToCheck);
				bp.playerGraphicsFront = graphicsFront.GetComponent<SpriteRenderer>();
				bp.playerAnimatorFront = graphicsFront.GetComponent<Animator>();

				GameObject graphicsBehind = GameObject.FindGameObjectWithTag(graphicsTagToCheck + "_Behind");
				bp.playerGraphicsBehind = graphicsBehind.GetComponent<SpriteRenderer>();
				bp.playerAnimatorBehind = graphicsBehind.GetComponent<Animator>();

				bp.playerBulletEmitters = new BulletEmitter[4];
				bp.playerBulletEmitters[0] = GameObject.FindGameObjectWithTag("Emitter_Blue").GetComponent<BulletEmitter>();
				bp.playerBulletEmitters[1] = GameObject.FindGameObjectWithTag("Emitter_Green").GetComponent<BulletEmitter>();
				bp.playerBulletEmitters[2] = GameObject.FindGameObjectWithTag("Emitter_Yellow").GetComponent<BulletEmitter>();
				bp.playerBulletEmitters[3] = GameObject.FindGameObjectWithTag("Emitter_Red").GetComponent<BulletEmitter>();

				EditorUtility.SetDirty(bp);
			}

			if(go.GetComponent<ComportementEnnemis>())
			{
				ComportementEnnemis ce = go.GetComponent<ComportementEnnemis>();
				ce.player = player.transform;
				EditorUtility.SetDirty(ce);
			}

			if(go.GetComponent<Bullet>())
			{
				Bullet bullet = go.GetComponent<Bullet>();
				bullet.VFXPool = GameObject.FindGameObjectWithTag("ParticlePool").GetComponent<ParticlePool>();
				bullet.LLScreenCorner = GameObject.FindGameObjectWithTag("BottomLeftOfCamera").transform;
				bullet.URScreenCorner = GameObject.FindGameObjectWithTag("TopRightOfCamera").transform;
				EditorUtility.SetDirty(bullet);
			}

			if(go.GetComponent<BulletEmitter>())
			{
				BulletEmitter be = go.GetComponent<BulletEmitter>();
				be.bulletGraphics = new SpriteRenderer[be.bullets.Length];
				be.bulletHitboxes = new CircleCollider2D[be.bullets.Length];
				for(int i=0; i<be.bullets.Length; i++)
				{
					be.bulletGraphics[i] = be.bullets[i].graphics;
					be.bulletHitboxes[i] = be.bullets[i].GetComponent<CircleCollider2D>();
				}
				EditorUtility.SetDirty(be);
			}

			if(go.GetComponent<AttackPattern>())
			{
				AttackPattern ap = go.GetComponent<AttackPattern>();
				ap.player = player.transform;
				EditorUtility.SetDirty(ap);
			}

			if(go.GetComponent<MonsterDeathManager>())
			{
				MonsterDeathManager mdm = go.GetComponent<MonsterDeathManager>();
				mdm.deathVFXPool = GameObject.FindGameObjectWithTag("ParticlePool_EnemyDeath").GetComponent<ParticlePool>();
				EditorUtility.SetDirty(mdm);
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
