using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public int nodeCount = 5;
    public int distance = 5;
    public Rigidbody2D startPin;
    public Rigidbody2D endPin;
    public Color directionEndColor;

    public Gradient gradient;

    List<Node> nodes;

	void Start () {
        nodes = new List<Node>(nodeCount);
		for(int i = 0; i < nodeCount; i++)
        {
            GameObject newNode = Instantiate(nodePrefab, new Vector3(Random.value*10 - 5, Random.value * 10 - 5), Quaternion.identity);
            Node node = newNode.GetComponent<Node>();
            node.index = i;
            nodes.Add(node);

            node.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float) i / nodeCount);
            Gradient colorGradient = new Gradient();

            Color startColor = gradient.Evaluate((float)i / nodeCount)*.8f;
            Color endColor;// = gradient.Evaluate((float)(i + 1) / nodeCount)*.8f;
            endColor = directionEndColor;


            colorGradient.colorKeys = new GradientColorKey[] { new GradientColorKey(startColor, 0), new GradientColorKey(endColor, 1) };
            node.GetComponent<LineRenderer>().colorGradient = colorGradient; 
            if(i > 0)
            {
                nodes[i - 1].target = nodes[i].transform;

                SpringJoint2D firstJoint = nodes[i - 1].gameObject.AddComponent<SpringJoint2D>();
                SpringJoint2D newJoint   = nodes[i]    .gameObject.AddComponent<SpringJoint2D>();

                firstJoint.connectedBody = newJoint  .gameObject.GetComponent<Rigidbody2D>();
                newJoint.connectedBody   = firstJoint.gameObject.GetComponent<Rigidbody2D>();

                firstJoint.autoConfigureDistance = false;
                newJoint  .autoConfigureDistance = false;

                firstJoint.distance = distance;
                newJoint  .distance = distance;

            }
        }
        nodes[nodes.Count - 1].GetComponent<LineRenderer>().enabled = false;
        if(startPin != null)
        {
            SpringJoint2D joint = nodes[0].gameObject.AddComponent<SpringJoint2D>();
            joint.connectedBody = startPin;
            joint.autoConfigureDistance = false;
            joint.distance = 0;
        }
        if(endPin != null)
        {
            SpringJoint2D joint = nodes[nodes.Count - 1].gameObject.AddComponent<SpringJoint2D>();
            joint.connectedBody = endPin;
            joint.autoConfigureDistance = false;
            joint.distance = 0;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
