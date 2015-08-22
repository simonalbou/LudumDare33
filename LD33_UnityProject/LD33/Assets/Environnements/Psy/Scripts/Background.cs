using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour 
{
    public GameObject bordGauche;
    public GameObject bordDroit;
    private Transform trBG;
    private Transform trBD;

    private Transform tr;

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
        if (tr.position.x < trBG.position.x)
        {

        }
	}
}
