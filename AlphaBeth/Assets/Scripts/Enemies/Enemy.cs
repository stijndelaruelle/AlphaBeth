using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    private EnemyBehaviour m_EnemyBehaviour;

    protected override void Start()
    {
        if (m_EnemyBehaviour != null)
            m_EnemyBehaviour.Initialize(this);

        base.Start();
    }

    private void Update()
    {
        if (m_EnemyBehaviour != null)
            m_EnemyBehaviour.FrameUpdate();
    }

    public override void SetNode(Node node)
    {
        base.SetNode(node);

        if (m_EnemyBehaviour != null)
            m_EnemyBehaviour.OnEnterNode(node);
    }

    protected override void OnLevelStart()
    {
        base.OnLevelStart();

        if (m_EnemyBehaviour != null)
            m_EnemyBehaviour.OnLevelStart();
    }
}
