using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Delegates
    public delegate void InputMistakeDelegate();
    public delegate void ReachedExitDelegate();

    //Variable
    private Node m_CurrentNode;

    //Events
    public event InputMistakeDelegate InputMistakeEvent;
    public event ReachedExitDelegate ReachedExitEvent;

    //Functions
    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (m_CurrentNode == null)
            return;

        if (LevelDirector.Instance == null)
            return;

        if (LevelDirector.Instance.HasInputBlockers())
            return;

        //Check what key was pressed this frame, and if there is neighbour with that key.
        //https://docs.unity3d.com/ScriptReference/Input-inputString.html
        foreach (char pressendChar in Input.inputString)
        {
            //Backspace
            if (pressendChar == '\b') { }

            //Enter & Return
            else if ((pressendChar == '\n') || (pressendChar == '\r')) { }

            //Everything else
            else
            {
                for (int i = 0; i <= (int)Direction.West; ++i)
                {
                    Node neighbour = m_CurrentNode.GetNeighbour((Direction)i);
                    if (neighbour != null)
                    {
                        //Yep this is the node we want to move to!
                        if (pressendChar == neighbour.GetTextCharacter())
                        {
                            SetNode(neighbour);
                            return;
                        }
                    }
                }
            }

            //If not, we made a mistake (pressed a wrong button) and are "punished?"
            if (InputMistakeEvent != null)
                InputMistakeEvent();
        }
    }

    public void SetNode(Node node)
    {
        if (m_CurrentNode != null)
            m_CurrentNode.RemoveCharacter(this);

        m_CurrentNode = node;
        transform.position = m_CurrentNode.transform.position;

        m_CurrentNode.AddCharacter(this);

        //Temp, should become a separate exit tile (just like HackShield)
        if (m_CurrentNode.IsExit)
        {
            if (ReachedExitEvent != null)
                ReachedExitEvent();
        }
    }
}
