using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelGenerator : MonoBehaviour
{
    public delegate void LevelDelegate();

    [SerializeField]
    private float m_TileSize;

    [Header("Required References")]
    [SerializeField]
    private Node m_NodePrefab;
    private List<Node> m_Nodes;
    public List<Node> Nodes
    {
        get { return m_Nodes; }
    }

    [SerializeField]
    private PolygonCollider2D m_CameraCollider;

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
    
    public void GenerateLevelFromChildren()
    {
        if (m_CurrentRoutine != null)
            StopCoroutine(m_CurrentRoutine);

        m_CurrentRoutine = StartCoroutine(GenerateLevelFromChildrenRoutine());
    }

    private IEnumerator GenerateLevelFromChildrenRoutine()
    {
        //Temp, wait one frame so everyone has the time to do subscribe etc before a level actually get's generated
        yield return new WaitForEndOfFrame();

        //Just get all our childnodes, the level was already cooked
        m_Nodes = new List<Node>(GetComponentsInChildren<Node>());

        //Let the world know!
        if (LevelGeneratedEvent != null)
            LevelGeneratedEvent();

        m_CurrentRoutine = null;

        yield return null;
    }

    public void GenerateLevelFromGrid()
    {
        if (m_NodePrefab == null)
        {
            Debug.LogWarning("Please assign a node prefab.");
            return;
        }

        if (m_CurrentRoutine != null)
            StopCoroutine(m_CurrentRoutine);

        m_CurrentRoutine = StartCoroutine(GenerateLevelFromGridRoutine());
    }

    private IEnumerator GenerateLevelFromGridRoutine()
    {
        //Temp, wait one frame so everyone has the time to do subscribe etc before a level actually get's generated
        yield return new WaitForEndOfFrame();

        //Get data
        int width = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_WIDTH, 5);
        int height = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_HEIGHT, 5);
        int seed = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_SEED, -1);

        if (seed == -1)
            UnityEngine.Random.InitState(System.Environment.TickCount); //Random enough?
        else
            UnityEngine.Random.InitState(seed);

        //Clear Level if needed
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
            newNode.transform.localPosition = new Vector3(x * m_TileSize, -y * m_TileSize, 0.0f); //0, 0 in the top left

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
        AssignNodeTextCharacters(seed);

        //Assign start node
        List<int> quadrants = new List<int> { 0, 1, 2, 3 };

        int randStartQuadrant = UnityEngine.Random.Range(0, quadrants.Count);
        m_StartNode = GetRandomNodeInQuadrant(width, height, quadrants[randStartQuadrant]);

        //Assign end node
        quadrants.Remove(randStartQuadrant); //Make sure the end quadrant is not the same as the start one

        int randEndQuadrant = UnityEngine.Random.Range(0, quadrants.Count);
        m_EndNode = GetRandomNodeInQuadrant(width, height, quadrants[randEndQuadrant]);
        m_EndNode.SetExit(true); //Visualize

        //Calculcate the collider bounds (so the camera doesn't go out of bounds)
        if (m_CameraCollider != null)
        {
            Vector3 topLeft = m_Nodes[0].transform.localPosition;
            Vector3 topRight = m_Nodes[width - 1].transform.localPosition;
            Vector3 bottomLeft = m_Nodes[(height * width) - width].transform.localPosition;
            Vector3 bottomRight = m_Nodes[(height * width) - 1].transform.localPosition;

            Vector2[] colliderPath = new Vector2[4];
            colliderPath[0] = new Vector2(topLeft.x - (m_TileSize * 0.5f), topLeft.y + (m_TileSize * 0.5f));
            colliderPath[1] = new Vector2(topRight.x + (m_TileSize * 0.5f), topRight.y + (m_TileSize * 0.5f));
            colliderPath[2] = new Vector2(bottomRight.x + (m_TileSize * 0.5f), bottomRight.y - (m_TileSize * 0.5f));
            colliderPath[3] = new Vector2(bottomLeft.x - (m_TileSize * 0.5f), bottomLeft.y - (m_TileSize * 0.5f));

            m_CameraCollider.SetPath(0, colliderPath);

            //Set ourselves to the center
            Vector3 total = topLeft + topRight + bottomLeft + bottomRight;
            transform.position = new Vector3(-total.x / 4.0f, -total.y / 4.0f, 0.0f);
        }

        //Let the world know!
        if (LevelGeneratedEvent != null)
            LevelGeneratedEvent();

        m_CurrentRoutine = null;

        yield return null;
    }


    public void GenerateLevelFromFile(string filePath)
    {
        if (m_NodePrefab == null)
        {
            Debug.LogWarning("Please assign a node prefab.");
            return;
        }

        if (m_CurrentRoutine != null)
            StopCoroutine(m_CurrentRoutine);

        m_CurrentRoutine = StartCoroutine(GenerateLevelFromFileRoutine(filePath));
    }

    public IEnumerator GenerateLevelFromFileRoutine(string filePath)
    {
        //Temp, wait one frame so everyone has the time to do subscribe etc before a level actually get's generated
        yield return new WaitForEndOfFrame();

        //Load the level from the file
        string levelData = "";
        try
        {
            levelData = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
        }
        catch (Exception e)
        {
            //The file was not found, but that shouldn't crash the game!
            Debug.LogError(e.Message);
        }

        if (levelData == "")
            yield return null;

        int width = levelData.IndexOf('\r');
        
        levelData = levelData.Replace("\r\n", ""); //Remove the enters
        levelData = levelData.Replace(" ", ""); //Remove all the extra spaces

        int height = levelData.Length / width;

        //Clear Level if needed
        if (m_Nodes == null) { m_Nodes = new List<Node>(); }
        else { ClearLevel(); }

        //Create all the nodes
        for (int i = 0; i < levelData.Length; ++i)
        {
            int x = (i % width);
            int y = (i / width);

            Node newNode = GameObject.Instantiate<Node>(m_NodePrefab);

            newNode.name = "Node (" + x + ", " + y + ")";
            newNode.transform.parent = transform;
            newNode.transform.localPosition = new Vector3(x * m_TileSize, -y * m_TileSize, 0.0f); //0, 0 in the top left

            //Set letter
            newNode.SetTextCharacter(levelData[i]);

            if (levelData[i] != '.')
                newNode.name = newNode.name + " - " + newNode.GetTextCharacter();

            m_Nodes.Add(newNode);
        }

        //Link all the nodes (empty tiles shouldn't be here, but for now this is easier!)
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

        //Remove all the empty nodes (super inefficient, but I'm prototyping here!)
        for (int i = m_Nodes.Count - 1; i > 0; --i)
        {
            Node currentNode = m_Nodes[i];

            if (currentNode.GetTextCharacter() == '.')
            {
                //Remove all the neighbours
                Node neighbour = currentNode.GetNeighbour(Direction.North);
                if (neighbour != null) { neighbour.SetNeighbour(Direction.South, null); }

                neighbour = currentNode.GetNeighbour(Direction.South);
                if (neighbour != null) { neighbour.SetNeighbour(Direction.North, null); }

                neighbour = currentNode.GetNeighbour(Direction.East);
                if (neighbour != null) { neighbour.SetNeighbour(Direction.West, null); }

                neighbour = currentNode.GetNeighbour(Direction.West);
                if (neighbour != null) { neighbour.SetNeighbour(Direction.East, null); }

                GameObject.Destroy(currentNode.gameObject);
                m_Nodes.RemoveAt(i);
            }
        }

        //Assign start node
        //m_StartNode = null; // m_Nodes[m_Nodes.Count - 7];

        //Assign end node
        //m_EndNode

        //Calculcate the collider bounds (so the camera doesn't go out of bounds)

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

    public void ResetLevel()
    {
        if (m_Nodes == null)
            return;

        for (int i = 0; i < m_Nodes.Count; ++i)
        {
            m_Nodes[i].ResetNode();
        }
    }

    public void AssignNodeTextCharacters(int seed = -1)
    {
        string availableTextCharacters = SaveGameManager.GetString(SaveGameManager.SAVE_LEVEL_TEXTCHARACTERS, "sdfghjkl");

        //ClearNodeTextCharacters();

        if (seed == -1)
            UnityEngine.Random.InitState(System.Environment.TickCount); //Random enough?
        else
            UnityEngine.Random.InitState(seed);

        //Give all the nodes a random character
        foreach (Node currentNode in m_Nodes)
        {
            //Create an array of all still available characters
            List<char> availableTextCharactersList = new List<char>(availableTextCharacters.ToCharArray());

            //Remove characters that our neighbours neighbours have claimed
            for (int dir = 0; dir <= (int)Direction.West; ++dir)
            {
                Node neighbour = currentNode.GetNeighbour((Direction)dir);

                if (neighbour != null)
                {
                    //Character this neighbour has claimed (this is allowed, you can have an E next to a another E (the one you're standing on))
                    //char takenChar = neighbour.GetTextCharacter();
                    //if (takenChar != '\0') { availableTextCharactersList.Remove(takenChar); }

                    //Characters his neighbours have claimed
                    for (int dir2 = 0; dir2 <= (int)Direction.West; ++dir2)
                    {
                        Node neighbour2 = neighbour.GetNeighbour((Direction)dir2);

                        if (neighbour2 != null)
                        {
                            char takenChar = neighbour2.GetTextCharacter();
                            if (takenChar != '\0') { availableTextCharactersList.Remove(takenChar); }
                        }
                    }
                }
            }

            if (availableTextCharactersList.Count > 0)
            {
                //Pick a random character from the remaining list and assign it
                int randInt = UnityEngine.Random.Range(0, availableTextCharactersList.Count);
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

    //Utility
    private Node GetRandomNodeInQuadrant(int width, int height, int quadrant)
    {
        if (m_Nodes == null)
            return null;

        //X
        bool isWidthOdd = ((width % 2) != 0);
        bool isWidthQuadrantOdd = ((quadrant % 2) != 0);

        int quadrandWidth = Mathf.FloorToInt(width * 0.5f);

        int minStartX = (quadrant % 2) * quadrandWidth;
        if (isWidthOdd && isWidthQuadrantOdd) { minStartX += 1; } //With odd sizes, the middle row is not used

        int maxStartX = minStartX + quadrandWidth; //-1, but because Random.Range is exclusive, we don't bother

        int randStartX = UnityEngine.Random.Range(minStartX, maxStartX);

        //Y
        bool isHeightOdd = ((height % 2) != 0);
        bool isHeightQuadrantOdd = (Mathf.FloorToInt(quadrant / 2) != 0);

        int quadrantHeight = Mathf.FloorToInt(height * 0.5f);

        int minStartY = (quadrant / 2) * quadrantHeight;
        if (isHeightOdd && isHeightQuadrantOdd) { minStartY += 1; }

        int maxStartY = minStartY + quadrantHeight; //-1, but because Random.Range is exclusive, we don't bother

        int randStartY = UnityEngine.Random.Range(minStartY, maxStartY);

        //Combine into a node ID
        int nodeID = randStartX + (randStartY * width);

        //Fetch the node
        if (nodeID < 0 || nodeID >= m_Nodes.Count)
            return null;

        return m_Nodes[nodeID];
    }
}
