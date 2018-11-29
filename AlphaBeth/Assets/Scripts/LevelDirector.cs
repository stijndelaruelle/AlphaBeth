using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    //Pretty lame, but current coördinates things
    [SerializeField]
    private LevelGenerator m_LevelGenerator;

    [SerializeField]
    private Player m_Player;

    private void Start()
    {
        if (m_LevelGenerator == null)
            return;

        m_LevelGenerator.LevelGeneratedEvent += OnLevelGenerated;

        //Generate a default level (so we don't start he game with the level settings menu open
        m_LevelGenerator.GenerateGridLevel(5, 5, "sdfghjkl", 42);
    }

    private void OnDestroy()
    {
        m_LevelGenerator.LevelGeneratedEvent -= OnLevelGenerated;
    }

    private void OnLevelGenerated()
    {
        m_Player.SetNode(m_LevelGenerator.StartNode);
    }
}
