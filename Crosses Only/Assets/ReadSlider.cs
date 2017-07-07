using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadSlider : MonoBehaviour {

    public Slider slider;
	
	void Update () {
        string value = slider.value.ToString();
        if (value.Length > 4)
            value = value.Substring(0, 4);
        transform.GetComponent<Text>().text = value;
	}
}
