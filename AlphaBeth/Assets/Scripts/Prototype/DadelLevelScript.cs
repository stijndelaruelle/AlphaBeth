using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadelLevelScript : MonoBehaviour
{
    [SerializeField]
    private LevelGenerator m_LevelGenerator;
    private List<Node> m_Nodes;

    [SerializeField]
    private NodeSprite m_DadelPrefab;

    [SerializeField]
    private NodeSprite m_ZwaanPrefab;

    [SerializeField]
    private NodeSprite m_LaweitPrefab;
    List<NodeSprite> m_SpriteGameObjects;

    private bool m_TriggeredEnding = false;

    private void Start()
    {
        //Alter the settings to be super indie
        SaveGameManager.SetBool(SaveGameManager.SAVE_OPTION_NEWCHARSONMISTAKE, false);
        SaveGameManager.SetBool(SaveGameManager.SAVE_OPTION_FOGOFWAR, false);
        SaveGameManager.SetBool(SaveGameManager.SAVE_OPTION_NODESDISAPPEAR, true);
        SaveGameManager.SetBool(SaveGameManager.SAVE_OPTION_SCREENSHAKE, true);

        //Only works thanks to the execution order, CHANGE
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelReadyEvent += OnLevelReady;
            LevelDirector.Instance.LevelStartEvent += OnLevelStart;
        }
    }

    private void OnLevelReady()
    {
        //Super hacky, only works for this specific level!
        if (m_LevelGenerator == null)
            return;

        m_Nodes = m_LevelGenerator.Nodes;

        m_Nodes[6].PlayerEnterEvent += OnPlayerFinishedDadels;
        m_Nodes[78].PlayerEnterEvent += OnPlayerFinishedLaweit;
        m_Nodes[90].PlayerEnterEvent += OnPlayerFinishedZwanen;
    }

    private void OnLevelStart()
    {
        //Remove the exit
        if (m_Nodes != null)
            m_Nodes[266].SetExit(false);

        m_TriggeredEnding = false;

        //Clear all the pictures
        if (m_SpriteGameObjects == null)
            m_SpriteGameObjects = new List<NodeSprite>();

        foreach (NodeSprite nodeSprite in m_SpriteGameObjects)
            GameObject.Destroy(nodeSprite.gameObject);

        m_SpriteGameObjects.Clear();
    }

    private void ConvertLevelForStage2(Node currentNode, NodeSprite prefab)
    {
        if (m_TriggeredEnding)
            return;

        if (m_Nodes == null)
            return;

        //Add the pictures
        if (m_SpriteGameObjects == null)
            m_SpriteGameObjects = new List<NodeSprite>();

        foreach(NodeSprite nodeSprite in m_SpriteGameObjects)
        {
            GameObject.Destroy(nodeSprite.gameObject);
        }

        m_SpriteGameObjects.Clear();

        //Make everything invisible
        foreach (Node node in m_Nodes)
        {
            node.IsAccessible = true;
            node.IsVisible = false;

            if (node.GetTextCharacter() != '.' && prefab != null && node != currentNode)
            {
                NodeSprite nodeSprite = GameObject.Instantiate<NodeSprite>(prefab);
                nodeSprite.Initialize(node);
                m_SpriteGameObjects.Add(nodeSprite);
            }
        }

        //Add an exit
        m_Nodes[266].SetExit(true);
        m_TriggeredEnding = true;
    }


    private void OnPlayerFinishedDadels(Character player)
    {
        Debug.Log("Dadels!");
        ConvertLevelForStage2(m_Nodes[6], m_DadelPrefab);
    }

    private void OnPlayerFinishedZwanen(Character player)
    {
        Debug.Log("Zwanen!");
        ConvertLevelForStage2(m_Nodes[90], m_ZwaanPrefab);
    }

    private void OnPlayerFinishedLaweit(Character player)
    {
        Debug.Log("Laweit!");
        ConvertLevelForStage2(m_Nodes[78], m_LaweitPrefab);
    }
}
