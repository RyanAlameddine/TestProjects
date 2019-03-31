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
    public Rigidbody2D startPin;
    public Rigidbody2D endPin;
    public Color directionEndColor;
    public Canvas canvas;
    public GameObject textPrefab;

    public Gradient gradient;

    List<Node> nodes = new List<Node>();

    private object lockObject = new object();

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    public IEnumerator VisualizerUpdate(IEnumerable<int> positions, string message)
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
            if(existingPositions.ContainsKey())
            createNodeObject(new Vector3(Random.value * 10 - 5, Random.value * 10 - 5), i, item);
            i++;
        }
        GenerateNodes(false);
        yield return null;
    }

    void Start () {

        var asm = Assembly.LoadFrom(@"\\GMRDC1\Folder Redirection\Ryan.Alameddine\Documents\Visual Studio 2017\Projects\DataStructures\ListProjects\ListProjects\bin\Debug\ListProjects.exe");

        //var listOfTypes = asm.GetTypes().Where(x => x.GetInterface(typeof(IVisualizable<int>).Name) != null).Select(x => (IVisualizable<int>)System.Activator.CreateInstance(x.MakeGenericType(typeof(int)))).ToList();

        GenerateNodes();

        IRunnable<int> runnable = asm.GetTypes().Where(x => x.GetInterface(typeof(IRunnable<int>).Name) != null).Select(x => (IRunnable<int>)System.Activator.CreateInstance(x)).First();

        runnable.Visualizable.OnUpdate += (positions, message) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(VisualizerUpdate(positions, message));

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

    void GenerateNodes(bool createNodes = true)
    {
        for (int i = 0; i < nodeCount; i++)
        {
            if (createNodes)
            {
                createNodeObject(new Vector3(Random.value * 10 - 5, Random.value * 10 - 5), i, (int)Random.Range(int.MinValue, int.MaxValue));
            }

            generateColorGradient(nodes[i], i);

            if (i > 0)
            {
                nodes[i - 1].target = nodes[i].transform;

                attach(nodes[i - 1].gameObject, nodes[i].gameObject);
                attach(nodes[i].gameObject, nodes[i - 1].gameObject);
            }
        }

        nodes[nodes.Count - 1].GetComponent<LineRenderer>().enabled = false;
        if (startPin != null)
        {
            SpringJoint2D joint = attach(nodes[0].gameObject, startPin.gameObject);
            joint.frequency = 2;
            joint.distance = 0;
        }
        if (endPin != null)
        {
            SpringJoint2D joint = attach(nodes[nodes.Count - 1].gameObject, endPin.gameObject);
            joint.frequency = 2;
            joint.distance = 0;
        }
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

        node.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)index / nodeCount);

        nodes.Add(node);

        return newNode;
    }

    void generateColorGradient(Node node, int index)
    {
        Gradient colorGradient = new Gradient();

        Color startColor = gradient.Evaluate((float)index / nodeCount) * .8f;
        Color endColor;// = gradient.Evaluate((float)(i + 1) / nodeCount)*.8f;
        endColor = directionEndColor;

        colorGradient.colorKeys = new GradientColorKey[] { new GradientColorKey(startColor, 0), new GradientColorKey(endColor, 1) };
        node.GetComponent<LineRenderer>().colorGradient = colorGradient;
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
