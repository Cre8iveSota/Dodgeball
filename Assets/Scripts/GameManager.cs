using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isChangeActiveCharacter;
    GameObject[] playerTeamMembers;
    GameObject enemyTeamMember;
    public GameObject mainChara, subChara;
    private GameObject ball;
    public int duration = 0;
    private int threshold = 30;

    public int Threshold { get => threshold; }


    // Start is called before the first frame update
    void Start()
    {
        playerTeamMembers = GameObject.FindGameObjectsWithTag("Player");
        enemyTeamMember = GameObject.FindGameObjectWithTag("Enemy");
        mainChara = playerTeamMembers.FirstOrDefault(i => i.GetComponent<MainPlayerController>());
        Debug.Log("so fa mainChara" + mainChara);
        subChara = playerTeamMembers.FirstOrDefault(i => i.GetComponent<SubPlayerController>());
        Debug.Log("so fa subChara" + subChara);
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    // Update is called once per frame
    public bool CheckHaveBallAsChildren(GameObject targetObject)
    {
        Transform[] allChildren = targetObject.GetComponentsInChildren<Transform>();
        bool ballExists = false;
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Ball"))
            {
                ballExists = true;
                break;
            }
        }
        return ballExists;
    }
    public GameObject GetBallHolderPlayer(bool isBallholder)
    {
        if (isBallholder)
        {
            if (CheckHaveBallAsChildren(mainChara))
            {
                return mainChara;
            }
            if (CheckHaveBallAsChildren(subChara))
            {
                return subChara;
            }
            return enemyTeamMember;
        }
        else
        {
            if (!CheckHaveBallAsChildren(subChara))
            {
                return subChara;
            }
            else if (!CheckHaveBallAsChildren(mainChara))
            {
                return mainChara;
            }
            else
            {
                // return mainChara as default
                Debug.LogWarning("You may get inappropriate gameObject");
                return mainChara;
            }
        }
    }
    public void CountingTimeOfHoldingShiftKey()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log("長押し");
            duration = duration + 1;
            Debug.Log("_duration" + duration);
        }
        else
        {
            duration = 0;
        }
    }
}
