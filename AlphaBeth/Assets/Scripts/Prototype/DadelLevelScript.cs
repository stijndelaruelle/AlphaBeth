using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadelLevelScript : MonoBehaviour
{
    [SerializeField]
    private LevelGenerator m_LevelGenerator;

    [SerializeField]
    private Node m_DadelFinish;

    [SerializeField]
    private Node m_LawaaiFinish;

    [SerializeField]
    private Node m_ZwaanFinish;

    [SerializeField]
    private Node m_FinalFinish;

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
        m_DadelFinish.PlayerEnterEvent += OnPlayerFinishedDadels;
        m_LawaaiFinish.PlayerEnterEvent += OnPlayerFinishedLaweit;
        m_ZwaanFinish.PlayerEnterEvent += OnPlayerFinishedZwanen;
    }

    private void OnLevelStart()
    {
        //Remove the exit
        m_FinalFinish.SetExit(false);

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

        //Add the pictures
        if (m_SpriteGameObjects == null)
            m_SpriteGameObjects = new List<NodeSprite>();

        foreach(NodeSprite nodeSprite in m_SpriteGameObjects)
        {
            GameObject.Destroy(nodeSprite.gameObject);
        }

        m_SpriteGameObjects.Clear();

        //Make everything invisible
        foreach (Node node in m_LevelGenerator.Nodes)
        {
            node.IsAccessible = true;
            node.IsVisible = false;

            if (prefab != null && node != currentNode && node.GetOriginalTextCharacter() != '+')
            {
                NodeSprite nodeSprite = GameObject.Instantiate<NodeSprite>(prefab);
                nodeSprite.Initialize(node);
                m_SpriteGameObjects.Add(nodeSprite);
            }
        }

        //Add an exit
        m_FinalFinish.SetExit(true);
        m_TriggeredEnding = true;
    }


    private void OnPlayerFinishedDadels(Character player)
    {
        Debug.Log("Dadels!");
        ConvertLevelForStage2(m_DadelFinish, m_DadelPrefab);
    }

    private void OnPlayerFinishedZwanen(Character player)
    {
        Debug.Log("Zwanen!");
        ConvertLevelForStage2(m_ZwaanFinish, m_ZwaanPrefab);
    }

    private void OnPlayerFinishedLaweit(Character player)
    {
        Debug.Log("Laweit!");
        ConvertLevelForStage2(m_LawaaiFinish, m_LaweitPrefab);
    }
}
