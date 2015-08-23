using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour 
{
    public GameObject bordGauche;
    public GameObject bordDroit;
    public PlatformingInputHandler controller;
    private Transform trBG;
    private Transform trBD;

    private Transform tr;
    public float vitesseParallaxe = 0f;
    public float bouge = 0f;

    public Camera camera1;
    private Transform camTransform;
    private MainCameraBehaviour camBouge;
    private Vector2 oldPositionCamera;

	// Use this for initialization
	void Start () 
    {
        tr = transform;
        trBG = bordGauche.transform;
        trBD = bordDroit.transform;

        camBouge = camera1.GetComponent<MainCameraBehaviour>();
        camTransform = camera1.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Mathf.Sign((camTransform.position.x - oldPositionCamera.x)) > 0)
        {
            if (tr.position.x < trBG.position.x)
            {
                tr.position = new Vector2(tr.position.x + bouge, tr.position.y);
            }
        }
        if (Mathf.Sign((camTransform.position.x - oldPositionCamera.x)) < 0)
        {
            if (tr.position.x > trBD.position.x)
            {
                tr.position = new Vector2(tr.position.x - bouge, tr.position.y);
            }
        }

        if (camBouge.mustFollowPlayer)
        {
            tr.position = new Vector2(tr.position.x + (-(camTransform.position.x - oldPositionCamera.x) * vitesseParallaxe * Time.deltaTime), tr.position.y);
        }
        oldPositionCamera = camTransform.position;
	}
}
