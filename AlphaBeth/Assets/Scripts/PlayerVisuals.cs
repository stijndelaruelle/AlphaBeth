using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    private Vector3 m_Offset;

    private void Start()
    {
        m_Offset = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);

        if (m_Player != null)
            m_Player.MoveEvent += OnPlayerMove;
    }

    private void OnDestroy()
    {
        if (m_Player != null)
            m_Player.MoveEvent -= OnPlayerMove;
    }

    private void OnPlayerMove(Node newNode)
    {
        transform.DOMove(newNode.transform.position + m_Offset, 0.1f).SetEase(Ease.InOutElastic, 0.5f, 0.0f);
    }
}
