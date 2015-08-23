using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This is not really part of the slope engine.
 * This script serves as a simple example of an input handler that could work with the CharacterController2D.
 * Feel free to edit it and add animation support, as it depends on your game's graphic needs !
 */

// Author : Simon Albou (@Syawra)

[RequireComponent(typeof(CharacterController2D))]
public class PlatformingInputHandler : MonoBehaviour
{
	#region References
		private CharacterController2D cc;
		[Header("References")]
		public Transform self;
		public Transform graphics;
		public Animator anim; // not used here because it's a simple example script. But it could be !
		public VirtualJoystick virtualJoystick;
	#endregion

	#region Physic data

		// speed
		[Header("Physics")]
		public float baseSpeed;
		public float baseGravity;
		private float currentSpeed, currentGravity;
		[HideInInspector]
		public Vector2 moveDir;

		// acceleration
		private float speedCurveModifier; // modifier due to slow acceleration or deceleration
		public AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 1, 1, 1);
		public float accelerationTime;
		public float gravityBoostPerSecond;
		private float accelerationStartTimeStamp, fallTimeStamp;

		// jumps
		[Header("Jump")]
		public AnimationCurve gravityJumpCurve = AnimationCurve.Linear(0, 1, 1, 1);
		public int maxAllowedJumps = 2;
		public float fastFallMultiplier = 2; // we'll land faster if we release the jump button earlier
		[HideInInspector]
		public bool dontUseFastFallForNextJump = false;
		[HideInInspector]
		public int jumpsLeft;
		private bool isJumping;
		private float jumpTimeStamp;
		private float descentTimeStamp;
	#endregion

	#region Animation-related
		private bool facesRight; // we won't do anything here but flip the character to make it face left or right.
		// Add anything related to an Animator component here.
	#endregion

	#region Inputs
		[HideInInspector]
		public bool inputInhibited = false; // Useful if some manager pauses the game

		private KeyCode[] jumpKeys, downKeys, leftKeys, rightKeys, upperKeys;
		private bool inputJump, inputJumpDown, inputDown, inputLeft, inputRight, inputUp;
		private bool goesLeft, goesRight, goesUp, goesDown; // Used for animations : shortcuts for, like, (inputLeft || Input.GetAxis("Horizontal")<0).
		private float horizontalAxis, verticalAxis;
		private bool xWasNotStrictlyPositive, xWasNotStrictlyNegative, yWasNotStrictlyPositive, yWasNotStrictlyNegative; // last frame data for computing horizontal acceleration
	#endregion
	
	#region Start
	
	void Awake ()
	{
		LoadInput();

		facesRight = true;
		currentSpeed = baseSpeed * Mathf.Abs(self.localScale.x);
		cc = GetComponent<CharacterController2D>();

		speedCurveModifier = 1;

		jumpsLeft = maxAllowedJumps;
	}

	// Initializes the lists of available keys, so we don't use the static vars all the time.
	void LoadInput()
	{
		jumpKeys = KeyBinding.jumpKeys;
		downKeys = KeyBinding.downKeys;
		leftKeys = KeyBinding.leftKeys;
		rightKeys = KeyBinding.rightKeys;
		upperKeys = KeyBinding.upperKeys;
	}

	#endregion
	
	#region Update
	
	void Update()
	{
		//if (!Input.GetKey(KeyCode.D)) Debug.Log(Input.GetAxis("Horizontal"));

		UpdateInput();

		UpdateMovement();
		UpdateAnimation();
	}

	// Resets all the inputs from last frame then check, our lists of available keys.
	void UpdateInput()
	{
		inputDown = false;
		inputLeft = false;
		inputRight = false;
		inputUp = false;

		inputJump = false;
		inputJumpDown = false;

		horizontalAxis = 0f;
		verticalAxis = 0f;
		
		goesLeft = goesRight = false;
	
		if(inputInhibited) return;

		for (int i = 0; i < downKeys.Length; i++)
		{
			if (Input.GetKey(downKeys[i])) inputDown = true;
			//else if (Input.GetAxis("Vertical") < -0.5f) inputDown = true;
		}

		for (int i = 0; i < leftKeys.Length; i++)
		{
			if (Input.GetKey(leftKeys[i])) inputLeft = true;
			//else if (Input.GetAxis("Horizontal") < -0.5f) inputLeft = true;
		}

		for (int i = 0; i < rightKeys.Length; i++)
		{
			if (Input.GetKey(rightKeys[i])) inputRight = true;
			//else if (Input.GetAxis("Horizontal") > 0.5f) inputRight = true;
		}

		for (int i = 0; i < upperKeys.Length; i++)
		{
			if (Input.GetKey(upperKeys[i])) inputUp = true;
			//else if (Input.GetAxis("Vertical") > 0.5f) inputUp = true;
		}

		for (int i = 0; i < jumpKeys.Length; i++)
		{
			if (Input.GetKey(jumpKeys[i])) inputJump = true;
			if (Input.GetKeyDown(jumpKeys[i])) inputJumpDown = true;
		}
		
		if(inputLeft && !inputRight) horizontalAxis = -1f;
		else if (!inputLeft && inputRight) horizontalAxis = 1f;
		else horizontalAxis = Input.GetAxis("Horizontal") + virtualJoystick.GetX();

		if(inputDown && !inputUp) verticalAxis = -1f;
		else if (!inputDown && inputUp) verticalAxis = 1f;
		else verticalAxis = Input.GetAxis("Vertical");

		goesLeft = inputLeft || Input.GetAxis("Horizontal") < 0 || virtualJoystick.GetX() < 0;
		goesRight = inputRight || Input.GetAxis("Horizontal") > 0 || virtualJoystick.GetX() > 0;
		goesUp = inputUp || Input.GetAxis("Vertical") > 0;
		goesDown = inputDown || Input.GetAxis("Vertical") < 0;
	}

	// Convert input into movement
	void UpdateMovement()
	{
		// Horizontal movement handling :

		currentSpeed = baseSpeed;
		// uncomment if scale support needed // currentSpeed = new Vector2(baseSpeed.x * Mathf.Abs(self.localScale.x), baseSpeed.y * Mathf.Abs(self.localScale.y));

		if (accelerationTime > 0)
		{
			if (cc.isGrounded) // we won't reset acceleration airborne, we would feel freezed
			{
				if (xWasNotStrictlyPositive && horizontalAxis > 0) accelerationStartTimeStamp = Time.time;
				if (xWasNotStrictlyNegative && horizontalAxis < 0) accelerationStartTimeStamp = Time.time;
			} 

			speedCurveModifier = accelerationCurve.Evaluate(Mathf.Clamp01((Time.time - accelerationStartTimeStamp) / accelerationTime));
		}

		// Vertical movement handling : multiple jump and fall system
		
		if (cc.justFell)
		{
			if (!isJumping) jumpsLeft--;
			fallTimeStamp = Time.time;
		}

		// We use isGrounded instead or justGotGrounded to avoid a one-frame bug.
		//if (cc.justGotGrounded) jumpsLeft = maxAllowedJumps;
		if (cc.isGrounded)
		{
			jumpsLeft = maxAllowedJumps;
			dontUseFastFallForNextJump = false;
		}

		if (inputJumpDown) Jump();

		float urgeFallWhenButtonReleased = 1;
		if(!dontUseFastFallForNextJump && !inputJump) urgeFallWhenButtonReleased = fastFallMultiplier;
		float timeSinceJump = (Time.time - jumpTimeStamp) * urgeFallWhenButtonReleased;

		if (isJumping)
		{
			//if (currentGravity < 0 && cc.isGrounded) isJumping = false;
			if (cc.justGotGrounded) isJumping = false;
			
			if (timeSinceJump > gravityJumpCurve.keys[gravityJumpCurve.keys.Length - 1].time)
			{
				isJumping = false;
				fallTimeStamp = Time.time;
			}
		}

		// Vertical movement handling : descending

		int oneWayLayers = cc._raycaster.oneWayLayers;

		if(verticalAxis < 0)
		{
			if (yWasNotStrictlyNegative) descentTimeStamp = Time.time; 
			cc._raycaster.collidableLayers &= ~oneWayLayers;			
		}
		else if(Time.time - descentTimeStamp > 0.01f)
			cc._raycaster.collidableLayers |= oneWayLayers;

		// Vertical movement handling : gravity value

		if (!isJumping)
		{
			currentGravity = baseGravity;
			if(!cc.isGrounded) currentGravity += baseGravity * gravityBoostPerSecond * (Time.time - fallTimeStamp);
		}
		else currentGravity = -1.0f * baseGravity * gravityJumpCurve.Evaluate(timeSinceJump);

		// Final vector :

		moveDir.y = currentGravity * -1.0f;
		moveDir.x = currentSpeed * horizontalAxis * speedCurveModifier;

		// Applying final movement :
		
		cc.Move (moveDir * Time.deltaTime);
	}

	// Save some data for the next frame here
	void LateUpdate()
	{
		xWasNotStrictlyNegative = horizontalAxis >= 0;
		xWasNotStrictlyPositive = horizontalAxis <= 0;

		yWasNotStrictlyNegative = verticalAxis >= 0;
		yWasNotStrictlyPositive = verticalAxis <= 0;
	}

	// Everything related to graphics goes here.
	void UpdateAnimation()
	{
		return;
		/**
		if(horizontalAxis == 0) return;
		float sign = Mathf.Sign(horizontalAxis);
		graphics.localScale = new Vector3(Mathf.Abs(graphics.localScale.x)*sign, graphics.localScale.y, graphics.localScale.z);
		//*/
	}
	
	#endregion

	#region Jump

	public void Jump()
	{
		if (jumpsLeft == 0) return;
		jumpsLeft--;

		isJumping = true;
		jumpTimeStamp = Time.time;
	}

	#endregion
}
