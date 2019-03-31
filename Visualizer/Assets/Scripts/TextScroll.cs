using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScroll : MonoBehaviour {

    Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(0, 30 * Time.deltaTime, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - .3f * Time.deltaTime);
	}
}
