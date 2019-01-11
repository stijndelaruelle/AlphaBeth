using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//abstract class & not interface because of the inspector
public abstract class EnemyBehaviour : MonoBehaviour
{
    public abstract void Initialize(Character character);

    public abstract void FrameUpdate();
    public abstract void OnEnterNode(Node node);
    public abstract void OnLevelStart();
}
