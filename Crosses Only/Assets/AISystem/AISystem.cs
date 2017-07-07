using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AISystem : MonoBehaviour {

    public GameObject[] buttons;
    public Node root;
    private System.Random r = new System.Random(1337);
    public GameObject iterationSlider;
    public GameObject constantSlider;

	public void takeTurn(byte[] state) {
        root = new Node(null, 0, state, 1, 0);

        for ( int i = 0; i < (int)iterationSlider.GetComponent<Slider>().value; i++) {
            // Tree policy (find node to expand)
            Node current = childSelect(root);

            // Default policy (unformly random chosen moves to check eligibility)
            int value = rollout(current);

            // Update values
            nodeUpdate(current, value);
        }

        buttons[bestChildUCB(root, 0).action].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
    }

    // Get available moves, if more availble moves than children, expand
    private Node childSelect(Node current) {
        while (!isTerminal(current.state)) {
            List<byte> availableMoves = getAvailableMoves(current.state);
            if (availableMoves.Count > current.children.Count)
                return expand(current);
            else
                current = bestChildUCB(current, (double)constantSlider.GetComponent<Slider>().value);
        }
        return current;

    }

    // Returns list of all cells that could be chosen
    private List<byte> getAvailableMoves(byte[] state) {
        List<byte> moves = new List<byte>();

        for (byte i = 0; i < 27; i++)
            if (state[i] == 0)
                moves.Add(i);

        return moves;
    }

    // Chooses an available move that has not been expanded to expand on
    private Node expand(Node current) {
        List<byte> availableMoves = getAvailableMoves(current.state);

        for ( int i = 0; i < availableMoves.Count; i++) {
            if (current.children.Exists(a => a.action == availableMoves[i]))
                continue;

            int playerActing = current.player * -1;

            byte[] tempState = new byte[27];
            System.Array.Copy(current.state, tempState, 27);
            tempState[availableMoves[i]] = 1;
            //checkCross(tempState);

            Node node = new Node(current, availableMoves[i], tempState, playerActing, current.depth + 1);
            current.children.Add(node);

            return node;
        }
        throw new System.Exception("Child not found");
    }

    // Best node to expand is decided by the Upper Confidence Bound (UCB)
    private Node bestChildUCB(Node current, double C) {
        Node bestChild = null;
        double best = double.NegativeInfinity;

        foreach (Node child in current.children) {
            double UCB1 = ((double)child.value / (double)child.visits) + C * System.Math.Sqrt((2.0 * System.Math.Log((double)current.visits) / (double)child.visits));

            if ( UCB1 > best) {
                bestChild = child;
                best = UCB1;
            }
        }

        return bestChild;

    }

    // Potential win or lose outcome decided from node via uniform random choices of available moves
    private int rollout(Node current) {
        byte[] tempState = new byte[27];
        System.Array.Copy(current.state, tempState, 27);

        int player = current.player;

        while (!isTerminal(tempState)) {
            List<byte> moves = getAvailableMoves(tempState);
            byte move = moves[r.Next(0, moves.Count)];
            tempState[move] = 1;
            player *= -1;
        }

        return player == 1 ? 1 : 0;
    }

    // Check if board is won
    private bool isTerminal(byte[] state)
    {
        byte[] tempState = new byte[27];
        System.Array.Copy(state, tempState, 27);

        checkCross(tempState);

        byte value = 1;
        for (int i = 0; i < 27; i++)
            value *= tempState[i];
        return value == 1;
    }

    // Backpropagation
    private void nodeUpdate(Node current, int value) {

        do
        {
            current.visits++;
            current.value += value;
            current = current.parent;
        } while (current != null);
    }

    private byte[] checkCross(byte[] state) {
        for (int j = 0; j < 3; j++){
            bool cross = false;
            for (int i = 0; i < 9; i++) {
                switch (i) {
                    case 2:
                        if ((state[j*9 + 0] == 1) && state[j * 9 + 1] == 1 && state[j * 9 + 2] == 1)
                            cross =  true;
                        break;
                    case 5:
                        if (state[j * 9 + 3] == 1 && state[j * 9 + 4] == 1 && state[j * 9 + 5] == 1)
                            cross =  true;
                        break;
                    case 6:
                        if (state[j * 9 + 0] == 1 && state[j * 9 + 3] == 1 && state[j * 9 + 6] == 1)
                            cross =  true;
                        if (state[j * 9 + 2] == 1 && state[j * 9 + 4] == 1 && state[j * 9 + 6] == 1)
                            cross =  true;
                        break;
                    case 7:
                        if (state[j * 9 + 1] == 1 && state[j * 9 + 4] == 1 && state[j * 9 + 7] == 1)
                            cross =  true;
                        break;
                    case 8:
                        if (state[j * 9 + 2] == 1 && state[j * 9 + 5] == 1 && state[j * 9 + 8] == 1)
                            cross = true;
                        if (state[j * 9 + 0] == 1 && state[j * 9 + 4] == 1 && state[j * 9 + 8] == 1)
                            cross = true;
                        if (state[j * 9 + 6] == 1 && state[j * 9 + 7] == 1 && state[j * 9 + 8] == 1)
                            cross = true;
                        break;
                    default:
                        break;
                }
                if (cross == true)
                    break;
            }
            if (cross == true)
            {
                for (int i = 0; i < 9; i++) {
                    state[j * 9 + i] = 1;
                }
            }
        }
        return state;
    }

}

public class Node {

    public int visits = 0;
    public int value = 0;
    public int action = 0;
    public int depth = 0;
    public byte[] state = new byte[27];
    public int player = 0;
    public List<Node> children = new List<Node>();
    public Node parent = null;

    public GameObject drawnNode;

    public Node(Node parent, int action, byte[] state, int player, int depth) {
        this.state = state;
        this.player = player;
        this.parent = parent;
        this.action = action;
        this.depth = depth;
    }

}
