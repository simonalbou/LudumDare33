using UnityEngine;
using System.Collections;

public class TexteDegats : MonoBehaviour 
{
    public float speed;
    public float time;
    private float timer = 0f;

    private TextMesh txtmsh;
    private Transform self;
    private Color clr;

	// Use this for initialization
	void Start () 
    {
        txtmsh = GetComponent<TextMesh>();
        self = transform;
        clr = txtmsh.color;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (timer <= time)
        {
            float offset = (Time.deltaTime/timer);
            clr = txtmsh.color;
            txtmsh.color = new Color(clr.r, clr.g, clr.b, (clr.a - offset));
            timer += Time.deltaTime;
            self.position = new Vector2(self.position.x, self.position.y + speed * Time.deltaTime);
        }
	}
}
