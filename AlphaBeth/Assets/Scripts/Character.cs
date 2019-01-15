using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public delegate void CharacterMoveDelegate(Node newNode);
    public delegate void CharacterDeathDelegate();

    //Variables
    [SerializeField]
    private Node m_StartNode;

    protected Node m_CurrentNode;
    public Node CurrentNode
    {
        get { return m_CurrentNode; }
    }

    private bool m_IsDead = false;
    public bool IsDead
    {
        get { return m_IsDead; }
    }

    //Events
    public event CharacterMoveDelegate MoveEvent;
    public event CharacterDeathDelegate DeathEvent;

    protected virtual void Start()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent += OnLevelStart;
        }

        SetNode(m_StartNode);
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent -= OnLevelStart;
        }
    }

    public virtual void SetNode(Node node)
    {
        if (m_CurrentNode != null)
            m_CurrentNode.RemoveCharacter(this);

        m_CurrentNode = node;

        if (m_CurrentNode != null)
            m_CurrentNode.AddCharacter(this);

        if (MoveEvent != null)
            MoveEvent(m_CurrentNode);
    }

    //Should be splitted into a healbar script? (IDamageable)
    public void Die()
    {
        if (m_IsDead)
            return;

        m_IsDead = true;

        //We don't track the death state or anything right now.
        if (DeathEvent != null)
            DeathEvent();
    }

    protected virtual void OnLevelStart()
    {
        //Reset tile
        SetNode(m_StartNode);

        m_IsDead = false;
    }
}
