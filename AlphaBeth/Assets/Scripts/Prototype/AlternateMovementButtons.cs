using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Should normally be 3 scripts, but hey this is super prototypy and I don't want to create too many file clutter

public class AlternateMovementButtons : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private Player m_Player;

    public void ChangeWalkCharacterSet()
    {
        int charSet = m_Player.WalkCharacterSet;
        charSet = (charSet + 1) % 3;

        m_Player.WalkCharacterSet = charSet;

        m_Text.text = "Character set " + (charSet + 1);
    }

    public void ToggleRandomize()
    {
        bool isRandom = m_Player.RandomizeCharacter;
        isRandom = !isRandom;

        m_Player.RandomizeCharacter = isRandom;

        m_Text.text = "Random: " + isRandom;
    }
}
