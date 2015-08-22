using UnityEngine;
using System.Collections;

public class AttackableObject : MonoBehaviour {

	public float maxHealth = 100;
	private float curHealth;
	public float atkMultiplier = 1;
	public float defMultiplier = 1;

	public Transform lifeBar;

	void Start()
	{
		curHealth = maxHealth;
	}

	public void Hurt(float dmg)
	{
		curHealth -= dmg;
		if (curHealth < 0) curHealth = 0;

		lifeBar.localScale = new Vector3(curHealth / maxHealth, lifeBar.localScale.y, lifeBar.localScale.z);

		if (curHealth == 0) Die();
	}

	public void Die()
	{

	}
}
