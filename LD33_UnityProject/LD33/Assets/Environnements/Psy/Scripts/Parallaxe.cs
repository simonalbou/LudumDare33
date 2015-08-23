using UnityEngine;
using System.Collections;

public class Parallaxe : MonoBehaviour 
{
    public float vitesseParallaxe = 0f;
    private Transform tr;
    public PlatformingInputHandler controller;

	// Use this for initialization
	void Start () 
    {
        tr = transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        tr.position = new Vector2(tr.position.x + (controller.moveDir.x * vitesseParallaxe), tr.position.y);
	}
}
