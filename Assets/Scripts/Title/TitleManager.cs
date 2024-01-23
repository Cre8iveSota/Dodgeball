using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0)
        {
            SoundManager.instance.PlayBGM(4);
        }
    }
    public void LoadScene(string name)
    {
        SoundManager.instance.PlaySE(1);
        SceneManager.LoadScene(name);
    }
}
