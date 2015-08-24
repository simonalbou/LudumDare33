using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Bullet))]
public class BulletEmitter : MonoBehaviour {

	#region properties

	[Header("References")]
	public Transform self;
	public Transform target;
	public Bullet selfBullet;
	public Bullet[] bullets;
	public CircleCollider2D[] bulletHitboxes;
	public SpriteRenderer[] bulletGraphics;
	public BulletEmitter[] subEmitters;
	
	[Header("Default Behaviour")]
	public bool continuousShooting; // enables default automatic behaviour
	public float shootTimeInterval; // used when continuousShooting is enabled
    public bool mustBeActiveToShoot = true;

	[HideInInspector]
	public bool canShoot = true; // set to false when the enemy is shutdown, for instance

	[Header("Bullet Stats")]
	public float bulletSpeed;
	public AnimationCurve bulletSpeedCurve = AnimationCurve.Linear(0, 1, 1, 1);
	public float bulletRotateSpeed;
	public AnimationCurve bulletRotateSpeedCurve = AnimationCurve.Linear(0, 1, 1, 1);

	#endregion

	// Continuous shooting means "default automatic behaviour"
	void Update ()
	{
		if (continuousShooting)
		{
			if (Time.time % shootTimeInterval < Time.deltaTime)
			{
				if (!target) SimpleShoot();
				else ShootTowards(target.position);
			}
		}
	}

	// Ready a new bullet and shoot it, given an orientation, a start position, and the parenting.
	// Default settings : SimpleShoot() fires a bullet in the same direction the emitter looks to, from its position.
	public void SimpleShoot(float startRotation=0, bool mirror=false, Vector3 startPosition=default(Vector3), bool isChild=false)
	{
		if (mustBeActiveToShoot && !selfBullet.enabled) return;
		if (!canShoot) return;

		Bullet temp = NewBullet();
		if (temp == null)
		{
			Debug.LogWarning(name + " : Not enough bullets !");
			return;
		}

		if (mirror)
		{
			temp.isMirrored = true;
			startPosition.x *= -1;
			startRotation *= -1;
		}
		else temp.isMirrored = false;

		temp.self.position = self.position + startPosition;
		temp.self.eulerAngles = Vector3.forward * (self.eulerAngles.z+startRotation);
		if (isChild) temp.self.parent = self;
		else temp.self.parent = null;

		temp.GetShot();
	}
	
	// Ready a new bullet and shoot it towards something. Start position and parenting are also given.
	public void ShootTowards(Vector3 destination, bool mirror = false, Vector3 startPosition = default(Vector3), bool isChild = false)
	{
        if (!canShoot) return;
        if (mustBeActiveToShoot && !selfBullet.enabled) return;

		selfBullet.LookAt(destination);

		SimpleShoot(0, mirror, startPosition, isChild);
	}

	// Returns the first available bullet in the library array.
	public Bullet NewBullet()
	{
		Bullet result = null;

		if(bullets.Length == 0) return null;

		for (int i = 0; i < bullets.Length; i++)
		{
			if (bullets[i].enabled) continue;

			result = bullets[i];
			break;
		}

		return result;
	}

    // Shuts down every sub-emitter of this emitter.
	public void KillEmitters()
	{
		if(subEmitters.Length == 0) return;
		for(int i=0; i< subEmitters.Length; i++)
		{
			if (!subEmitters[i].canShoot) continue;
			subEmitters[i].canShoot = false;
			subEmitters[i].KillEmitters();
		}
	}
	
	// Shortcut : quick rotation
	public void Rotate(float angle)
	{
		selfBullet.Rotate(angle);
	}

	// Shortcut : change bullet graphics
	public void ChangeBullets(Sprite sprite, float hitboxRadius)
	{
		for(int i=0; i<bullets.Length; i++)
		{
			bulletHitboxes[i].radius = hitboxRadius;

			if (sprite != null) bulletGraphics[i].sprite = sprite;
			else bulletGraphics[i].sprite = null;
		}
	}
}
