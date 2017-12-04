using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Reciever
{
	bool recieve ();
}

public class Ammo : MonoBehaviour
{
	public Material startMaterial;
	public Material endMaterial;

	private Renderer rend_;
	private AudioSource aSource_;


	public float rechargeTime = 1.0f;
	public float timeSinceLastGive = 2.0f;

	// Use this for initialization
	void Start ()
	{
		aSource_ = GetComponent<AudioSource>();

		rend_ = GetComponent<Renderer>();
		rend_.material = startMaterial;
	}
	
	// Update is called once per frame
	void Update ()
	{
		timeSinceLastGive += Time.deltaTime;

		rend_.material.Lerp( startMaterial, endMaterial, timeSinceLastGive / rechargeTime );
	}

	private void OnTriggerStay ( Collider other )
	{
		bool didRecieve = false;
		if ( timeSinceLastGive > rechargeTime )
		{
			foreach ( var reciever in other.gameObject.GetComponents<Reciever>() )
			{
				if (reciever.recieve())
				{
					didRecieve = true;
				}
			}

			foreach ( var reciever in other.gameObject.GetComponentsInChildren<Reciever>() )
			{
				if (reciever.recieve())
				{
					didRecieve = true;
				}
			}

			if (didRecieve)
			{
				aSource_.Play();
				timeSinceLastGive = 0.0f;
			}
		}
	}

	private void OnTriggerEnter ( Collider other )
	{

	}

}
