using UnityEngine;
using System.Collections;

public class ParticlePool : MonoBehaviour {

	public ParticleSystem[] particleSystems;
	public Transform[] particleTransforms;

	// Returns an available particle system.
	public int GetAvailableVFX()
	{
		int result = -1;

		for(int i=0; i<particleSystems.Length; i++)
		{
			if (!particleSystems[i].isPlaying)
			{
				result = i;
				break;
			}
		}

		return result;
	}

	// Sends an available particle system somwhere, and plays it with the proper color. Used for bullet deaths.
	public void PlayVFX(Vector3 position, Color color = default(Color))
	{
		int index = GetAvailableVFX();
		if (index == -1) return;
		particleTransforms[index].position = position;
		particleSystems[index].startColor = color;
		particleSystems[index].Play();
	}
}
