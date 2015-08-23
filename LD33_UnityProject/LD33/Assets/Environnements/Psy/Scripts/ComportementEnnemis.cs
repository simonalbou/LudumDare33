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


	// Use this for initialization
	void Start () 
    {
        self = transform;
	    positionBase = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (fly)
        {
            if (Vector2.Distance(player.position, self.position) < 3f)
            {
                attack = true;
            }
            else
            {
                attack = false;
            }

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

            transform.rotation = Quaternion.Slerp(transform.rotation, angleRotation, Time.deltaTime * speed);
            transform.GetChild(0).rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(0f, 0f, 0f));

            transform.Translate(transform.up * Time.deltaTime * speed, Space.World);
        }

        if (longRange)
        {

        }

        if (walk)
        {

        }
	}
}
