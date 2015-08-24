using UnityEngine;
using System.Collections;

public class HumanHead : SpecialAttack {

	public float angleCrachat;

	public override void OnJustObtained()
	{
		base.OnJustObtained();

		// do more things if needed
	}

	public override void OnUsingBodypart()
	{
		base.OnUsingBodypart();

		bodyPart.currentEmitter.SimpleShoot(angleCrachat, !bodyPart.playerScript.facesRight);

		// do more things if needed, especially launch special attacks
	}

	public override void OnJustLost()
	{
		base.OnJustLost();

		// do more things if needed
	}
}
