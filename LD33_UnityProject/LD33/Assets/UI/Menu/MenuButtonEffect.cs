using UnityEngine;
using System.Collections;

public class MenuButtonEffect : MonoBehaviour {

	public string sceneToLoad;
	public GameObject objectToEnable;
	public GameObject objectToDisable;
	public bool quitsApplication;

	public void ActivateButton()
	{
		if (objectToEnable) EnableGameObject();
		if (objectToDisable) DisableGameObject();
		if (sceneToLoad != "") LoadScene();
		if (quitsApplication) QuitApplication();
	}

	public void LoadScene()
	{
		Application.LoadLevel(sceneToLoad);
	}

	public void EnableGameObject()
	{
		objectToEnable.SetActive(true);
	}

	public void DisableGameObject()
	{
		objectToDisable.SetActive(false);
	}

	public void QuitApplication()
	{
		Application.Quit();
	}

}
