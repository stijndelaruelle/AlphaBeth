using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    private void Update()
    {
        for (int i = 0; i < 4; ++i)
        {
            if (Input.GetKeyDown((KeyCode)(KeyCode.F1 + i)))
            {
                SceneManager.LoadScene(i);
            }
        }
    }
}
