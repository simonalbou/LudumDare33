using UnityEngine;
using System.Collections;

public class Bodypart : MonoBehaviour {

	[Header("Battle Stats")]
	public int slotNumber; // 0 = head, 1 = arms, 2 = misc, 3 = legs
	public float cooldown;
	public float damage; // base damage from this attack only
	public float atkMultiplier; // atk bonus for all attacks
	public float defMultiplier; // def bonus from all attacks
	
	[Header("Physic Stats")]
	public float speedIncrease;
	public float gravityIncrease;
	public int extraAllowedJumps;
	public bool affectsJumpCurve = false;
	public AnimationCurve alternativeJumpCurve;
	private AnimationCurve oldJumpCurve;

	[Header("References")]
	public SpriteRenderer playerGraphics;
	public Animator playerAnimator;
	public PlatformingInputHandler playerScript;
	public AttackableObject playerBattleStats;

	[Header("Graphics")]
	public Sprite sprite;
	public RuntimeAnimatorController animatorController;



	public virtual void OnJustObtained()
	{
		playerGraphics.sprite = sprite;
		playerAnimator.runtimeAnimatorController = animatorController;

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
