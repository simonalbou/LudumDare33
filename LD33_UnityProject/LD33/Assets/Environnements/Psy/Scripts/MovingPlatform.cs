using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour 
{
    public bool horizontal;
    public float offset;
    public float speed;

    private Transform self;
    private Vector2 positionBase;
    private CharacterController2D cc2d;
    public bool droiteHaut = true;

	// Use this for initialization
	void Start () 
    {
        self = transform;
        positionBase = self.position;
        cc2d = GetComponent<CharacterController2D>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (horizontal)
        {
            if (droiteHaut)
            {
                if (self.position.x < positionBase.x + offset)
                {
                    cc2d.Move(new Vector2(speed * Time.deltaTime, 0f));
                }
                else
                {
                    droiteHaut = false;
                }
            }
            else
            {
                if (self.position.x > positionBase.x - offset)
                {
                    cc2d.Move(new Vector2(-speed * Time.deltaTime, 0f));
                }
                else
                {
                    droiteHaut = true;
                }
            }
        }
        else
        {
            if (droiteHaut)
            {
                if (self.position.y < positionBase.y + offset)
                {
                    cc2d.Move(new Vector2(0f, speed * Time.deltaTime));
                }
                else
                {
                    droiteHaut = false;
                }
            }
            else
            {
                if (self.position.y > positionBase.y - offset)
                {
                    cc2d.Move(new Vector2(0f, -speed * Time.deltaTime));
                }
                else
                {
                    droiteHaut = true;
                }
            }
        }
	}
}
