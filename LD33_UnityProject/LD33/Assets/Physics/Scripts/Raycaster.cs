using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The Raycaster must be part of the CharacterController.
 * It's the component that draws rays to return data from any BoxCollider2D hit while trying to move.
 */

// Author : Simon Albou (@Syawra)

// This struct stores data from the raycasts, just as a regular RaycastHit2D, with extra infos added.
public struct RaycastHitPlus
{
	public RaycastHit2D hit;
	public Vector2 ray;
	public Collider2D box;
}
[RequireComponent(typeof(BoxCollider2D))]
public class Raycaster : MonoBehaviour
{
	#region structures

	private struct RaycastOrigins
	{
		public Vector2 topRight;
		public Vector2 topLeft;
		public Vector2 bottomRight;
		public Vector2 bottomLeft;
	}

	#endregion

	#region properties

	// Number of rays
	public int totalHorizontalRays = 8;
	public int totalVerticalRays = 4;

	// Data extrapolated from number of rays
	private float _verticalDistanceBetweenRays;
	private float _horizontalDistanceBetweenRays;

	// Stores the result of our raycasts
	private RaycastHitPlus _outcome;

	// The object that stores our raycasts' origins.
	private RaycastOrigins _origins;

	// Which layers are actually collidable ?
	public LayerMask collidableLayers = 1;

	// Which layers contain platforms that only block you from above ? (always useful in platforming)
	public LayerMask oneWayLayers = 0;

	// internal references
	public BoxCollider2D box;
	public Transform self;

	// Distance between collider limit and the actual raycast starting point
	public float skinWidth = 0.02f;
	
	#endregion

	#region preparative functions

	void Awake()
	{
		RecalculateDistanceBetweenRays();
	}
	
	void RecalculateDistanceBetweenRays()
	{
		// figure out the distance between our rays in both directions

		// horizontal
		var colliderUsableHeight = box.size.y * Mathf.Abs( self.localScale.y ) - ( 2f * skinWidth );
		_verticalDistanceBetweenRays = colliderUsableHeight / ( totalHorizontalRays - 1 );
		
		// vertical
		var colliderUsableWidth = box.size.x * Mathf.Abs( self.localScale.x ) - ( 2f * skinWidth );
		_horizontalDistanceBetweenRays = colliderUsableWidth / ( totalVerticalRays - 1 );
		
	}

	private void DrawRay( Vector3 start, Vector3 dir, Color color )
	{
		Debug.DrawRay( start, dir, color );
	}

	#endregion

	private void refreshRayOrigins()
	{
		Vector2 scaledColliderSize = new Vector2( box.size.x * Mathf.Abs( self.localScale.x ), box.size.y * Mathf.Abs( self.localScale.y ) ) / 2;
		Vector2 scaledCenter = new Vector2 (box.offset.x * self.localScale.x ,box.offset.y * self.localScale.y );

		Vector2 positionAsVector2 = new Vector2(self.position.x, self.position.y);

		_origins.topRight = positionAsVector2 + new Vector2( scaledCenter.x + scaledColliderSize.x, scaledCenter.y + scaledColliderSize.y );
		_origins.topRight.x -= skinWidth;
		_origins.topRight.y -= skinWidth;

		_origins.topLeft = positionAsVector2 + new Vector2(scaledCenter.x - scaledColliderSize.x, scaledCenter.y + scaledColliderSize.y);
		_origins.topLeft.x += skinWidth;
		_origins.topLeft.y -= skinWidth;

		_origins.bottomRight = positionAsVector2 + new Vector2(scaledCenter.x + scaledColliderSize.x, scaledCenter.y - scaledColliderSize.y);
		_origins.bottomRight.x -= skinWidth;
		_origins.bottomRight.y += skinWidth;

		_origins.bottomLeft = positionAsVector2 + new Vector2(scaledCenter.x - scaledColliderSize.x, scaledCenter.y - scaledColliderSize.y);
		_origins.bottomLeft.x += skinWidth;
		_origins.bottomLeft.y += skinWidth;

		RecalculateDistanceBetweenRays();
	}

	// Could be used for scale and rotation : cast rays in 4 directions simultaneously
	public RaycastHitPlus CastRaysEverywhere(float length)
	{
		refreshRayOrigins ();

		RaycastHitPlus result = CastLeftRays (length, true);
		if (result.box != null) return result;

		result = CastRightRays(length, true);
		if (result.box != null) return result;

		result = CastLowerRays(length, true);
		if (result.box != null) return result;

		result = CastUpperRays(length, true);
		return result;
	}

	public RaycastHitPlus CastLeftRays(float length, bool dontrefreshRayOrigins = false)
	{
		_outcome.box = null;

		if(!dontrefreshRayOrigins) refreshRayOrigins();
		
		var rayDistance = Mathf.Abs(length) + skinWidth;
		var rayDirection = -Vector2.right;
		var initialRayOrigin = _origins.bottomLeft;
		
		int totalRays = totalHorizontalRays;
		
		for( var i = 0; i < totalRays; i++ )
		{
			_outcome.ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays);

			DrawRay(_outcome.ray, rayDirection, Color.red);
			
			_outcome.hit = Physics2D.Raycast(_outcome.ray, rayDirection, rayDistance, collidableLayers);
			
			if (_outcome.hit.collider != null)
			{
				if ((1 << _outcome.hit.collider.gameObject.layer & oneWayLayers) > 0) continue;

				if (Mathf.Abs(_outcome.hit.point.x - _outcome.ray.x) > skinWidth)
				{
					_outcome.box = _outcome.hit.collider;
					return _outcome;
				}
			}
		}
		
		return _outcome;
	}

	public RaycastHitPlus CastRightRays(float length, bool dontrefreshRayOrigins = false)
	{
		_outcome.box = null;

		if(!dontrefreshRayOrigins) refreshRayOrigins ();

		var rayDistance = Mathf.Abs(length) + skinWidth;
		var rayDirection = Vector2.right;
		var initialRayOrigin = _origins.bottomRight;

		int totalRays = totalHorizontalRays;

		for( var i = 0; i < totalRays; i++ )
		{
			_outcome.ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays);

			DrawRay(_outcome.ray, rayDirection, Color.red);
			
			_outcome.hit = Physics2D.Raycast(_outcome.ray, rayDirection, rayDistance, collidableLayers);
			
			if (_outcome.hit.collider != null)
			{
				if (Mathf.Abs(_outcome.hit.point.x - _outcome.ray.x) > 0.015f)
				{
					if ((1<<_outcome.hit.collider.gameObject.layer & oneWayLayers) > 0) continue;
					_outcome.box = _outcome.hit.collider;
					return _outcome;
				}
			}
		}

		return _outcome;
	}

	public RaycastHitPlus CastUpperRays(float length, bool dontrefreshRayOrigins = false)
	{
		_outcome.box = null;

		if(!dontrefreshRayOrigins) refreshRayOrigins ();
		
		var rayDistance = Mathf.Abs(length) + skinWidth;
		var rayDirection = Vector2.up;
		var initialRayOrigin = _origins.topLeft;
		
		int totalRays = totalVerticalRays;
		
		for( var i = 0; i < totalRays; i++ )
		{
			_outcome.ray = new Vector2(initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y);

			DrawRay( _outcome.ray, rayDirection, Color.red );
			
			_outcome.hit = Physics2D.Raycast(_outcome.ray, rayDirection, rayDistance, collidableLayers);
			
			if (_outcome.hit.collider != null)
			{
				if ((1 << _outcome.hit.collider.gameObject.layer & oneWayLayers) > 0) continue;

				_outcome.box = _outcome.hit.collider;
				return _outcome;
			}
		}

		return _outcome;
	}

	public RaycastHitPlus CastLowerRays(float length, bool dontrefreshRayOrigins = false)
	{
		_outcome.box = null;

		if(!dontrefreshRayOrigins) refreshRayOrigins ();
		
		var rayDistance = Mathf.Abs( length ) + skinWidth;
		var rayDirection = -Vector2.up;
		var initialRayOrigin = _origins.bottomLeft;
		
		int totalRays = totalVerticalRays;
		
		for( var i = 0; i < totalRays; i++ )
		{
			_outcome.ray = new Vector2(initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y);

			DrawRay(_outcome.ray, rayDirection, Color.red);
			
			_outcome.hit = Physics2D.Raycast(_outcome.ray, rayDirection, rayDistance, collidableLayers);
			
			if (_outcome.hit.collider != null)
			{
				_outcome.box = _outcome.hit.collider;
				return _outcome;
			}
		}

		return _outcome;
	}
}
