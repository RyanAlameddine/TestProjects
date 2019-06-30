using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera))]
public class MouseControls : MonoBehaviour {
    [SerializeField]
    GameObject holding;

    [SerializeField]
    PostProcessVolume volume;

    [SerializeField]
    NodeManager nodeManager;

    [SerializeField]
    float scrollSensitivity = 1;

    [SerializeField]
    float rotateVelocity = 0;

    float scrollVelocity = 0;
    Vector3 zoomPosition = Vector3.zero;
    float lerpPercent = 0.2f;

    Vector3 mousePos = Vector3.zero;

    Vector2? targetPosition = null;
    Vector2 startPosition = Vector2.zero;
    Vector2 clickedPosition = Vector2.zero;

    [SerializeField]
    float panMultiplier = 10;
	
	// Update is called once per frame
	void FixedUpdate () {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Grab objects
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);

            if (hit)
            {
                holding = hit.transform.gameObject;
                holding.GetComponent<Rigidbody2D>().isKinematic = true;
                Debug.Log(hit);
            }
        }
        else
        {
            //Delete Pins
            if (!Input.GetKey(KeyCode.Mouse0) && Input.GetKeyDown(KeyCode.Mouse1))
            {
                // Construct a ray from the current mouse coordinates
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);

                if (hit)
                {
                    nodeManager.DeletePin(hit.transform.gameObject);
                }
            }
        }

        //Scrolling smoothly
        float scrolled = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;

        scrollVelocity = Mathf.Lerp(scrollVelocity, scrolled, lerpPercent);

        if(Mathf.Abs(scrollVelocity - scrolled) < 0.001)
        {
            scrollVelocity = scrolled;
            targetPosition = transform.position;
        }

        if (Mathf.Abs(scrolled) > 0.5)
        {
            targetPosition = null;
            zoomPosition = mousePos;
        }
        if (targetPosition == null)
        {
            ZoomOrthoCamera(zoomPosition, scrollVelocity);
            volume.enabled = false;
        }
        else
        {
            volume.enabled = true;
        }
        
        //Holding objects and dragging and droppping
        if (holding != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                holding.transform.position = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition) - new Vector3(0, 0, transform.position.z);
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Node node = holding.GetComponent<Node>();
                    if (node)
                    {
                        nodeManager.Pin(node, holding.transform.position);
                    }
                }
            }
            else
            {
                holding.GetComponent<Rigidbody2D>().isKinematic = false;
                holding = null;
            }
        }

        if (Input.GetKey(KeyCode.Mouse3))
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, -rotateVelocity));
            volume.enabled = false;
        }else if (Input.GetKey(KeyCode.Mouse4))
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, rotateVelocity));
            volume.enabled = false;
        }

        //Pan camera
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            clickedPosition = mousePos;
            startPosition = new Vector2(transform.position.x, transform.position.y);
        }
        if (Input.GetKey(KeyCode.Mouse2))
        {
            zoomPosition = transform.position;
            targetPosition = (clickedPosition - new Vector2(mousePos.x, mousePos.y)) + startPosition;
        }
        else
        {
            clickedPosition = Vector2.zero;
        }

        if (targetPosition != null)
        {

            if (clickedPosition != Vector2.zero)
            {
                var lerped = Vector2.Lerp(targetPosition.Value, transform.position, .2f);
                lerped = Vector2.Lerp(lerped, transform.position, .2f);
                lerped = Vector2.Lerp(lerped, transform.position, .2f);
                transform.position = new Vector3(lerped.x, lerped.y, -10);
                Debug.Log(targetPosition);
            }
            else
            {
                var lerped = Vector2.Lerp(targetPosition.Value, transform.position, .8f);
                transform.position = new Vector3(lerped.x, lerped.y, -10);

                if (Mathf.Abs((new Vector2(transform.position.x, transform.position.y) - targetPosition.Value).sqrMagnitude) < 0.001)
                {
                    transform.position = new Vector3(targetPosition.Value.x, targetPosition.Value.y, -10);
                }
            }
        }
    }

    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        if(Camera.main.orthographicSize - amount <= 0)
        {
            return;
        }

        // Calculate how much we will have to move towards the zoomTowards position
        float multiplier = (1.0f / Camera.main.orthographicSize * amount);

        // Move camera
        transform.position += (zoomTowards - transform.position) * multiplier;

        // Zoom camera
        Camera.main.orthographicSize -= amount;
        
    }
}
