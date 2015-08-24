using UnityEngine;
using System.Collections;

public class SpecialAttack : MonoBehaviour {

	public Bodypart bodyPart;

	public virtual void OnJustObtained()
	{
		
	}

	public virtual void OnUsingBodypart()
	{
		// example : bodyPart.currentEmitter.SimpleShoot();
	}

	public virtual void OnJustLost()
	{
		
	}
}
