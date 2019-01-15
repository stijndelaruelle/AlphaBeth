using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private Sprite m_IdleSprite;

    [SerializeField]
    private Sprite m_AttackSprite;

    private void Start()
    {
        if (m_Player != null)
        {
            OnMove(m_Player.CurrentNode);

            m_Player.MoveEvent += OnMove;
            m_Player.StartAttackEvent += OnStartAttack;
            m_Player.StopAttackEvent += OnStopAttack;
        }

        if (m_SpriteRenderer != null)
            m_SpriteRenderer.sprite = m_IdleSprite;
    }

    private void OnDestroy()
    {
        if (m_Player != null)
            m_Player.MoveEvent -= OnMove;
    }

    private void OnMove(Node newNode)
    {
        if (newNode != null)
            m_Player.transform.DOMove(newNode.transform.position, 0.15f).SetEase(Ease.InOutElastic, 0.5f, 0.15f);
    }

    private void OnStartAttack()
    {
        if (m_SpriteRenderer != null)
            m_SpriteRenderer.sprite = m_AttackSprite;
    }

    private void OnStopAttack()
    {
        if (m_SpriteRenderer != null)
            m_SpriteRenderer.sprite = m_IdleSprite;
    }
}
