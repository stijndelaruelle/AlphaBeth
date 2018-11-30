using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDirector : Singleton<LevelDirector>
{
    public delegate void LevelReadyDelegate();
    public delegate void LevelStartDelegate();
    public delegate void LevelEndDelegate();

    [SerializeField]
    private LevelGenerator m_LevelGenerator;

    [SerializeField]
    private Player m_Player;

    //Simple system that allows windows/cutscenes/etc... to block player input.
    private int m_PlayerInputBlockers = 0;
    private bool m_IsLevelReady = false;

    public event LevelReadyDelegate LevelReadyEvent;
    public event LevelStartDelegate LevelStartEvent;
    public event LevelEndDelegate LevelEndEvent;

    private void Start()
    {
        m_PlayerInputBlockers = 0;

        if (m_LevelGenerator != null)
        {
            m_LevelGenerator.LevelGeneratedEvent += OnLevelGenerated;

            //Generate a default level (so we don't start he game with the level settings menu open
            m_LevelGenerator.GenerateGridLevel();
        }

        if (m_Player != null)
        {
            m_Player.ReachedExitEvent += OnPlayerReachedExit;
            m_Player.InputMistakeEvent += OnPlayerInputMistake;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_LevelGenerator != null)
            m_LevelGenerator.LevelGeneratedEvent -= OnLevelGenerated;

        if (m_Player != null)
        {
            m_Player.ReachedExitEvent -= OnPlayerReachedExit;
            m_Player.InputMistakeEvent -= OnPlayerInputMistake;
        }
    }

    public void StartLevel()
    {
        if (m_IsLevelReady == false)
            return;

        if (LevelStartEvent != null)
            LevelStartEvent();

        //Reset player position (after event because of fog of war sequencing). This may bite us later on
        m_Player.SetNode(m_LevelGenerator.StartNode);
    }

    //Input Blocker
    public void AddInputBlocker()
    {
        m_PlayerInputBlockers += 1;
    }

    public void RemoveInputBlocker()
    {
        m_PlayerInputBlockers -= 1;

        if (m_PlayerInputBlockers < 0)
            m_PlayerInputBlockers = 0;
    }

    public bool HasInputBlockers()
    {
        return (m_PlayerInputBlockers > 0);
    }

    //Callbacks
    private void OnLevelGenerated()
    {
        m_IsLevelReady = true;

        if (LevelReadyEvent != null)
            LevelReadyEvent();
    }

    private void OnPlayerReachedExit()
    {
        if (LevelEndEvent != null)
            LevelEndEvent();
    }

    private void OnPlayerInputMistake()
    {
        //Check if te option is enabled
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_OPTION_NEWCHARSONMISTAKE, true) == false)
            return;

        m_LevelGenerator.AssignNodeTextCharacters();
    }
}
