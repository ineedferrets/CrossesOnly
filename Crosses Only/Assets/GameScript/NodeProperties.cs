using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeProperties : MonoBehaviour {

    public Node mCTSNode;
    public byte[] pubState = new byte[27];
    public GameObject visualParentLine;
    public IEnumerable<GameObject> children;

    private IPointerEnterHandler mouseHandler;

	public void setParams(Node mCTSNode, IEnumerable<GameObject> children, Vector2 localPos, GameObject UIparent) {
        this.mCTSNode = mCTSNode;
        this.children = children;
        transform.SetParent(UIparent.transform);
        transform.localPosition = new Vector3(localPos.x, localPos.y);
        pubState = mCTSNode.state;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log(mCTSNode.state);
    }

    public void OnMouseEnter() {

    }

}
