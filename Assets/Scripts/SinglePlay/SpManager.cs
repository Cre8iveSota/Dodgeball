using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class SpManager : MonoBehaviour
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
    SpBallController spBallController;
    public float actionSpeedEnemy = 0.6f;
    public GameObject changeLevelPanel, lvImg, readyImg, goImg, gameOverPanel, gameClearPanel;
    public TMP_Text lvText;
    public int currentEnemyLv = 0;
    bool isActiveDisplyaImg = false;
    bool isActiveChangeLvPanel = false;


    void Start()
    {
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);
        currentEnemyLv = 0;
        actionSpeedEnemy = 0.6f;

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
        if (ball != null) spBallController = ball.GetComponent<SpBallController>();

        ChangeEnemyLv();
    }

    void Update()
    {
        scoreText.text = $"You {playerScore} - {enemyScore} Enemy";
        enemyLvText.text = $"Enemy\nLv.{currentEnemyLv}";

        if (GetBallHolderTeamPlayer(true) == mainCharaInstance || GetBallHolderTeamPlayer(true) == subCharaInstance) hasPlayer1TeamBall = true;
        if (GetBallHolderTeamPlayer(true) == enemyInstance || GetBallHolderTeamPlayer(true) == subEnemyInstance) hasPlayer1TeamBall = false;
        if (playerScore == 3)
        {
            ChangeEnemyLv();
            playerScore = 0;
            enemyScore = 0;
        }
        else if (enemyScore == 3)
        {
            gameOverPanel.SetActive(true);
        }
    }
    public void ChangeEnemyLv()
    {
        if (currentEnemyLv == 5)
        {
            currentEnemyLv = 4;
            gameClearPanel.SetActive(true);
        }
        currentEnemyLv++;
        actionSpeedEnemy -= 0.1f;
        lvText.text = $"Lv.{currentEnemyLv}";
        spBallController.reciever = mainCharaInstance;
        mainCharaInstance.GetComponent<SpMainPlayerController>().enabled = false;
        enemyInstance.GetComponent<SpEnemyController>().enabled = false;
        changeLevelPanel.SetActive(true);
        StartCoroutine(DisplayImgForSecond(lvImg, 2f, false));
        StartCoroutine(DisplayImgForSecond(readyImg, 3f, false));
        StartCoroutine(DisplayImgForSecond(goImg, 1f, true));

    }

    IEnumerator DisplayImgForSecond(GameObject imgs, float seconds, bool isLastImg)
    {
        while (isActiveDisplyaImg)
        {
            yield return new WaitForSeconds(0.1f);
        }
        isActiveDisplyaImg = true;
        imgs.SetActive(true);
        yield return new WaitForSeconds(seconds);
        imgs.SetActive(false);
        isActiveDisplyaImg = false;
        if (isLastImg)
        {
            ChangeLvPanelClosing();
        }
    }

    void ChangeLvPanelClosing()
    {
        isActiveChangeLvPanel = true;
        changeLevelPanel.SetActive(false);
        mainCharaInstance.GetComponent<SpMainPlayerController>().enabled = true;
        enemyInstance.GetComponent<SpEnemyController>().enabled = true;
        isActiveChangeLvPanel = false;
    }

    public void ResetPosition()
    {
        mainCharaInstance.transform.position = new Vector3(0, 0, -6f);
        subCharaInstance.transform.position = new Vector3(0, 0, 16f);
        enemyInstance.transform.position = new Vector3(0, 0, 6f);
        subEnemyInstance.transform.position = new Vector3(0, 0, -16f);
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
            if (CheckHaveBallAsChildren(mainCharaInstance) || spBallController.throwMan == mainCharaInstance)
            {
                return mainCharaInstance;
            }
            else if (CheckHaveBallAsChildren(subCharaInstance) || spBallController.throwMan == subCharaInstance)
            {
                return subCharaInstance;
            }
            else if (CheckHaveBallAsChildren(enemyInstance) || spBallController.throwMan == enemyInstance)
            {
                return enemyInstance;
            }
            else if (CheckHaveBallAsChildren(subEnemyInstance) || spBallController.throwMan == subEnemyInstance)
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
