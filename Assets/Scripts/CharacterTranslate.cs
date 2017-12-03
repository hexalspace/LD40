using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTranslate : MonoBehaviour
{
	public float speed = 10;

	// Use this for initialization
	void Start ()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float translation = Input.GetAxis( "Vertical" ) * speed * Time.deltaTime;
		float strafe = Input.GetAxis( "Horizontal" ) * speed * Time.deltaTime;

		transform.Translate( strafe, 0, translation );

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}
}
