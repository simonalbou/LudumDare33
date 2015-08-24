using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour 
{
    public float vitesseParallaxe = 0f;
    public float bouge = 0f;

	[Header("References")]
	public Transform bordGauche;
	public Transform bordDroit;
	public PlatformingInputHandler controller;

	public Transform tr;

	public Camera camera1;
    public Transform camTransform;
    public MainCameraBehaviour camBouge;
    private Vector2 oldPositionCamera;

	
	// Update is called once per frame
	void Update ()
    {
        if (Mathf.Sign((camTransform.position.x - oldPositionCamera.x)) > 0)
        {
            if (tr.position.x < bordGauche.position.x)
            {
                tr.position = new Vector2(tr.position.x + bouge, tr.position.y);
            }
        }
        if (Mathf.Sign((camTransform.position.x - oldPositionCamera.x)) < 0)
        {
            if (tr.position.x > bordDroit.position.x)
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
