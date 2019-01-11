using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNoGridChaseBehaviour : EnemyBehaviour
{
    private Character m_Character;
    private Vector3 m_OriginalPosition;

    [SerializeField]
    private float m_Speed;
    private float m_CurrentSpeed;

    [SerializeField]
    private Player m_Player;

    public override void Initialize(Character character)
    {
        m_Character = character;
        m_OriginalPosition = m_Character.transform.position;

        if (m_Player != null)
            m_Player.InputMistakeEvent += OnPlayerInputMistake;
    }

    private void OnDestroy()
    {
        if (m_Player != null)
            m_Player.InputMistakeEvent -= OnPlayerInputMistake;
    }

    public override void FrameUpdate()
    {
        if (m_Player.IsDead)
            return;

        //Determine the direction
        Vector3 dir = m_Player.transform.position - m_Character.transform.position;

        if (dir.magnitude < 0.5f)
        {
            m_Player.Die();
            return;
        }

        dir.Normalize();

        m_Character.transform.position += dir * (m_CurrentSpeed * Time.deltaTime);
    }

    //Callbacks
    public override void OnEnterNode(Node node)
    {
        //We don't use nodes
    }

    public override void OnLevelStart()
    {
        m_Character.transform.position = m_OriginalPosition;
        m_CurrentSpeed = m_Speed;
    }

    private void OnPlayerInputMistake()
    {
        m_CurrentSpeed *= 2;
    }
}
