using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseControls : MonoBehaviour {
    [SerializeField]
    GameObject holding;

    [SerializeField]
    NodeManager nodeManager;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Construct a ray from the current mouse coordinates
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);

            if (hit)
            {
                holding = hit.transform.gameObject;
                holding.GetComponent<Rigidbody2D>().isKinematic = true;
                Debug.Log(hit);
            }
        }

        if (holding != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                holding.transform.position = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition) - new Vector3(0, 0, transform.position.z);
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    nodeManager.Pin(holding.GetComponent<Node>(), holding.transform.position);
                }
            }
            else
            {
                holding.GetComponent<Rigidbody2D>().isKinematic = false;
                holding = null;
            }
        }
    }
}
