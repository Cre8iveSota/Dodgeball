using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Instractions : MonoBehaviour
{
    public Sprite[] instractions;
    Image image;
    [SerializeField] private int num = 0;
    public bool isDoneInstractions;
    public GameObject instractionsPanel, backButton;
    [SerializeField] private TMP_Text nextText;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (num == 0)
        {
            backButton.SetActive(false);
        }
        else if (num == 5)
        {
            nextText.text = "OK";
        }
        else
        {
            backButton.SetActive(true);
            nextText.text = "Next";
        }

    }

    public void StartInstractions()
    {
        image = GetComponent<Image>();
        instractionsPanel.SetActive(true);
        num = 0;
        image.sprite = instractions[num];
        isDoneInstractions = false;
    }

    public void NextInstractions()
    {
        SoundManager.instance.PlaySE(1);
        if (num == instractions.Length - 1) { instractionsPanel.SetActive(false); isDoneInstractions = true; }
        else if (num < instractions.Length) { num++; image.sprite = instractions[num]; }
    }
    public void BackInstractions()
    {
        SoundManager.instance.PlaySE(1);
        if (0 < num) { num--; image.sprite = instractions[num]; }
    }
}
