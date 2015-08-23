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

	// Use this for initialization
	void Start () 
    {
        tr = transform;
        trBG = bordGauche.transform;
        trBD = bordDroit.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        tr.position = new Vector2(tr.position.x + (-controller.moveDir.x * vitesseParallaxe), tr.position.y);
        if (Mathf.Sign(controller.moveDir.x) > 0)
        {
            if (tr.position.x < trBG.position.x)
            {
                tr.position = new Vector2(tr.position.x + 38.4f, tr.position.y);
            }
        }
        if (Mathf.Sign(controller.moveDir.x) < 0)
        {
            if (tr.position.x > trBD.position.x)
            {
                tr.position = new Vector2(tr.position.x - 38.4f, tr.position.y);
            }
        }
	}
}
