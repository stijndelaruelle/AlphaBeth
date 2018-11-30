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
    private List<Player> m_Characters; //For now this is only the player, change to a more generic "character" once we start addng enemies

    private bool m_IsExit = false;
    public bool IsExit
    {
        get { return m_IsExit; }
    }

    private void Awake()
    {
        m_Characters = new List<Player>();
    }


    private void UpdateVisualText()
    {
        //Function that manages all the visual stuff, so we don't spread this all over the place
        if (m_Text == null)
            return;

        m_Text.enabled = (m_Characters.Count == 0);
        m_Text.text = m_TextCharacter.ToString();

        if (m_IsExit) { m_Text.color = Color.yellow; }
    }

    public void SetTextCharacter(char textChar)
    {
        m_TextCharacter = textChar;
        UpdateVisualText();
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


    public void AddCharacter(Player player)
    {
        m_Characters.Add(player);
        UpdateVisualText();
    }

    public void RemoveCharacter(Player player)
    {
        m_Characters.Remove(player);
        UpdateVisualText();
    }


    //Temp, just to visualise where the exit is, this should become a level object at some point (just like HackShield)
    public void SetExit(bool state)
    {
        m_IsExit = state;
        UpdateVisualText();
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
