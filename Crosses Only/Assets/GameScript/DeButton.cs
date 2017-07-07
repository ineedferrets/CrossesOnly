using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeButton : MonoBehaviour {

    public GameObject Canvas;

    public void enableDisableCanvas() {
        Canvas.SetActive(!Canvas.activeSelf);
        if (Canvas.activeSelf)
            Canvas.GetComponent<DrawingTrees>().drawTheTreePlease();
    }

}
