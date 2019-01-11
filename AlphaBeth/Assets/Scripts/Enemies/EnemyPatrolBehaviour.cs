using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolBehaviour : EnemyBehaviour
{
    private Character m_Character;

    [SerializeField]
    private float m_UpdateTime;
    private float m_UpdateTimer;

    [SerializeField]
    private List<Node> m_Path;
    private int m_CurrentNodeID = 0;

    public override void Initialize(Character character)
    {
        m_Character = character;
        m_UpdateTimer = m_UpdateTime;
    }

    public override void FrameUpdate()
    {
        if (m_UpdateTimer < 0.0f)
        {
            //Move the character
            m_CurrentNodeID += 1;

            if (m_CurrentNodeID >= m_Path.Count)
                m_CurrentNodeID = 0;

            if (m_Path.Count > 0)
                m_Character.SetNode(m_Path[m_CurrentNodeID]);

            //Prepare for the next update
            m_UpdateTimer += m_UpdateTime; //If we were below 0, diminish that time from the next update.
        }

        m_UpdateTimer -= Time.deltaTime;
    }

    //Callbacks
    public override void OnEnterNode(Node node)
    {
        if (node == null)
            return;

        //Kill everyone! (Death doesn't really work yet but hey! It's a start)
        for (int i = node.Characters.Count - 1; i >= 0; --i)
        {
            Character character = node.Characters[i];

            if (character != m_Character)
                character.Die();
        }
    }

    public override void OnLevelStart()
    {
        m_CurrentNodeID = 0;
        m_UpdateTimer = m_UpdateTime;
    }
}
