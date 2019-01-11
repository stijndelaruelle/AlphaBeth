using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterVisuals : MonoBehaviour
{
    [SerializeField]
    private Character m_Character;

    private void Start()
    {
        if (m_Character != null)
        {
            OnCharacterMove(m_Character.CurrentNode);
            m_Character.MoveEvent += OnCharacterMove;
        }  
    }

    private void OnDestroy()
    {
        if (m_Character != null)
            m_Character.MoveEvent -= OnCharacterMove;
    }

    private void OnCharacterMove(Node newNode)
    {
        if (newNode != null)
            m_Character.transform.DOMove(newNode.transform.position, 0.1f).SetEase(Ease.InOutElastic, 0.5f, 0.0f);
    }
}
