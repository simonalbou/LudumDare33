using UnityEngine;
using System.Collections;

public class Parallaxe : MonoBehaviour 
{
    public float vitesseParallaxe = 0f;
    private Transform tr;
    public PlatformingInputHandler controller;

    public Camera camera1;
    private Transform camTransform;
    private MainCameraBehaviour camBouge;
    private Vector2 oldPositionCamera;

	// Use this for initialization
	void Start () 
    {
        tr = transform;

        camBouge = camera1.GetComponent<MainCameraBehaviour>();
        camTransform = camera1.transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (camBouge.mustFollowPlayer)
        {
            tr.position = new Vector2(tr.position.x + (-(camTransform.position.x - oldPositionCamera.x) * vitesseParallaxe * Time.deltaTime), tr.position.y);
        }
        oldPositionCamera = camTransform.position;
	}
}
