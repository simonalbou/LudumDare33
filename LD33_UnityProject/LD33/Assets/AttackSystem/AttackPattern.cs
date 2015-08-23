using UnityEngine;
using System.Collections;

public class AttackPattern : MonoBehaviour {

	public float delayBetweenEachShot = 1;
	public Transform player;
	public BulletEmitter bulletEmitter;

	public virtual void Update ()
	{
		if(Time.time % delayBetweenEachShot < Time.deltaTime)
		{
			Fire();
		}
	}

	public virtual void Fire()
	{
		bulletEmitter.ShootTowards(player.position);
		bulletEmitter.Rotate(15);
		bulletEmitter.SimpleShoot();
		bulletEmitter.Rotate(-30);
		bulletEmitter.SimpleShoot();
	}
}
