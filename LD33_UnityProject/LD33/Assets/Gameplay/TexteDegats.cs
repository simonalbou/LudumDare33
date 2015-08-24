using UnityEngine;
using System.Collections;

public class TexteDegats : MonoBehaviour 
{
    private TextMesh txtmsh;
    private Transform self;
    private Color clr;

    [Header("Vitesse Verticale")]
    public float speed;
    private float timer = 0f;

    [Header("Changement scale")]
    public float minimumScale = 0.0f;
    public float maximumScale = 1f;
    public float durationScale = 5.0f;
    private float startTimeScale;
    private bool canChangeAlpha = false;
    public AnimationCurve curveScale;

    [Header("Changement alpha")]
    public float minimum = 0.0f;
    public float maximum = 1f;
    public float duration = 5.0f;
    private float startTimeAlpha;
    public AnimationCurve curveAlpha;


	// Use this for initialization
	void Start () 
    {
        txtmsh = GetComponent<TextMesh>();
        self = transform;
        clr = txtmsh.color;

        startTimeScale = Time.time;

        //StartCoroutine(FadeOut(time));
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!canChangeAlpha)
        {
            float time = (Time.time - startTimeAlpha) / durationScale;
            self.localScale = new Vector2(Mathf.Lerp(minimumScale, maximumScale, curveScale.Evaluate(time)), Mathf.Lerp(minimumScale, maximumScale, curveScale.Evaluate(time)));
            if (time >= 1f)
            {
                startTimeAlpha = Time.time;
                canChangeAlpha = true;
            }
        }

        if (canChangeAlpha)
        {
            float t = (Time.time - startTimeAlpha) / duration;
            txtmsh.color = new Color(clr.r, clr.g, clr.b, Mathf.Lerp(minimum, maximum, curveAlpha.Evaluate(t)));
        }

	    if (timer <= duration + durationScale)
        {
            float delta = Time.deltaTime;
            timer += delta;
            self.position = new Vector2(self.position.x, self.position.y + speed * delta); 
       }
	}
}
