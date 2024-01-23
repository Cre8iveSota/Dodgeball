using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void LoadScene(string name)
    {
        SoundManager.instance.PlaySE(1);
        SceneManager.LoadScene(name);
    }
}
