using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	#region stats

	[Header("Stats")]
	public float power = 1; // used for ally shots
	public float lifeSpan = 5;
	
	public float speed;
	public AnimationCurve speedMod = AnimationCurve.Linear(0, 1, 1, 1);
	public float rotateSpeed;
	public AnimationCurve rotateSpeedMod = AnimationCurve.Linear(0, 1, 1, 1);

	[Header("Homing stuff")]
	public Transform target; // object targeted if we're homing
	public float homingSpeed; // rotate speed while homing
	public AnimationCurve homingSpeedMod = AnimationCurve.Linear(0, 1, 1, 1);

	public bool dontDestroyWhenCollided;

	#endregion

	#region references

	[Header("References")]
	public Transform self;
	public SpriteRenderer graphics;
	public ParticlePool VFXPool;
	public Transform LLScreenCorner, URScreenCorner; // stand for lower left and upper right

	#endregion

	private float timeSinceSpawned;

	void Start()
	{
		if(graphics) graphics.enabled = enabled;
	}

	void Update ()
	{
		timeSinceSpawned += Time.deltaTime;
		self.Translate(Vector3.up * speed * (lifeSpan == 0 ? 1 : speedMod.Evaluate(timeSinceSpawned / lifeSpan)) * Time.deltaTime, Space.Self);
		self.Rotate(Vector3.forward * rotateSpeed * (lifeSpan == 0 ? 1 : rotateSpeedMod.Evaluate(timeSinceSpawned / lifeSpan)) * Time.deltaTime);

		if (target) SlowlyLookAt(target.position);

		if (lifeSpan > 0 && timeSinceSpawned > lifeSpan) Die();
	}

	public bool GetShot()
	{
		if (enabled) return false;

		if(graphics) graphics.enabled = true;
		enabled = true;
		timeSinceSpawned = 0;

		return true;
	}

	public void LookAt(Vector3 dest)
	{
		float angle = Mathf.Atan2(dest.y-self.position.y, dest.x-self.position.x) * Mathf.Rad2Deg - 90.0f;
		self.eulerAngles = Vector3.forward * angle;
		
		// PK SA MARCH PA
	}

	// Homing behaviour
	public void SlowlyLookAt(Vector3 dest)
	{
		// Same formula as LookAt, but we won't turn now
		float angleWanted = Mathf.Atan2(dest.y, dest.x) * Mathf.Rad2Deg - 90.0f;
		
		float diff = angleWanted - self.eulerAngles.z + 360;
		bool turnCCW = diff % 180 < 180;

		if (diff > 180) turnCCW = false;
		else if (diff > 0) turnCCW = true;
		else if (diff > -180) turnCCW = false;
		else turnCCW = true;
		
		self.Rotate(Vector3.forward * homingSpeed * (lifeSpan == 0 ? 1 : homingSpeedMod.Evaluate(timeSinceSpawned / lifeSpan)) * Time.deltaTime * (turnCCW ? 1 : -1));
	}

	// The bullet disappears when it his something or goes offscreen.
	public void Die()
	{
		// Launch VFX under some other conditions
		if (VFXPool && graphics && IsOnScreen()) VFXPool.PlayVFX(self.position, graphics.color);

		// Disable script and graphics
		enabled = false;
		if (graphics) graphics.enabled = false;

		// Return to pooling
		self.parent = null;
	}

	// Is the bullet currently visible on screen ?
	public bool IsOnScreen()
	{
		if (self.position.x < LLScreenCorner.position.x) return false;
		if (self.position.y < LLScreenCorner.position.y) return false;
		if (self.position.x > URScreenCorner.position.x) return false;
		if (self.position.y > URScreenCorner.position.y) return false;

		return true;
	}

	// Shortcut : quick rotation
	public void Rotate(float angle)
	{
		self.Rotate(Vector3.forward * angle);
	}
}
