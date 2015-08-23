using UnityEngine;
using System.Collections;

public class ComportementEnnemis : MonoBehaviour 
{
    public Transform player;
    private Vector2 positionBase;
    private Transform self;
    public bool fly;
    public bool walk;
    public bool longRange;
    private bool attack;
    public float baseSpeed;
    public float attackSpeed;
    public float distanceAggro;
    private bool droite;
    public float offset;

    private CharacterController2D cc2d;

	// Use this for initialization
	void Start () 
    {
        self = transform;
	    positionBase = transform.position;
        if (walk)
        {
            cc2d = GetComponent<CharacterController2D>();
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Vector2.Distance(player.position, self.position) < distanceAggro)
        {
            attack = true;
        }
        else
        {
            attack = false;
        }


        if (fly)
        {
            Quaternion angleRotation;
            float speed;
            if (attack)
            {
                angleRotation = Quaternion.LookRotation(Vector3.forward, player.position - transform.position);
                speed = attackSpeed;
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

        if (longRange)
        {

        }

        if (walk)
        {
            if (attack)
            {
                cc2d.Move(new Vector2(Mathf.Sign(self.position.x - player.position.x)*-attackSpeed*Time.deltaTime, 0f));
            }
            else
            {
                if (droite)
                {
                    if (self.position.x < positionBase.x + offset)
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
                    if (self.position.x > positionBase.x - offset)
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
}
