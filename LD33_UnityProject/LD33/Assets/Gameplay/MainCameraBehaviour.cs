using UnityEngine;
using System.Collections;

public class MainCameraBehaviour : MonoBehaviour {

	public PlatformingInputHandler controller;
	public Transform player, self;
	public float lerpSpeed, distanceOffset, cameraDelay;
	private float lastPlayerPosX, moveTimestamp;
	// helper vars
	private float abs, lastFrameAbs, sign, lastFrameSign;

	public bool playerDidntMove, mustFollowPlayer;

	void Update ()
	{
		/**
		if (lastPlayerPosX == player.position.x) playerDidntMove = true;
		else if (playerDidntMove)
		{
			playerDidntMove = false;
			StartCoroutine(MoveTowardsPlayer());
        }
		//*/

		abs = Mathf.Abs(controller.moveDir.x);
        if (abs > 0.4f) sign = Mathf.Sign(controller.moveDir.x);
		if (abs > 0.4f && (lastFrameAbs < 0.4f || sign != lastFrameSign)) moveTimestamp = Time.time;

		float targetX = (player.position.x + distanceOffset * sign);
        mustFollowPlayer = Mathf.Abs(self.position.x - targetX) > 0.1f;

		if (mustFollowPlayer && Time.time - moveTimestamp > cameraDelay)
		{
			float newPosX = Mathf.Lerp(self.position.x, targetX, lerpSpeed * Time.deltaTime);
			self.position = new Vector3(newPosX, self.position.y, self.position.z);
			//if (lastPlayerPosX == player.position.x && Mathf.Abs(self.position.x-player.position.x)<0.1f) mustFollowPlayer = false;
		}

		lastFrameAbs = abs;
		lastFrameSign = sign;
	}
}
