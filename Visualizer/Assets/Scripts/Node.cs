﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    
    public int index;
    public Transform target;

    public Rigidbody2D rigidBody;

    LineRenderer lineRenderer;
    

	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, .1f);
        curve.AddKey(.4f, .1f);
        curve.AddKey(.4001f, .7f);
        curve.AddKey(.5f, .1f);
        curve.AddKey(1, .1f);
        lineRenderer.widthCurve = curve;
    }
	
	// Update is called once per frame
	void Update () {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, Vector3.Lerp(transform.position, target.position, .4f));
            lineRenderer.SetPosition(2, Vector3.Lerp(transform.position, target.position, .4001f ));
            lineRenderer.SetPosition(3, Vector3.Lerp(transform.position, target.position, .5f));
            lineRenderer.SetPosition(4, target.position);
        }
	}
}
