using UnityEngine;
using System.Collections;

public class Bodypart : MonoBehaviour {

	[Header("Flavor things")]
	public Sprite icon;
	public string bodypartName, flavorText;

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

	[Header("Attacks or Bullets")]
	public Sprite attackSprite;
	public float bulletRadius = 0.2f;
	public float bulletLifespan = 1f;
	public float bulletSpeed = 3f;
	public AnimationCurve bulletSpeedCurve = AnimationCurve.Linear(0, 1, 1, 1);
	public float bulletRotateSpeed = 0f;
	public AnimationCurve bulletRotateSpeedCurve = AnimationCurve.Linear(0, 1, 1, 1);
	public SpecialAttack specialAttack;

	[Header("Graphics - Front")]
	public Sprite characterSpriteFront;
	public RuntimeAnimatorController animatorControllerFront;

	[Header("Graphics - Behind")]
	public Sprite characterSpriteBehind;
	public RuntimeAnimatorController animatorControllerBehind;

	[Header("References")]
	public SpriteRenderer playerGraphicsFront;
	public Animator playerAnimatorFront;
	public SpriteRenderer playerGraphicsBehind;
	public Animator playerAnimatorBehind;
	public PlatformingInputHandler playerScript;
	public AttackableObject playerBattleStats;
	public BulletEmitter[] playerBulletEmitters;

	// Shortcuts
	[HideInInspector]
	public BulletEmitter currentEmitter { get { return playerBulletEmitters[slotNumber]; } }

	public virtual void OnJustObtained()
	{
		// Change graphics : front
		if (characterSpriteFront) playerGraphicsFront.sprite = characterSpriteFront;
		else playerGraphicsFront.sprite = null;
		if (animatorControllerFront) playerAnimatorFront.runtimeAnimatorController = animatorControllerFront;
		else playerAnimatorFront.runtimeAnimatorController = null;

		// Change graphics : behind
		if (characterSpriteBehind) playerGraphicsBehind.sprite = characterSpriteBehind;
		else playerGraphicsBehind.sprite = null;
		if (animatorControllerBehind) playerAnimatorBehind.runtimeAnimatorController = animatorControllerBehind;
		else playerAnimatorBehind.runtimeAnimatorController = null;

		// Change misc player stats
		playerBattleStats.atkMultiplier *= atkMultiplier;
		playerBattleStats.defMultiplier *= defMultiplier;
		playerScript.baseSpeed += speedIncrease;
		playerScript.baseGravity += gravityIncrease;
		playerScript.maxAllowedJumps += extraAllowedJumps;

		// Change attacks
		currentEmitter.ChangeBullets(attackSprite, bulletRadius);
		for(int i=0; i< currentEmitter.bullets.Length; i++)
		{
			currentEmitter.bullets[i].power = damage;
			currentEmitter.bullets[i].lifeSpan = bulletLifespan;
			currentEmitter.bullets[i].speed = bulletSpeed;
			currentEmitter.bullets[i].speedMod = bulletSpeedCurve;
			currentEmitter.bullets[i].rotateSpeed = bulletRotateSpeed;
			currentEmitter.bullets[i].rotateSpeedMod = bulletRotateSpeedCurve;
		}

		// Change jump
		if (affectsJumpCurve)
		{
			oldJumpCurve = playerScript.gravityJumpCurve;
			playerScript.gravityJumpCurve = alternativeJumpCurve;
		}

		// Other behaviour
		if (specialAttack) specialAttack.OnJustObtained();
	}

	public virtual void OnUsingBodypart()
	{
		if (makesPlayerJump) playerScript.Jump();
		if (specialAttack) specialAttack.OnUsingBodypart();
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

		if (specialAttack) specialAttack.OnJustLost();
	}
}
