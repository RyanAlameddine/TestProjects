using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;


using CommonVisInterface;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

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

    List<Node> nodes = new List<Node>();

    Dictionary<int, GameObject> pinLinks = new Dictionary<int, GameObject>();

    private object lockObject = new object();

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    public IEnumerator VisualizerUpdate(IEnumerable<int> positions, List<List<int>> connections, string message)
    {
        Text text = Instantiate(textPrefab, canvas.transform).GetComponent<Text>();
        text.text = message;
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
        yield return null;
    }


    void Start () {

        var asm = Assembly.LoadFrom(@"\\GMRDC1\Folder Redirection\Ryan.Alameddine\Documents\Visual Studio 2017\Projects\DataStructures\ListProjects\ListProjects\bin\Debug\ListProjects.exe");

        //var listOfTypes = asm.GetTypes().Where(x => x.GetInterface(typeof(IVisualizable<int>).Name) != null).Select(x => (IVisualizable<int>)System.Activator.CreateInstance(x.MakeGenericType(typeof(int)))).ToList();
        

        IRunnable<int> runnable = asm.GetTypes().Where(x => x.GetInterface(typeof(IRunnable<int>).Name) != null).Select(x => (IRunnable<int>)System.Activator.CreateInstance(x)).First();

        runnable.Visualizable.OnUpdate += (positions, connections, message) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(VisualizerUpdate(positions, connections, message));

            lock (lockObject)
            {
                Monitor.Wait(lockObject);
            }
        };
        Thread t = new Thread(() =>
        {
            runnable.Run(tokenSource.Token).Wait();
        });

        t.Start();
	}

    void GenerateNodes(List<List<int>> connections)
    {
        for (int i = 0; i < nodeCount; i++)
        {

            foreach(int connectionIndex in connections[i])
            {
                Debug.Log(connectionIndex);
                AttachNodeLine(nodes[i], nodes[connectionIndex].transform);

                attach(nodes[i].gameObject, nodes[connectionIndex].gameObject);
                attach(nodes[connectionIndex].gameObject, nodes[i].gameObject);
            }

            /*
            if (i > 0)
            {
                AttachNodeLine(nodes[i - 1], nodes[i].transform);

                attach(nodes[i - 1].gameObject, nodes[i].gameObject);
                attach(nodes[i].gameObject, nodes[i - 1].gameObject);
            }*/
        }

        //nodes[nodes.Count - 1].GetComponent<LineRenderer>().enabled = false;
        if (startPin != null)
        {
            AttachToPin(nodes[0].gameObject, startPin);
        }
        if (endPin != null)
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
        GameObject pin = Instantiate(pinPrefab, position, Quaternion.identity);
        pin.GetComponent<SpriteRenderer>().color = holding.GetComponent<SpriteRenderer>().color;
        pinLinks.Add(holding.item, pin);

        AttachToPin(holding.gameObject, pin.GetComponent<Pin>());
    }

    public void DeletePin(GameObject gameObject)
    {
        pinLinks.Where(linkPair => linkPair.Value == gameObject).Select((linkPair) =>
        {
            nodes.Where(node => node.GetComponents<SpringJoint2D>().Where(sj => sj.connectedBody == gameObject).Select((joint) => 
            {
                Destroy(joint);
                return true;
            }));
            pinLinks.Remove(linkPair.Key);
            return linkPair.Key;
        });


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

        //node.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)index / nodeCount);
        node.GetComponent<SpriteRenderer>().color = gradient.Evaluate(((float)item)/int.MaxValue);

        nodes.Add(node);

        return newNode;
    }

    void generateColorGradient(Node node, LineRenderer lineRenderer)
    {
        Gradient colorGradient = new Gradient();

        //Color startColor = gradient.Evaluate((float)index / nodeCount) * .8f;
        Color startColor = gradient.Evaluate(((float)node.item) / int.MaxValue) * .8f;
        Color endColor;// = gradient.Evaluate((float)(i + 1) / nodeCount)*.8f;
        endColor = directionEndColor;

        colorGradient.colorKeys = new GradientColorKey[] { new GradientColorKey(startColor, 0), new GradientColorKey(endColor, 1) };
        lineRenderer.colorGradient = colorGradient;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            lock (lockObject)
            {
                Monitor.PulseAll(lockObject);
            }
        }
	}
    
    private void OnDestroy()
    {
        tokenSource?.Cancel();
        Debug.Log("Deleted");
    }
}
