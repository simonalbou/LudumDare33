using UnityEngine;
using System.Collections;

public class VirtualJoystick : MonoBehaviour 
{
    private Vector2 basePosition;
    private Vector2 newPosition;
    private Vector2 oldPosition;
    private Transform trsfrm;
    private SpriteRenderer spr;
    private int fingerId1;


    public GameObject cercle;
    private GameObject cercleGO;
    private SpriteRenderer sprCercle;
    private Transform transformCercle;

    void Start()
    {
        trsfrm = transform;
        spr = GetComponent<SpriteRenderer>();
        newPosition = basePosition;
        cercleGO = Instantiate(cercle, Vector2.zero, Quaternion.identity) as GameObject;
        sprCercle = cercleGO.GetComponent<SpriteRenderer>();
        transformCercle = cercleGO.transform;
    }

	// Update is called once per frame
	void Update () 
    {
        /*
         * Gestion du déplacement du cercle intérieur avec le multitouch
         */
        int tapCount = Input.touchCount;
        for (int  i = 0; i < tapCount; i++)
        {
            Touch tch = Input.GetTouch(0);
            fingerId1 = tch.fingerId;
            if (tch.fingerId == fingerId1)
            {
                if (tch.phase == TouchPhase.Began)
                {
                    if (Camera.main.ScreenToViewportPoint(tch.position).x < 0.5f)
                    {
                        basePosition = Camera.main.ScreenToWorldPoint(tch.position);
                        trsfrm.position = basePosition;
                        spr.enabled = true;

                        transformCercle.position = basePosition;
                        sprCercle.enabled = true;
                    }
                }
                if (tch.phase == TouchPhase.Ended)
                {
                    spr.enabled = false;
                    newPosition = basePosition;
                    sprCercle.enabled = false;
                }
                if (tch.phase == TouchPhase.Moved || tch.phase == TouchPhase.Stationary)
                {
                    if (Vector2.Distance(Camera.main.ScreenToWorldPoint(tch.position), basePosition) < 0.8f)
                    {
                        newPosition = Camera.main.ScreenToWorldPoint(tch.position);
                    }
                    else
                    {
                        Vector2 test = Camera.main.ScreenToWorldPoint(tch.position);
                        Vector2 test2 = basePosition - test;
                        newPosition = -test2.normalized * 0.8f + basePosition;
                    }
                    trsfrm.position = newPosition;

                    //Déplacement
                }
            }
        }
            
        /*
         * Gestion à la souris
         * 
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
            if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), basePosition) < 1f)
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
        }*/
        Debug.Log(((newPosition.x - basePosition.x) / 0.8f));
	}

    public float GetX()
    {
        return (((newPosition.x - basePosition.x) / 0.8f));
    }
}
