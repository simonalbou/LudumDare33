using UnityEngine;
using System.Collections;

public class SpecialAttack_Template : SpecialAttack {

	public override void OnJustObtained()
	{
		base.OnJustObtained();

		// do more things if needed
	}

	public override void OnUsingBodypart()
	{
		base.OnUsingBodypart();

		// do more things if needed, especially launch special attacks
	}

	public override void OnJustLost()
	{
		base.OnJustLost();

		// do more things if needed
	}
}
