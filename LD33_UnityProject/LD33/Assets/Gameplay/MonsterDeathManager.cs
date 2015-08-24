using UnityEngine;
using System.Collections;

public class MonsterDeathManager : MonoBehaviour {

	[Header("Droppable Bodyparts")]
	public Bodypart commonDrop;
	public Bodypart rareDrop;
	public Bodypart superRareDrop;
	[Header("References")]
	//public SpriteRenderer enemyGraphics;
	//public Animator enemyAnimator;
	public BodypartIcon droppableIcon;
	public AttackPattern attackPattern;
	public ComportementEnnemis enemyScript;
	public ParticlePool deathVFXPool;

	public void LaunchDeath()
	{
		attackPattern.enabled = false;
		enemyScript.enabled = false;
		deathVFXPool.PlayVFX(enemyScript.self.position);
		DropIcon();
		enemyScript.gameObject.SetActive(false);
	}

	public Bodypart GetRandomBodypart()
	{
		float rng = Random.value;

		if (superRareDrop)
		{
			if (rng < 0.1f) return superRareDrop;
			else if (rareDrop && rng < 0.4f) return rareDrop;
			else if (commonDrop) return commonDrop;
		}
		else if (rareDrop)
		{
			if (rng < 0.35f) return rareDrop;
			else if (commonDrop) return commonDrop;
		}
		else if (commonDrop) return commonDrop;

		return null;
	}

	public void DropIcon()
	{
		Bodypart part = GetRandomBodypart();
		if (part == null) return;

		droppableIcon.bodyPart = part;
		droppableIcon.selfImage.sprite = part.icon;
		droppableIcon.enabled = true;
		droppableIcon.selfImage.enabled = true;
		droppableIcon.self.position = enemyScript.self.position;
		droppableIcon.self.rotation = Quaternion.identity;
		droppableIcon.self.parent = null;
	}
}
