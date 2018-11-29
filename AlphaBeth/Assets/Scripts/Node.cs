using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

public class Node : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Text;

    private char m_TextCharacter = '\0';
    private Node[] m_Neighbours = new Node[4]; //Currently 4, could become 8


    public void SetTextCharacter(char textChar)
    {
        m_TextCharacter = textChar;
        m_Text.text = m_TextCharacter.ToString();
    }

    public char GetTextCharacter()
    {
        return m_TextCharacter;
    }


    public void SetNeighbour(Direction direction, Node neighbour)
    {
        m_Neighbours[(int)direction] = neighbour;
    }

    public Node GetNeighbour(Direction direction)
    {
        return m_Neighbours[(int)direction];
    }

    public char GetNeighbourCharacter(Direction direction)
    {
        Node neighbour = m_Neighbours[(int)direction];

        if (neighbour == null)
            return '\0';

        return neighbour.GetTextCharacter();
    }

    private void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Color[] colors = new Color[] { Color.green, Color.red, Color.blue, Color.yellow };
        Vector3[] offset = new Vector3[] { new Vector3(-0.05f, 0.0f, 0.0f), new Vector3(0.0f, -0.05f, 0.0f), new Vector3(0.05f, 0.0f, 0.0f), new Vector3(0.0f, 0.05f, 0.0f) };
        for (int i = 0; i < m_Neighbours.Length; ++i)
        {
            Node neighbour = m_Neighbours[i];

            if (neighbour != null)
            {
                Gizmos.color = colors[i];
                Gizmos.DrawLine(transform.position + offset[i], neighbour.transform.position + offset[i]);
            }
        }
        
    }
}
