using UnityEngine;
using System.Collections;

public class Bodypart : MonoBehaviour {

	[Header("Battle Stats")]
	public int slotNumber; // 0 = head, 1 = arms, 2 = misc, 3 = legs
	public float cooldown;
	public float damage = 1; // base damage from this attack only
	public float atkMultiplier = 1; // atk bonus for all attacks
	public float defMultiplier = 1; // def bonus from all attacks
	
	[Header("Physic Stats")]
	public float speedIncrease;
	public float gravityIncrease;
	public int extraAllowedJumps;
	public bool makesPlayerJump = false;
	public bool affectsJumpCurve = false;
	public AnimationCurve alternativeJumpCurve;
	private AnimationCurve oldJumpCurve;

	[Header("Graphics - Front")]
	public Sprite characterSpriteFront;
	public RuntimeAnimatorController animatorControllerFront;

	[Header("Graphics - Behind")]
	public Sprite characterSpriteBehind;
	public RuntimeAnimatorController animatorControllerBehind;

	[Header("Flavor things")]
	public Sprite icon;
	public string bodypartName, flavorText;

	[Header("References")]
	public SpriteRenderer playerGraphicsFront;
	public Animator playerAnimatorFront;
	public SpriteRenderer playerGraphicsBehind;
	public Animator playerAnimatorBehind;
	public PlatformingInputHandler playerScript;
	public AttackableObject playerBattleStats;

	public virtual void OnJustObtained()
	{
		if (characterSpriteFront) playerGraphicsFront.sprite = characterSpriteFront;
		else playerGraphicsFront.sprite = null;
		if (animatorControllerFront) playerAnimatorFront.runtimeAnimatorController = animatorControllerFront;
		else playerAnimatorFront.runtimeAnimatorController = null;

		if (characterSpriteBehind) playerGraphicsBehind.sprite = characterSpriteBehind;
		else playerGraphicsBehind.sprite = null;
		if (animatorControllerBehind) playerAnimatorBehind.runtimeAnimatorController = animatorControllerBehind;
		else playerAnimatorBehind.runtimeAnimatorController = null;

		playerBattleStats.atkMultiplier *= atkMultiplier;
		playerBattleStats.defMultiplier *= defMultiplier;
		playerScript.baseSpeed += speedIncrease;
		playerScript.baseGravity += gravityIncrease;
		playerScript.maxAllowedJumps += extraAllowedJumps;

		if (affectsJumpCurve)
		{
			oldJumpCurve = playerScript.gravityJumpCurve;
			playerScript.gravityJumpCurve = alternativeJumpCurve;
		}
	}

	public virtual void OnUsingBodypart()
	{
		if (makesPlayerJump) playerScript.Jump();
	}

	public virtual void OnJustLost()
	{
		playerBattleStats.atkMultiplier /= atkMultiplier;
		playerBattleStats.defMultiplier /= defMultiplier;

		playerScript.baseSpeed -= speedIncrease;
		playerScript.baseGravity -= gravityIncrease;
		playerScript.maxAllowedJumps -= extraAllowedJumps;

		if (affectsJumpCurve)
		{
			playerScript.gravityJumpCurve = oldJumpCurve;
		}
	}
}
