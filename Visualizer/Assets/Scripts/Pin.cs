using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour {

    [HideInInspector]
    public LineRenderer lineRenderer;

    [HideInInspector]
    public Transform target;
    
	void Awake () {
        lineRenderer = GetComponent<LineRenderer>();
	}

    public void CalculateLineRenderer()
    {
        if (target != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.position);
        }
    }
    
    void Update () {
        CalculateLineRenderer();

        transform.rotation = Camera.main.transform.rotation;
	}
}
