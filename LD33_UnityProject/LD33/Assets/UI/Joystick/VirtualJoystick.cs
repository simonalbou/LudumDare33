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
    private bool bouge = false;
    private bool moitie = false;

	[Header("References")]
	public TouchManager touchManager;
	public Camera mainCam;

    void Start()
    {
        trsfrm = transform;
        spr = GetComponent<SpriteRenderer>();
        newPosition = basePosition;
        cercleGO = Instantiate(cercle, Vector2.zero, Quaternion.identity) as GameObject;
        cercleGO.transform.parent = Camera.main.transform;
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
		/**/
		for (int i = 0; i < tapCount; i++)
		{
			Touch tch = Input.GetTouch(i);

			if (tch.phase == TouchPhase.Began)
			{
				// We only take inputs from the left part of the screen
				if (mainCam.ScreenToViewportPoint(tch.position).x > 0.5f) continue;
				
				// If the joystick already exists, we don't take this one
				if (touchManager.joystickTouchId != -1) continue;

				// If this finger already manages a BodyPartIcon, we don't take it either
				if (!touchManager.IsFingerAvailableForJoystick(tch.fingerId)) continue;

				moitie = true;
				touchManager.joystickTouchId = tch.fingerId;
				basePosition = Camera.main.ScreenToWorldPoint(tch.position);
				trsfrm.position = basePosition;
				spr.enabled = true;

				transformCercle.position = basePosition;
				sprCercle.enabled = true;
				
				continue;
			}

			// After the first frame for a given input, we only use the right fingerId.
			if (tch.fingerId != touchManager.joystickTouchId) break;

			// Turns out we have to constantly check if the same finger has not been assigned to an icon at the first frame.
			if (!touchManager.IsFingerAvailableForJoystick(tch.fingerId))
			{
				moitie = false;
				bouge = false;
				spr.enabled = false;
				sprCercle.enabled = false;
				touchManager.joystickTouchId = -1;
				break;
			}

			if (tch.phase == TouchPhase.Ended)
			{
				moitie = false;
				bouge = false;
				spr.enabled = false;
				newPosition = cercleGO.transform.position;
				sprCercle.enabled = false;
				touchManager.joystickTouchId = -1;
			}
			if (tch.phase == TouchPhase.Moved || tch.phase == TouchPhase.Stationary)
			{
				bouge = true;
				if (Vector2.Distance(mainCam.ScreenToWorldPoint(tch.position), cercleGO.transform.position) < 0.8f)
				{
					newPosition = mainCam.ScreenToWorldPoint(tch.position);
				}
				else
				{
					Vector2 test = mainCam.ScreenToWorldPoint(tch.position);
					Vector2 test3 = cercleGO.transform.position;
					Vector2 test2 = test3 - test;
					newPosition = -test2.normalized * 0.8f + test3;
				}

				//Déplacement
				trsfrm.position = newPosition;
			}
		}
		//*/

		/**
		if (tapCount > 0)
		{
            Touch tch = Input.GetTouch(0);
            fingerId1 = tch.fingerId;
            if (tch.fingerId == fingerId1)
            {
				if (tch.phase == TouchPhase.Began)
                {
                    if (Camera.main.ScreenToViewportPoint(tch.position).x < 0.5f)
                    {
                        moitie = true;
                    }
                    if (moitie)
                    {
                        basePosition = Camera.main.ScreenToWorldPoint(tch.position);
                        trsfrm.position = basePosition;
                        spr.enabled = true;

                        transformCercle.position = basePosition;
                        sprCercle.enabled = true;
                    }
                }
            }
        }
		//*/

        /*
         * Gestion à la souris
         */
        /*if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < 0.5f)
                    {
                        basePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        trsfrm.position = basePosition;
                        spr.enabled = true;

                        transformCercle.position = basePosition;
                        sprCercle.enabled = true;
                    }
        }
        if(Input.GetMouseButtonUp(0))
        {
            spr.enabled = false;
            newPosition = basePosition;
            sprCercle.enabled = false;
        }
        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), basePosition) < 0.8f)
            {
                newPosition = Input.mousePosition;
            }
            else
            {
                Vector2 test = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 test2 = basePosition - test;
                newPosition = -test2.normalized * 0.8f + basePosition;
            }

            //Déplacement
            trsfrm.position = newPosition;
        }*/
	}

    public float GetX()
    {
        if (bouge)
        {
            if (moitie)
            {
                Vector2 positionCercleExterieur = cercleGO.transform.position;
                return (((newPosition.x - positionCercleExterieur.x) / 0.8f));
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }
}
