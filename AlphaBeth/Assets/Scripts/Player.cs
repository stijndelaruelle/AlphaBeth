using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    //Delegates
    public delegate void PlayerInputMistakeDelegate();
    public delegate void PlayerReachedExitDelegate();
    public delegate void PlayerStartAttackDelegate();
    public delegate void PlayerStopAttackDelegate();

    //Variables
    private Direction m_LastDirection;

    //Alternate step movement
    private uint m_LastStepID = 0; //0 = Left, 1 = Right
    private char[,] m_WalkCharacters = new char[,]
                                        {
                                            {'f', 'j'}, //North
                                            {'k', 'l'}, //East
                                            {'v', 'n'}, //South
                                            {'s', 'd'}, //West
                                        };

    //Events
    public event PlayerInputMistakeDelegate InputMistakeEvent;
    public event PlayerReachedExitDelegate ReachedExitEvent;

    public event PlayerStartAttackDelegate StartAttackEvent;
    public event PlayerStopAttackDelegate StopAttackEvent;

    //Functions
    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (StartAttackEvent != null)
                StartAttackEvent();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (StopAttackEvent != null)
                StopAttackEvent();
        }

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
                            m_LastStepID = (m_LastStepID + 1) % 2; //Alternate between 0 and 1 (left & right)

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

    public override void SetNode(Node node)
    {
        //Cleanup
        if (m_CurrentNode != null)
            m_CurrentNode.RemovePlayer(this);

        //TEST movement
        ResetNodeFromWalkMovement(m_CurrentNode);

        //Change the current node
        base.SetNode(node);

        if (m_CurrentNode == null)
            return;

        m_CurrentNode.AddPlayer(this);

        //TEST movement (alter the neighbours 
        PrepareNodeForWalkMovement(m_CurrentNode);

        //Temp, should become a separate exit tile (just like HackShield)
        if (m_CurrentNode.IsExit)
        {
            if (ReachedExitEvent != null)
                ReachedExitEvent();
        }
    }

    private void ResetNodeFromWalkMovement(Node node)
    {
        if (node == null || node.GetOriginalTextCharacter() != ' ')
            return;

        node.SetTextCharacter(' ');

        //Reset all the neighbours, if needed
        for (int i = 0; i < 4; ++i)
        {
            Direction currentDirection = (Direction)i;

            Node neighbour = m_CurrentNode.GetNeighbour(currentDirection);
            if (neighbour != null)
            {
                if (neighbour.GetOriginalTextCharacter() == ' ')
                    neighbour.SetTextCharacter(' ');
            }
        }
    }

    private void PrepareNodeForWalkMovement(Node node)
    {
        if (node == null || node.GetOriginalTextCharacter() != ' ')
            return;

        for (int i = 0; i < 4; ++i)
        {
            Direction currentDirection = (Direction)i;

            Node neighbour = m_CurrentNode.GetNeighbour(currentDirection);
            if (neighbour != null)
            {
                if (neighbour.GetOriginalTextCharacter() == ' ')
                {
                    neighbour.SetTextCharacter(m_WalkCharacters[i, m_LastStepID]);
                }
            }
        }
    }
}
