using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeValueRotator : MonoBehaviour
{
    Transform camTrans;
    void Start()
    {
        camTrans = Camera.main.transform;
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, camTrans.rotation, 0.02f);
    }
}
