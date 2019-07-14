using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;
using System.IO;

public class NodeManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public int nodeCount = 5;
    public float distance = 5;
    public float frequency = 10;
    public Pin startPin;
    public Pin endPin;
    public Color directionEndColor;
    public Canvas canvas;
    public GameObject textPrefab;
    public GameObject pinPrefab;
    public GameObject nodeLinePrefab;

    public Gradient gradient;

    [HideInInspector]
    public string asmPath;

    List<Node> nodes = new List<Node>();

    public CommandManager commandManager;

    Dictionary<int, GameObject> pinLinks = new Dictionary<int, GameObject>();

    public static bool pulsed = false;

    Runnables runnables;

    private int targetASM;

    public Vector2 GradientRange = new Vector2(0, 100);

    //Coroutine runnable;

    [SerializeField]
    Dropdown dropdown;
    [SerializeField]
    InputField valueField;
    [SerializeField]
    InputField indexField;

    public void VisualizerUpdate(IEnumerable<int> positions, List<List<int>> connections, string message)
    {
        
        nodeCount = positions.Count();
        int i = 0;
        Dictionary<int, Vector3> existingPositions = new Dictionary<int, Vector3>();

        foreach (Node node in nodes)
        {
            if (positions.Contains(node.item))
            {
                existingPositions.Add(node.item, node.transform.position);
            }
            Destroy(node.gameObject);
        }
        
        nodes.Clear();
        Debug.Log("Updated");
        foreach (int item in positions)
        {
            if (existingPositions.ContainsKey(item))
            {
                GameObject newNode = createNodeObject(existingPositions[item], i, item);
                if (pinLinks.ContainsKey(item))
                {
                    AttachToPin(newNode, pinLinks[item].GetComponent<Pin>());
                }
            }
            else
            {
                createNodeObject(new Vector3(Random.value * 10 - 5, Random.value * 10 - 5), i, item);
            }
            i++;
        }

        LinkedList<int> toRemove = new LinkedList<int>();
        foreach (var item in pinLinks.Keys)
        {
            if (nodes.Where(x => x.item == item).Count() == 0)
            {
                toRemove.AddFirst(item);
            }
        }
        foreach (int item in toRemove)
        {
            Destroy(pinLinks[item]);
            pinLinks.Remove(item);
        }

        GenerateNodes(connections);
    }

    public static Text CreateScrollingText(GameObject textPrefab, GameObject canvas, string message)
    {
        Text text = Instantiate(textPrefab, canvas.transform).GetComponent<Text>();
        text.text = message;
        return text;
    }


    void Start () {
        commandManager = new CommandManager(VisualizerUpdate, dropdown, valueField, indexField, nodes);
        commandManager.llCreate();
	}

    void GenerateNodes(List<List<int>> connections)
    {
        for (int i = 0; i < nodeCount; i++)
        {

            foreach(int connectionIndex in connections[i])
            {
                AttachNodeLine(nodes[i], nodes[connectionIndex].transform);

                attach(nodes[i].gameObject, nodes[connectionIndex].gameObject);
                attach(nodes[connectionIndex].gameObject, nodes[i].gameObject);
            }
        }

        if (startPin != null && nodes.Count > 0)
        {
            AttachToPin(nodes[0].gameObject, startPin);
        }
        if (endPin != null && nodes.Count > 0)
        {
            AttachToPin(nodes[nodes.Count - 1].gameObject, endPin);
        }
    }

    public void AttachNodeLine(Node node, Transform target)
    {
        GameObject nodeLine = Instantiate(nodeLinePrefab, node.transform);
        var line = nodeLine.GetComponent<NodeLine>();
        line.target = target;
        line.CalculateLineRenderer();

        generateColorGradient(node, nodeLine.GetComponent<LineRenderer>());
    }

    public void Pin(Node holding, Vector3 position)
    {
        if (pinLinks.ContainsKey(holding.item))
        {
            return;
        }

        GameObject pin = Instantiate(pinPrefab, position, Quaternion.identity);
        Color targetColor = holding.GetComponent<SpriteRenderer>().color;
        pin.GetComponent<SpriteRenderer>().color = new Color(targetColor.r, targetColor.g, targetColor.b, 0.5f);
        pinLinks.Add(holding.item, pin);

        AttachToPin(holding.gameObject, pin.GetComponent<Pin>());
    }

    public void DeletePin(GameObject gameObject)
    {
        if (!gameObject.GetComponent<Pin>())
        {
            return;
        }

        LinkedList<int> keysToRemove = new LinkedList<int>();

        Rigidbody2D rigidbody = gameObject.GetComponent<Rigidbody2D>();

        foreach (var linkPair in pinLinks)
        {
            if (linkPair.Value != gameObject) continue;
            foreach(Node node in nodes)
            {
                foreach(SpringJoint2D sJ in node.GetComponents<SpringJoint2D>())
                {
                    if(sJ.connectedBody == rigidbody)
                    {
                        Destroy(sJ);
                    }
                }
            }
            keysToRemove.AddFirst(linkPair.Key);
        }

        for(var node = keysToRemove.First; node != null; node = node.Next)
        {
            pinLinks.Remove(node.Value);
        }

        Destroy(gameObject);
    }

    private void AttachToPin(GameObject node, Pin pin)
    {
        SpringJoint2D joint = attach(node, pin.gameObject);
        joint.frequency = 2;
        joint.distance = 0;
        pin.lineRenderer.enabled = true;
        pin.target = node.transform;
        pin.CalculateLineRenderer();
    }

    SpringJoint2D attach(GameObject start, GameObject target)
    {
        SpringJoint2D joint = start.AddComponent<SpringJoint2D>();
        joint.connectedBody = target.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.distance = distance;
        joint.frequency = frequency;

        return joint;
    }

    GameObject createNodeObject(Vector3 position, int index, int item)
    {
        GameObject newNode = Instantiate(nodePrefab, position, Quaternion.identity);
        Node node = newNode.GetComponent<Node>();
        node.index = index;
        node.item = item;
        node.GetComponentInChildren<TextMesh>().text = item.ToString();

        node.GetComponent<SpriteRenderer>().color = gradient.Evaluate((item - GradientRange.x) / (GradientRange.y - GradientRange.x));

        nodes.Add(node);

        return newNode;
    }

    void generateColorGradient(Node node, LineRenderer lineRenderer)
    {
        Gradient colorGradient = new Gradient();

        Color startColor = gradient.Evaluate((node.item - GradientRange.x) / (GradientRange.y - GradientRange.x)) * .8f;
        Color endColor;
        endColor = directionEndColor;

        colorGradient.colorKeys = new GradientColorKey[] { new GradientColorKey(startColor, 0), new GradientColorKey(endColor, 1) };
        lineRenderer.colorGradient = colorGradient;
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Return))
        {
            pulse();
        }
	}

    public void pulse()
    {
        pulsed = true;
        commandManager.pulse();
    }

    public void DropdownSelect()
    {
        commandManager.DropdownSelect();
    }
    
    private void OnDestroy()
    {
        Debug.Log("Deleted");
    }

    public void ChangeASM(Dropdown dropdown)
    {
        targetASM = dropdown.value;
        string targetASMname = "";
        //StopCoroutine(runnable);
        switch (targetASM)
        {
            case 0:
                targetASMname = "LinkedList";
                commandManager.llCreate();
                //runnable = StartCoroutine(runnables.SinglyLinked());
                break;
            case 1:
                targetASMname = "DCLL";
                commandManager.cdllCreate();
                //runnable = StartCoroutine(runnables.DoublyLinked());
                break;
            case 2:
                targetASMname = "Tree";
                commandManager.tCreate();
                //runnable = StartCoroutine(runnables.Tree());
                break;
        }

        VisualizerUpdate(new int[0], new List<List<int>>() { }, $"Switched to {targetASMname}");
    }
}
