using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class HarmEvent : UnityEvent<float> { }

public class AttackableObject : MonoBehaviour {

	[Header("Misc battle stats")]
	public float maxHealth = 100;
	private float curHealth;
	public float atkMultiplier = 1;
	public float defMultiplier = 1;

	[Header("References")]
	public Transform lifeBar;
	public SpriteRenderer lifeBarSprite;
	public MonsterDeathManager deathManager;

	// Who can shoot at this ?
	[Header("Collision Things")]
	public bool takesEnemyBullets = true;
	public bool takesAllyBullets = false;

	// Does this object block bullets or it is see-through ?
	public bool destroyBullet;

	public HarmEvent OnHitByBullet;

	// Invincibility things
	[HideInInspector]
	public bool isInvincible;
	private float invincibilityDuration, invincibilityTimeStamp;

	void Awake()
	{
		OnHitByBullet = new HarmEvent();
	}

	void Start()
	{
		OnHitByBullet.AddListener(Hurt);
		curHealth = maxHealth;
	}

	void Update()
	{
		if (isInvincible && Time.time > invincibilityTimeStamp + invincibilityDuration)
			isInvincible = false;

		if (Input.GetKeyDown(KeyCode.K)) Die();
	}

	public void Hurt(float dmg)
	{
		float trueDmg = dmg / defMultiplier;
		curHealth -= trueDmg;
		if (curHealth < 0) curHealth = 0;

		float lifeRatio = curHealth / maxHealth;
		lifeBar.localScale = new Vector3(lifeRatio, lifeBar.localScale.y, lifeBar.localScale.z);
		lifeBarSprite.color = new Color(1 - lifeRatio*lifeRatio, lifeRatio, 0);

		if (curHealth == 0) Die();
	}
	
	// if the death manager is not specified, we'll assume this script refers to the player, whose death leads to a game over.
	public void Die()
	{
		if (deathManager) deathManager.LaunchDeath();
		else GameOver();
	}

	public void GameOver()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// Don't even check tags if we just can't get shot.
		if (isInvincible) return;

		// We only take bullets or ally bullets, or an enemy's body.
		if (other.tag != "Attack" && other.tag != "AllyAttack" && other.tag != "Enemy") return;

		// From those two, sort out the right kind of bullets
		if (other.tag == "Attack" && !takesEnemyBullets) return;
		if (other.tag == "AllyAttack" && !takesAllyBullets) return;
		if (other.tag == "Enemy" && !takesEnemyBullets) return;

		if (other.tag != "Enemy")
		{
			Bullet bullet = other.GetComponent<Bullet>();
			if (!bullet.enabled) return;

			if (destroyBullet && !bullet.dontDestroyWhenCollided) bullet.Die();
			if (bullet.dontDestroyWhenCollided) MakeInvincible(0.5f);
			if (OnHitByBullet != null) OnHitByBullet.Invoke(bullet.power);
		}
		else
		{
			ComportementEnnemis enemyScript = other.GetComponent<ComportementEnnemis>();
			if (OnHitByBullet != null) OnHitByBullet.Invoke(enemyScript.collisionDamage);
			MakeInvincible(0.5f);
		}
	}

	// Example : when hurt by an enemy's body, it would be unfair if the player could get hurt every frame.
	public void MakeInvincible(float duration)
	{
		isInvincible = true;
		invincibilityDuration = duration;
		invincibilityTimeStamp = Time.time;
	}
}
