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

	private AudioSource aSource_;

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
		if ( hit_ )
		{
			hitForHowLong_ += Time.deltaTime;
		}

		rend_.material.Lerp( startMaterial, endMaterial, hitForHowLong_/transitionTime );
	}

	void Hit()
	{
		if (hit_ != true)
		{
			aSource_.Play();
		}

		hit_ = true;
	}

	public bool isComplete ()
	{
		return hit_;
	}
}
