using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLine : MonoBehaviour {

    LineRenderer lineRenderer;
    public Transform target;

    void Awake () {
        lineRenderer = GetComponent<LineRenderer>();

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, .1f);
        curve.AddKey(.4f, .1f);
        curve.AddKey(.4001f, .5f);
        curve.AddKey(.5f, .1f);
        curve.AddKey(1, .1f);
        lineRenderer.widthCurve = curve;
    }

    public void CalculateLineRenderer()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, Vector3.Lerp(transform.position, target.position, .4f));
        lineRenderer.SetPosition(2, Vector3.Lerp(transform.position, target.position, .4001f));
        lineRenderer.SetPosition(3, Vector3.Lerp(transform.position, target.position, .5f));
        lineRenderer.SetPosition(4, target.position);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateLineRenderer();
    }
}
