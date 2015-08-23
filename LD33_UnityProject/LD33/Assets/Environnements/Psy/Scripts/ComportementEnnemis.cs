using UnityEngine;
using System.Collections;

public class ComportementEnnemis : MonoBehaviour 
{
	private Vector2 positionBase;
	[Header("Basic behaviour")]
	public bool flyInCircles;
	public bool walk;
	public float walkOffset;
	public bool followsPath;
	public Transform groupOfPathPoints;
	public Transform[] pathPoints;
	public float blendingTimeBetweenTargetPoints = 1;
	private float curChangeTargetProgression = 0;
	private bool isChangingTargetPoint = false;
	private int currentTargetPoint;

	[Header("When Angry :")]
	public float distanceAggro;
	public bool chargesPlayer;
	public float speedWhileAttacking;
	public bool usesSpecialAttack;
	//public AttackPattern attackPattern;

	[Header("Basic Stats")]
	private bool attacking;
    public float baseSpeed;
    private bool droite;

	[Header("References")]
	public Transform player;
	public Transform self;
	public CharacterController2D cc2d;
	
	// Use this for initialization
	public virtual void Start () 
    {
		groupOfPathPoints.parent = null;
        self = transform;
	    positionBase = self.position;
		cc2d = GetComponent<CharacterController2D>();
	}
	
	// Update is called once per frame
	public virtual void Update () 
    {
        if(Vector2.Distance(player.position, self.position) < distanceAggro)
		{
			attacking = true;
			groupOfPathPoints.parent = self;
		}
		else
		{
			attacking = false;
			groupOfPathPoints.parent = null;
		}

		if (followsPath)
		{
			if (!attacking) UpdateFollowPath();
			else if (chargesPlayer) UpdateFly(); // we use the same behaviour here
        }

		if (flyInCircles) UpdateFly();
		if (walk) UpdateWalk();
	}

	public virtual void UpdateFollowPath()
	{
		if (isChangingTargetPoint) curChangeTargetProgression += Time.deltaTime / blendingTimeBetweenTargetPoints;
		if (curChangeTargetProgression > 1)
		{
			curChangeTargetProgression = 0;
			isChangingTargetPoint = false;
			currentTargetPoint++;
			if (currentTargetPoint == pathPoints.Length) currentTargetPoint = 0;
		}

		Vector3 targetPos = pathPoints[currentTargetPoint].position * (1-curChangeTargetProgression) + pathPoints[(currentTargetPoint+1)%pathPoints.Length].position * curChangeTargetProgression;

		Vector3 delta = (targetPos - self.position).normalized * Time.deltaTime * baseSpeed;

		cc2d.Move(delta);
		
		if(Vector2.Distance(pathPoints[currentTargetPoint].position, self.position) < 0.2f)
		{
			isChangingTargetPoint = true;
		}

	}

	public virtual void UpdateFly()
	{
		Quaternion angleRotation;
		float speed;
		if (attacking)
		{
			angleRotation = Quaternion.LookRotation(Vector3.forward, player.position - transform.position);
			speed = speedWhileAttacking;
		}
		else
		{
			angleRotation = Quaternion.LookRotation(Vector3.forward, positionBase - (Vector2)transform.position);
			speed = baseSpeed;
		}

		self.rotation = Quaternion.Slerp(self.rotation, angleRotation, Time.deltaTime * speed);
		self.GetChild(0).rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(0f, 0f, 0f));

		self.Translate(self.up * Time.deltaTime * speed, Space.World);
	}

	public virtual void UpdateWalk()
	{
		if (attacking)
		{
			cc2d.Move(new Vector2(Mathf.Sign(self.position.x - player.position.x) * -speedWhileAttacking * Time.deltaTime, 0f));
		}
		else
		{
			if (droite)
			{
				if (self.position.x < positionBase.x + walkOffset)
				{
					cc2d.Move(new Vector2(Mathf.Sign(self.position.x - player.position.x) * baseSpeed * Time.deltaTime, 0f));
				}
				else
				{
					droite = false;
				}
			}
			else
			{
				if (self.position.x > positionBase.x - walkOffset)
				{
					cc2d.Move(new Vector2(Mathf.Sign(self.position.x - player.position.x) * -baseSpeed * Time.deltaTime, 0f));
				}
				else
				{
					droite = true;
				}
			}
		}
	}
}
