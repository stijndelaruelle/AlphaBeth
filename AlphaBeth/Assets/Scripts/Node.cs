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
    public delegate void CharacterDelegate(Character player);
    public delegate void PlayerDelegate(Player player);

    [SerializeField]
    private TextMeshProUGUI m_Text;

    [SerializeField]
    private SpriteRenderer m_Sprite;

    [SerializeField] //Mostly so it get's remembered after making a prefab
    private char m_TextCharacter = '\0';

    [SerializeField] //Mostly so it get's remembered after making a prefab
    private Node[] m_Neighbours = new Node[4]; //Currently 4, could become 8
    private List<Character> m_Characters;
    public List<Character> Characters
    {
        get { return m_Characters; }
    }

    //Temp, just to visualise where the exit is, this should become a level object at some point (just like HackShield)
    [SerializeField] //Mostly so it get's remembered after making a prefab
    private bool m_IsExit = false;
    public bool IsExit
    {
        get { return m_IsExit; }
    }

    //Only used for the fog of war
    private bool m_IsExplored = false;
    private bool m_IsAccessible = true;
    public bool IsAccessible
    {
        get { return m_IsAccessible; }
        set { m_IsAccessible = value; } //Shouldn't be able to do that, super cheat for indie level
    }

    //Only for the super indie level
    private bool m_IsVisible = true;
    public bool IsVisible
    {
        get { return m_IsVisible; }
        set { m_IsVisible = value; UpdateVisualText(); }
    }

    public event CharacterDelegate CharacterEnterEvent;
    public event CharacterDelegate CharacterLeaveEvent;

    public event PlayerDelegate PlayerEnterEvent;
    public event PlayerDelegate PlayerLeaveEvent;

    private void Awake()
    {
        m_Characters = new List<Character>();
    }

    private void Start()
    {
        SaveGameManager.BoolVariableChangedEvent += OnSaveGameBoolVariableChanged;  
    }

    private void OnDestroy()
    {
        SaveGameManager.BoolVariableChangedEvent -= OnSaveGameBoolVariableChanged;
    }

    public bool CanAccess()
    {
        if (m_IsAccessible == false)
            return false;

        return true;
    }

    public bool CanAccess(char typedChar)
    {
        if (CanAccess() == false)
            return false;

        return (typedChar == m_TextCharacter);
    }

    private void UpdateVisualText()
    {
        //Function that manages all the visual stuff, so we don't spread this all over the place
        if (m_Text == null)
            return;

        m_Text.enabled = (m_Characters.Count == 0) && m_IsAccessible && m_IsVisible;

        //Check if fog of war is enabled
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_OPTION_FOGOFWAR, true))
        {
            //Only show the character once we've explored it
            if (m_IsExplored)
            {
                m_Text.text = m_TextCharacter.ToString();

                if (m_IsExit) { m_Text.color = Color.yellow; }
                else          { m_Text.color = Color.white; }
            }
            else
            {
                m_Text.text = "?";
                m_Text.color = Color.black;
            }
        }
        else
        {
            //Just show the character
            m_Text.text = m_TextCharacter.ToString();

            if (m_IsExit) { m_Text.color = Color.yellow; }
            else          { m_Text.color = Color.white; }
        }
    }

    private void UpdateSprite()
    {
        m_Sprite.enabled = (m_Characters.Count == 0);
        m_Sprite.sortingOrder = -100 - ((int)(transform.position.y) * 2) + (int)(transform.position.x);
    }

    public void SetTextCharacter(char textChar)
    {
        m_TextCharacter = textChar;
        UpdateVisualText();
        UpdateSprite();
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


    public void AddCharacter(Character character)
    {
        m_Characters.Add(character);
        UpdateVisualText();

        //Let the world know
        if (CharacterEnterEvent != null)
            CharacterEnterEvent(character);
    }

    public void RemoveCharacter(Character character)
    {
        m_Characters.Remove(character);
        UpdateVisualText();

        //Let the world know
        if (CharacterLeaveEvent != null)
            CharacterLeaveEvent(character);
    }

    public void AddPlayer(Player player)
    {
        //We do this even if fog of war is currently disabled (so we can enable it at any time)
        m_IsExplored = true;

        //Also explore all our neighbours
        for (int i = 0; i <= (int)Direction.West; ++i)
        {
            Node neighbour = m_Neighbours[i];

            if (neighbour != null)
                neighbour.Explore();
        }

        //Nodes dissapear mode
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_OPTION_NODESDISAPPEAR, false))
        {
            m_IsAccessible = false;
        }

        //Let the world know
        if (PlayerEnterEvent != null)
            PlayerEnterEvent(player);
    }

    public void RemovePlayer(Player player)
    {
        //Let the world know
        if (PlayerLeaveEvent != null)
            PlayerLeaveEvent(player);
    }

   
    public void SetExit(bool state)
    {
        m_IsExit = state;
        UpdateVisualText();
    }

    public void Explore()
    {
        m_IsExplored = true;
        UpdateVisualText();
    }

    public void ResetNode()
    {
        //Level reset, so let's hide ourself
        m_IsExplored = false;
        m_IsAccessible = true;
        m_IsVisible = true;

        m_Characters.Clear();

        UpdateVisualText();
    }

    //Callbacks
    private void OnSaveGameBoolVariableChanged(string key, bool value)
    {
        if (key == SaveGameManager.SAVE_OPTION_FOGOFWAR)
            UpdateVisualText();
    }

    private void OnDrawGizmosSelected()
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
