using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSprite : MonoBehaviour
{
    private Node m_Node;

    public void Initialize(Node node)
    {
        m_Node = node;

        if (m_Node != null)
            m_Node.CharacterEnterEvent += OnCharacterEnter;

        transform.parent = node.gameObject.transform;
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-45, 45));
    }

    private void OnDestroy()
    {
        if (m_Node != null)
            m_Node.CharacterEnterEvent -= OnCharacterEnter;
    }

    private void OnCharacterEnter(Player player)
    {
        gameObject.SetActive(false);
    }
}
