using UnityEngine;
using System.Collections;

public class MovingRocks : MonoBehaviour 
{
    public float speed;
    public float offset;
    private bool monte = false;

    private Transform tr;
    private Vector2 positionBase;

	// Use this for initialization
	void Start () 
    {
        tr = transform;
        positionBase = tr.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (monte)
        {
            if (tr.position.y < positionBase.y + offset)
            {
                float newPosY = Mathf.Lerp(tr.position.y, tr.position.y + offset, speed * Time.deltaTime);
                tr.position = new Vector2(tr.position.x, newPosY);
            }
            else
            {
                monte = false;
            }
        }
        else
        {
            if (tr.position.y > positionBase.y - offset)
            {
                float newPosY = Mathf.Lerp(tr.position.y, tr.position.y - offset, speed * Time.deltaTime);
                tr.position = new Vector2(tr.position.x, newPosY);
               // tr.position = new Vector2(tr.position.x, tr.position.y - speed * Time.deltaTime);
            }
            else
            {
                monte = true;
            }
        }
	}
}
