using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScroll : MonoBehaviour {

    Text text;
    float seconds = 0;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //transform.position += new Vector3(0, 30 * Time.deltaTime, 0);
        //text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - .3f * Time.deltaTime);
        seconds += Time.fixedDeltaTime;
        if(seconds > 5)
        {
            Destroy(gameObject);
        }
	}
}
