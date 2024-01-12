using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isChangeActiveCharacter;
    GameObject[] playerTeamMembers;
    public GameObject mainChara, subChara;
    // Start is called before the first frame update
    void Start()
    {
        playerTeamMembers = GameObject.FindGameObjectsWithTag("Player");
        mainChara = playerTeamMembers.FirstOrDefault(i => i.GetComponent<PlayerController>().enabled == true);
        Debug.Log("so fa mainChara" + mainChara);
        subChara = playerTeamMembers.FirstOrDefault(i => i != mainChara);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("so fa" + mainChara.transform.position);
        if (isChangeActiveCharacter && mainChara != null && subChara != null)
        {
            subChara.GetComponent<SubPlayerController>().enabled = false;
            mainChara.GetComponent<SubPlayerController>().enabled = true;
            subChara.GetComponent<PlayerController>().enabled = true;
            mainChara.GetComponent<PlayerController>().enabled = false;

            isChangeActiveCharacter = false;
            GameObject tmp = mainChara;
            mainChara = subChara;
            subChara = tmp;
        }
    }
}
