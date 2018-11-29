using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public delegate void LevelDelegate();

    [SerializeField]
    private float m_TileSize;

    [Header("Required References")]
    [SerializeField]
    private Node m_NodePrefab;
    private List<Node> m_Nodes;

    private Node m_StartNode;
    public Node StartNode
    {
        get { return m_StartNode; }
    }

    private Node m_EndNode;
    public Node EndNode
    {
        get { return m_EndNode; }
    }

    private Coroutine m_CurrentRoutine;

    public event LevelDelegate LevelGeneratedEvent;
    

    public void GenerateGridLevel(int width, int height, string availableCharacters, int seed)
    {
        if (m_NodePrefab == null)
        {
            Debug.LogWarning("Please assign a node prefab.");
            return;
        }

        if (m_CurrentRoutine != null)
            StopCoroutine(m_CurrentRoutine);

        m_CurrentRoutine = StartCoroutine(GenerateGridLevelRoutine(width, height, availableCharacters, seed));
    }

    private IEnumerator GenerateGridLevelRoutine(int width, int height, string availableTextCharacters, int seed)
    {
        if (m_Nodes == null) { m_Nodes = new List<Node>(); }
        else { ClearLevel(); }

        //Create all the nodes
        for (int i = 0; i < width * height; ++i)
        {
            int x = (i % width);
            int y = (i / width);

            Node newNode = GameObject.Instantiate<Node>(m_NodePrefab);

            newNode.name = "Node (" + x + ", " + y + ")";
            newNode.transform.parent = transform;
            newNode.transform.position = new Vector3(x * m_TileSize, -y * m_TileSize, 0.0f); //0, 0 in the top left

            m_Nodes.Add(newNode);
        }

        //Link all the nodes
        for (int i = 0; i < m_Nodes.Count; ++i)
        {
            Node currentNode = m_Nodes[i];

            int x = (i % width);
            int y = (i / width);

            //Assign right neighbour (and assign their left to us)
            if (x >= 0 && x < width - 1)
            {
                Node rightNeighbour = m_Nodes[i + 1];
                currentNode.SetNeighbour(Direction.East, rightNeighbour);
                rightNeighbour.SetNeighbour(Direction.West, currentNode);
            }

            //Assign bottom neighbour (and assign their top to us)
            if (y >= 0 && y < height - 1)
            {
                Node bottomNeighbour = m_Nodes[i + width];
                currentNode.SetNeighbour(Direction.South, bottomNeighbour);
                bottomNeighbour.SetNeighbour(Direction.North, currentNode);
            }
        }

        //Assign node characters
        AssignNodeTextCharacters(availableTextCharacters, seed);

        //Assign start & end nodes
        m_StartNode = m_Nodes[0];
        m_EndNode = m_Nodes[m_Nodes.Count - 1];

        //Let the world know!
        if (LevelGeneratedEvent != null)
            LevelGeneratedEvent();

        m_CurrentRoutine = null;

        yield return null;
    }

    private void ClearLevel()
    {
        if (m_Nodes == null)
            return;

        for (int i = m_Nodes.Count - 1; i >= 0; --i)
            GameObject.Destroy(m_Nodes[i].gameObject);

        m_Nodes.Clear();
    }


    private void AssignNodeTextCharacters(string availableTextCharacters, int seed)
    {
        ClearNodeTextCharacters();

        Random.InitState(seed);

        //Give all the nodes a random character
        for (int i = 0; i < m_Nodes.Count; ++i)
        {
            Node currentNode = m_Nodes[i];

            //Create an array of all still available characters
            List<char> availableTextCharactersList = new List<char>(availableTextCharacters.ToCharArray());

            //Remove characters that our neighbours (and our neighbours neighbours!) have claimed
            for (int dir = 0; dir <= (int)Direction.West; ++dir)
            {
                Node neighbour = currentNode.GetNeighbour((Direction)dir);

                if (neighbour != null)
                {
                    //Character this neighbour has claimed
                    char takenChar = neighbour.GetTextCharacter();
                    if (takenChar != '\0') { availableTextCharactersList.Remove(takenChar); }

                    //Characters his neighbours have claimed
                    for (int dir2 = 0; dir2 <= (int)Direction.West; ++dir2)
                    {
                        Node neighbour2 = neighbour.GetNeighbour((Direction)dir2);

                        if (neighbour2 != null)
                        {
                            takenChar = neighbour2.GetTextCharacter();
                            if (takenChar != '\0') { availableTextCharactersList.Remove(takenChar); }
                        }
                    }
                }
            }

            if (availableTextCharactersList.Count > 0)
            {
                //Pick a random character from the remaining list and assign it
                int randInt = Random.Range(0, availableTextCharactersList.Count);
                currentNode.SetTextCharacter(availableTextCharactersList[randInt]);

                currentNode.name = currentNode.name + " - " + currentNode.GetTextCharacter();
            }
        }
    }

    private void ClearNodeTextCharacters()
    {
        for (int i = 0; i < m_Nodes.Count; ++i)
        {
            m_Nodes[i].SetTextCharacter('\0');
        }
    }
}
