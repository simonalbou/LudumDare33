using UnityEngine;
using System.Collections;

public class VirtualJoystick : MonoBehaviour 
{
    private Vector2 basePosition;
    private Vector2 newPosition;
    private Vector2 oldPosition;
    private Transform trsfrm;
    private SpriteRenderer spr;

    void Start()
    {
        trsfrm = transform;
        spr = GetComponent<SpriteRenderer>();
        newPosition = basePosition;
    }

	// Update is called once per frame
	void Update () 
    {
        /*foreach (Touch tch in Input.touches)
        {
            if (tch.phase == TouchPhase.Began)
            {
                basePosition = tch.position;
                trsfrm.position = basePosition;
                Debug.Log(basePosition);
            }
        }*/
        if (Input.GetMouseButtonDown(0))
        {
            basePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            trsfrm.position = basePosition;
            spr.enabled = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            spr.enabled = false;
            newPosition = basePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), basePosition) < 1.5f)
            {
                newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                Vector2 test = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 test2 = basePosition - test;
                newPosition = -test2.normalized*1.5f + basePosition;
            }
            trsfrm.position = newPosition;
        }
        Debug.Log(((newPosition.x - basePosition.x) / 1.5f));
	}

    public float GetX()
    {
        return (((newPosition.x - basePosition.x) / 1.5f));
    }
}
