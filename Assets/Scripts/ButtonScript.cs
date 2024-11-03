using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    private int maxPossiblesLevels = 2;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void Play()
    {
        int levelNumberToLoad = UnityEngine.Random.Range(0, maxPossiblesLevels);
        Debug.Log("Number of the Loaded Level:" + levelNumberToLoad);
        SceneManager.LoadScene("Scene" + Convert.ToString(levelNumberToLoad));
    }

    public void Exit()
    {
        Application.Quit();
    }
}