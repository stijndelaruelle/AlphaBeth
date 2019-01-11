using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    //Delegates
    public delegate void CharacterMoveDelegate(Node newNode);
    public delegate void PlayerInputMistakeDelegate();
    public delegate void PlayerReachedExitDelegate();
        
    //Variable
    private Node m_CurrentNode;
    private Direction m_LastDirection;

    //Events
    public event CharacterMoveDelegate MoveEvent;
    public event PlayerInputMistakeDelegate InputMistakeEvent;
    public event PlayerReachedExitDelegate ReachedExitEvent;

    //Functions
    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Die();
        }
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
                for (int i = 0; i < 4; ++i)
                {
                    //Always check in the last chosen direction first, in case 2 neighbours have the same letter
                    Direction currentDirection = (Direction)(((int)m_LastDirection + i) % 4);

                    Node neighbour = m_CurrentNode.GetNeighbour(currentDirection);
                    if (neighbour != null)
                    {
                        //Yep this is the node we want to move to!
                        if (neighbour.CanAccess(pressendChar))
                        {
                            SetNode(neighbour);
                            m_LastDirection = currentDirection;
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
            m_CurrentNode.RemovePlayer(this);

        m_CurrentNode = node;
        m_CurrentNode.AddPlayer(this);

        if (MoveEvent != null)
            MoveEvent(m_CurrentNode);

        //Temp, should become a separate exit tile (just like HackShield)
        if (m_CurrentNode.IsExit)
        {
            if (ReachedExitEvent != null)
                ReachedExitEvent();
        }
    }
}
