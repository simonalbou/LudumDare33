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

	[Header("References")]
	public SpriteRenderer playerGraphics;
	public Animator playerAnimator;
	public PlatformingInputHandler playerScript;
	public AttackableObject playerBattleStats;

	[Header("Graphics")]
	public Sprite characterSprite;
	public Sprite icon;
	public RuntimeAnimatorController animatorController;
	public string bodypartName, flavorText;


	public virtual void OnJustObtained()
	{
		if(characterSprite) playerGraphics.sprite = characterSprite;
		if(animatorController) playerAnimator.runtimeAnimatorController = animatorController;

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
