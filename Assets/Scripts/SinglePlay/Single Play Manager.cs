using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class SinglePlayManager : MonoBehaviour
{
    public bool hasPlayer1TeamBall;
    public Quaternion normalRotation, normalRightRotation, normalLeftRotation, inverseRotation, inverseRightRotation, inverseLeftRotation;
    public GameObject mainChara, subChara, enemy, enemySub;
    public GameObject mainCharaInstance, subCharaInstance, enemyInstance, subEnemyInstance;
    public GameObject empty;
    public GameObject realBall, realBallInstance;
    public TMP_Text scoreText, enemyLvText;
    public int playerScore, enemyScore = 0;
    public int enemyLv = 1;
    public int duration = 0;
    private int threshold = 10;
    public int Threshold { get => threshold; }
    SPballController sPballController;


    void Start()
    {
        normalRotation = Quaternion.Euler(0f, 0f, 0f);
        normalRightRotation = Quaternion.Euler(0f, 45f, 0f);
        normalLeftRotation = Quaternion.Euler(0f, 315f, 0f);
        inverseRotation = Quaternion.Euler(0f, 180f, 0f);
        inverseRightRotation = Quaternion.Euler(0f, 225f, 0f);
        inverseLeftRotation = Quaternion.Euler(0f, 135f, 0f);

        mainCharaInstance = Instantiate(mainChara, new Vector3(0, 0, -6f), Quaternion.identity);
        subCharaInstance = Instantiate(subChara, new Vector3(0, 0, 16f), inverseRotation);
        enemyInstance = Instantiate(enemy, new Vector3(0, 0, 6f), inverseRotation);
        subEnemyInstance = Instantiate(enemySub, new Vector3(0, 0, -16f), Quaternion.identity);

        if (mainCharaInstance != null && realBall != null)
        {
            realBallInstance = Instantiate(realBall, mainCharaInstance.transform);
        }

        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null) sPballController = ball.GetComponent<SPballController>();
    }

    void Update()
    {
        scoreText.text = $"You {playerScore} - {enemyScore} Enemy";
        enemyLvText.text = $"Enemy\nLv.{enemyLv}";

        if (GetBallHolderTeamPlayer(true) == mainCharaInstance || GetBallHolderTeamPlayer(true) == subCharaInstance) hasPlayer1TeamBall = true;
        if (GetBallHolderTeamPlayer(true) == enemyInstance || GetBallHolderTeamPlayer(true) == subEnemyInstance) hasPlayer1TeamBall = false;
    }

    public void CountingTimeOfHoldingShiftKey()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            duration = duration + 1;
        }
        else
        {
            duration = 0;
        }
    }

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
    public GameObject GetBallHolderTeamPlayer(bool isBallholder)
    {
        if (isBallholder)
        {
            if (CheckHaveBallAsChildren(mainCharaInstance) || sPballController.ThrowMan == mainCharaInstance)
            {
                return mainCharaInstance;
            }
            else if (CheckHaveBallAsChildren(subCharaInstance) || sPballController.ThrowMan == subCharaInstance)
            {
                return subCharaInstance;
            }
            else if (CheckHaveBallAsChildren(enemyInstance) || sPballController.ThrowMan == enemyInstance)
            {
                return enemyInstance;
            }
            else if (CheckHaveBallAsChildren(subEnemyInstance) || sPballController.ThrowMan == subEnemyInstance)
            {
                return subEnemyInstance;
            }
            else
            {
                Debug.Log("Warrning: You maight get unitended result from GetBallHolderTeamPlayer");
                return empty;
            }
        }
        else
        {
            // todo tmp ball holderにボールがある時は考慮していない
            if (CheckHaveBallAsChildren(mainCharaInstance))
            {
                return subCharaInstance;
            }
            else if (CheckHaveBallAsChildren(subCharaInstance))
            {
                return mainCharaInstance;
            }
            else if (CheckHaveBallAsChildren(enemyInstance))
            {
                return subEnemyInstance;
            }
            else
            {
                return enemyInstance;
            }
        }
    }
}
