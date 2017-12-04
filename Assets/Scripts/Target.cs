using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, Goal
{
	public Material startMaterial;
	public Material endMaterial;
	public float transitionTime = .5f;

	private Renderer rend_;
	private bool hit_ = false;
	private float hitForHowLong_ = 0.0f;


	// Use this for initialization
	void Start ()
	{
		rend_ = GetComponent<Renderer>();
		rend_.material = startMaterial;
	}

	// Update is called once per frame
	void Update ()
	{
		if ( hit_ )
		{
			hitForHowLong_ += Time.deltaTime;
		}

		rend_.material.Lerp( startMaterial, endMaterial, hitForHowLong_/transitionTime );
	}

	void Hit()
	{
		hit_ = true;
	}

	public bool isComplete ()
	{
		return hit_;
	}
}
