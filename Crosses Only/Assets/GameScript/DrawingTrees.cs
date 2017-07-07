using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DrawingTrees : MonoBehaviour {

    public GameObject defaultNode;
    public GameObject defaultLine;
    public GameObject canvasObj;
    public AISystem theAI;
    public static GameObject rootNode;
    IEnumerable<GameObject> treeCollection;

    private Vector3 colorDifference;

    public void Awake() { colorDifference = new Vector3(Color.blue.r - Color.red.r, Color.blue.b - Color.red.b, Color.blue.g - Color.red.g); Debug.Log(colorDifference); drawTheTreePlease(); }

    public void drawTheTreePlease() {

        if (theAI.root == null)
            return;

        Node root = theAI.root;

        Vector2 rootPos = new Vector2(0, 0.3f*canvasObj.GetComponent<RectTransform>().rect.height);

        if (rootNode)
            destroyAllChildren(rootNode);

        rootNode = Instantiate(defaultNode);
        rootNode.GetComponent<NodeProperties>().setParams(root, Enumerable.Empty<GameObject>(), rootPos, transform.gameObject);
        treeCollection = new[] { rootNode };


        treeCollection.Concat(exploreNode(rootNode, 0.7f * Screen.width, 0.1f * Screen.height));

    }

    public IEnumerable<GameObject> exploreNode(GameObject nodeObj, float width, float heightInterval) {

        Node node = nodeObj.GetComponent<NodeProperties>().mCTSNode;

        if (node.children.Count == 0)
            return Enumerable.Empty<GameObject>();

        float interval = width / node.children.Count;
        Vector2 centre = new Vector2(nodeObj.transform.localPosition.x, nodeObj.transform.localPosition.y);
        IEnumerable<GameObject> returnEnum = Enumerable.Empty<GameObject>();
        GameObject tempTreeNode;

        for (int i = 0; i < node.children.Count; i++) {
            if (!treeCollection.Any(a => a.GetComponent<NodeProperties>().mCTSNode == node.children[i]))
            {
                Vector2 childCentre = new Vector2((width / 2) - interval * (i + 0.5f), -heightInterval);

                tempTreeNode = Instantiate(defaultNode);
                tempTreeNode.GetComponent<NodeProperties>().setParams(node.children[i], Enumerable.Empty<GameObject>(), childCentre, nodeObj);
                Debug.Log((float)tempTreeNode.GetComponent<NodeProperties>().mCTSNode.value / (float)tempTreeNode.GetComponent<NodeProperties>().mCTSNode.visits);
                float gradiation = Mathf.Clamp((float)tempTreeNode.GetComponent<NodeProperties>().mCTSNode.value / (float)tempTreeNode.GetComponent<NodeProperties>().mCTSNode.visits, 0.0f, 1.0f);
                tempTreeNode.GetComponent<Image>().color = new Color(Color.red.r + gradiation * colorDifference.x, Color.red.b + gradiation * colorDifference.y, Color.red.g + gradiation * colorDifference.z);

                GameObject tempLine = Instantiate(defaultLine);
                tempLine.transform.parent = tempTreeNode.transform;
                tempLine.transform.localPosition = new Vector3(-childCentre.x / 2, -childCentre.y / 2);
                tempLine.transform.localScale = new Vector3(1 + 2 * gradiation, Mathf.Sqrt(Mathf.Pow(childCentre.x, 2) + Mathf.Pow(childCentre.y, 2)));
                tempLine.transform.rotation = Quaternion.Euler(0, 0, -180 * Mathf.Atan(childCentre.x / childCentre.y) / Mathf.PI);
                tempLine.GetComponent<Image>().color = new Color(Color.red.r + gradiation * colorDifference.x, Color.red.b + gradiation * colorDifference.y, Color.red.g + gradiation * colorDifference.z);

                tempTreeNode.GetComponent<NodeProperties>().visualParentLine = tempLine;

                returnEnum.Concat(new[] { tempTreeNode });
            }
            else {
                tempTreeNode = treeCollection.Where(a => a.GetComponent<NodeProperties>().mCTSNode == node.children[i]).ElementAt(0);
            }
            tempTreeNode.GetComponent<NodeProperties>().children.Concat(exploreNode(tempTreeNode, interval, heightInterval));
        }

        return returnEnum;
    }

    public void destroyAllChildren(GameObject node) {
        if (node.GetComponent<NodeProperties>().children.Count() == 0)
            Destroy(node); return;

        foreach (GameObject child in node.GetComponent<NodeProperties>().children)
            destroyAllChildren(child);

        Destroy(node); return;
    }

    public struct TreeNodes
    {
        public Vector2 localPosition;
        public Node mCSTNode;
        public GameObject visualNode;
        public GameObject visualParentLine;
        public IEnumerable<TreeNodes> children;

        public TreeNodes(Vector2 localPosition, Node mCSTNode, GameObject visualNode, GameObject canvasParent, GameObject visualParentLine) {
            this.localPosition = localPosition;
            this.mCSTNode = mCSTNode;
            this.visualNode = visualNode;
            visualNode.transform.parent = canvasParent.transform;
            visualNode.transform.localPosition = new Vector3( localPosition.x, localPosition.y);
            this.children = new[] { new TreeNodes() };
            this.visualParentLine = visualParentLine;
        }
    }

    public void destroyTree() {
        if (rootNode != null)
            destroyAllChildren(rootNode);
    }

}

