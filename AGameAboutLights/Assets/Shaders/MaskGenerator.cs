using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskGenerator : MonoBehaviour {

    public Canvas canvas;

	void Start ()
    {
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.position = Vector2.zero;
	}
}
