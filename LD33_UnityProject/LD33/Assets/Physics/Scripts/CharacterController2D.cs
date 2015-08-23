using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * The Charactercontroller2D script allows a GameObject to move in a 2D space.
 * It works exclusively with a BoxCollider2D, supports scale, slopes, and even stepping on another CharacterController2D which is moving.
 * Just call Charactercontroller2D.Move(Vector2) in your own script, and it'll be functional.
 * You also have access to three collision events : one for any collision, one for horizontal collisions, one for vertical collisions.
 */

// Author : Simon Albou (@Syawra)

[RequireComponent(typeof(Raycaster))]
public class CharacterController2D : MonoBehaviour
{
	// Collision Flags
	public class CollisionFlags2D
	{
		public bool right;
		public bool left;
		public bool above;
		public bool below;
		public bool becameGroundedThisFrame;
		public bool fellThisFrame;

		public void reset()
		{
			right = left = above = below = becameGroundedThisFrame = fellThisFrame = false;
		}
	}

	#region properties

	public Transform self, graphics;
	public BoxCollider2D box;

	[HideInInspector]
	[NonSerialized]
	public CollisionFlags2D collisionFlags = new CollisionFlags2D();
	public bool isGrounded { get { return collisionFlags.below; } }
	public bool justGotGrounded { get { return collisionFlags.becameGroundedThisFrame; } }
	public bool justFell { get { return collisionFlags.fellThisFrame; } }

	public Raycaster _raycaster;
	private float skinWidth;

	// If we land on something that moves, we must be able to move along : (unless stated otherwise by setting canBeCarried to false)
	public bool canBeCarried, canCarry;
	[HideInInspector]
	public CharacterController2D parent;
	[HideInInspector]
	public List<CharacterController2D> children;
	
	public event Action<RaycastHitPlus> onCollisionEnter, onHorizontalCollisionEnter, onVerticalCollisionEnter;

	#endregion

	#region toolbox

	public float bottom { get { return self.position.y + (box.offset.y - box.size.y * 0.5f) * self.localScale.y; } }
	public float top { get { return self.position.y + (box.offset.y + box.size.y * 0.5f) * self.localScale.y; } }

	public float bottomDelta { get { return self.position.y - bottom; } }
	public float topDelta { get { return self.position.y - top; } }

	public float left { get { return self.position.x + (box.offset.x - box.size.x * 0.5f) * self.localScale.x; } }
	public float right { get { return self.position.x + (box.offset.x + box.size.x * 0.5f) * self.localScale.x; } }

	public float leftDelta { get { return self.position.x - left; } }
	public float rightDelta { get { return self.position.x - right; } }

	#endregion

	#region Monobehaviour

	void Awake()
	{
		if(!self) self = GetComponent<Transform>();
		if(!box) box = GetComponent<BoxCollider2D>();
	}

	// Copy the raycaster's skinWidth into this component
	void Start()
	{
		skinWidth = _raycaster.skinWidth;
		children = new List<CharacterController2D>();
	}
	
	// Debug - uncomment when needed
	/*void Update()
	{
		if (Input.GetKeyDown(KeyCode.L)) // Do something
	}*/

	#endregion

	#region Movement

	private void moveHorizontally( ref Vector3 deltaMovement )
	{
		float length = Mathf.Abs(deltaMovement.x) + skinWidth;
		bool isGoingRight = deltaMovement.x > 0;

		RaycastHitPlus result = isGoingRight ? _raycaster.CastRightRays(length) : _raycaster.CastLeftRays(length);

		if( result.box != null )
		{
			// stop the horizontal movement as we bumped into something
			deltaMovement.x = 0;
			
			// flip the collision flags
			if (isGoingRight) collisionFlags.right = true;
			else collisionFlags.left = true;

			// And finally, call events
			if (onCollisionEnter != null) onCollisionEnter(result);
			if (onHorizontalCollisionEnter != null)	onHorizontalCollisionEnter (result);
		}
	}

	private void moveVertically( ref Vector3 deltaMovement )
	{
		bool isGoingUp = deltaMovement.y > 0;
		float length = Mathf.Abs(deltaMovement.y) + skinWidth;

		RaycastHitPlus result = isGoingUp ? _raycaster.CastUpperRays(length, false) :_raycaster.CastLowerRays(length);

		if( result.box )
		{
			// set our new deltaMovement and recalculate the rayDistance taking it into account
			deltaMovement.y = result.hit.point.y - result.ray.y;

			// remember to remove the skinWidth from our deltaMovement
			if( isGoingUp )
			{
				deltaMovement.y -= skinWidth;
				collisionFlags.above = true;
			}
			else
			{
				deltaMovement.y += skinWidth;
				collisionFlags.below = true;	
			}

			// Watch out, the global event could be called twice (one for horizontal movement, one for vertical movement).
			if (onCollisionEnter != null)	onCollisionEnter(result);
			if (onVerticalCollisionEnter != null) onVerticalCollisionEnter (result);
		}
	}

	public void Move( Vector3 deltaMovement )
	{
		if(children.Count != 0)
		{
			foreach(CharacterController2D cc in children)
			{
				cc.Move(deltaMovement);
			}
		}

		// Reset the collision info
		var wasGroundedBeforeMoving = collisionFlags.below;
		collisionFlags.reset();

		// First we check movement in the horizontal dir
		if( deltaMovement.x != 0 )
			moveHorizontally( ref deltaMovement );
		
		// Next, check movement in the vertical dir
		if( deltaMovement.y != 0)
			moveVertically( ref deltaMovement );
		
		// So, we move as planned, as if there wasn't any obstacle...
		self.Translate( deltaMovement, Space.World );
	
		// Did we just get grounded ?
		if( !wasGroundedBeforeMoving && collisionFlags.below )
			collisionFlags.becameGroundedThisFrame = true;

		// Did we just get airborne ?
		if (wasGroundedBeforeMoving && !collisionFlags.below)
			collisionFlags.fellThisFrame = true;
	}

	#endregion	

	#region Moving ground handling

	void LateUpdate()
	{
		if (!canBeCarried) return;

		CharacterController2D possibleGround = FindGround();

		if (possibleGround == parent) return;
		
		StopFollowing();

		if (possibleGround != null)
		{
			if (!possibleGround.canCarry) return;
			parent = possibleGround;
			possibleGround.children.Add(this);
		}
	}

	CharacterController2D FindGround()
	{
		RaycastHitPlus hit = _raycaster.CastLowerRays(5 * skinWidth, true);

		if(hit.box == null) return null;

		return hit.box.GetComponent<CharacterController2D>();		
	}

	void StopFollowing()
	{
		if (!parent) return;
		parent.children.Remove(this);
		parent = null;
	}

	#endregion
}
