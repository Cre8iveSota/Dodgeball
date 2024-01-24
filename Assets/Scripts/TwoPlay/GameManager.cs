using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool hasPlayer1TeamBall;
    GameObject[] playerTeamMembers;
    public GameObject enemyTeamMember;
    public GameObject mainChara, subChara, mainChara2, subChara2;
    // public GameObject ball;
    public GameObject realBall;
    public GameObject realBallInstance;
    public GameObject mainCharaInstance, subCharaInstance, mainChara2Instance, subChara2Instance;
    BallController ballControllerForMaster;

    public int duration = 0;
    private int threshold = 10;

    public int Threshold { get => threshold; }
    PhotonView photonView;
    public Quaternion normalRotation, normalRightRotation, normalLeftRotation, inverseRotation, inverseRightRotation, inverseLeftRotation;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera subCamera;

    public TMP_Text timer, score;
    float elapsedTime;
    public int main1score, main2score = 0;
    public GameObject FinishPanel;
    public bool isGameEnd;
    public GameObject countDownPanel;
    public TMP_Text countDownText;
    public bool canMove = false;
    bool isDoneCountDown = false;


    void Start()
    {
        isGameEnd = false;

        normalRotation = Quaternion.Euler(0f, 0f, 0f);
        normalRightRotation = Quaternion.Euler(0f, 45f, 0f);
        normalLeftRotation = Quaternion.Euler(0f, 315f, 0f);
        inverseRotation = Quaternion.Euler(0f, 180f, 0f);
        inverseRightRotation = Quaternion.Euler(0f, 225f, 0f);
        inverseLeftRotation = Quaternion.Euler(0f, 135f, 0f);

        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            mainCamera.gameObject.SetActive(true);
            subCamera.gameObject.SetActive(false);

            subCharaInstance = PhotonNetwork.Instantiate(subChara.name, new Vector3(0, 0, 16f), inverseRotation);
            mainCharaInstance = PhotonNetwork.Instantiate(mainChara.name, new Vector3(0, 0, -6f), Quaternion.identity);
            photonView.RPC("InitializeInstanceMainChara", RpcTarget.Others, mainCharaInstance.GetPhotonView().ViewID);
            photonView.RPC("InitializeInstanceSubChara", RpcTarget.Others, subCharaInstance.GetPhotonView().ViewID);
            StartCoroutine(JustWaitForCreationTargetInstance(mainCharaInstance, WaitFor));
        }
        else
        {
            mainCamera.gameObject.SetActive(false);
            subCamera.gameObject.SetActive(true);
            StartCoroutine(WaitForRealBallInstanceAndContinue());
        }

        countDownPanel.SetActive(true);
        StartCoroutine(CountDownForStart());
    }

    private void Update()
    {

        if (isDoneCountDown) elapsedTime += Time.deltaTime;
        timer.text = $"Time: {elapsedTime / 60f:00}:{elapsedTime % 60:00}";
        score.text = $"Blue {main1score} - {main2score} Red";

        if (main1score == 3 || main2score == 3)
        {
            Time.timeScale = 0;
            timer.text = "Time: 00";
            Debug.Log("GameEnd");
            FinishPanel.SetActive(true);
            isGameEnd = true;
        }

        if (mainChara2Instance == null || subChara2Instance == null) return;
        if (GetBallHolderTeamPlayer(true) == mainCharaInstance || GetBallHolderTeamPlayer(true) == subCharaInstance) hasPlayer1TeamBall = true;
        if (GetBallHolderTeamPlayer(true) == mainChara2Instance || GetBallHolderTeamPlayer(true) == subChara2Instance) hasPlayer1TeamBall = false;
    }

    public void ResetPosition()
    {
        mainCharaInstance.transform.position = new Vector3(0, 0, -6f);
        subCharaInstance.transform.position = new Vector3(0, 0, 16f);
        mainChara2Instance.transform.position = new Vector3(0, 0, 6f);
        subChara2Instance.transform.position = new Vector3(0, 0, -16f);
    }
    public IEnumerator WaitForRealBallInstanceAndContinue()
    {
        while (realBallInstance == null)
        {
            yield return null;
        }
        // ボールが生成されるのを待つ
        StartCoroutine(WaitForGenerateBall());
    }
    IEnumerator CountDownForStart()
    {
        for (int i = 3; i >= 0; i--)
        {
            if (i == 0)
            {
                countDownText.text = $"Start";
                isDoneCountDown = true;
            }
            else
            {
                countDownText.text = $"{i}";
                yield return new WaitForSeconds(1);
            }

            if (isDoneCountDown)
            {
                countDownPanel.SetActive(false);
                canMove = true;
            }
        }
    }

    IEnumerator JustWaitForCreationTargetInstance(GameObject targetInstance, Func<IEnumerator> delayExecuteMethod)
    {
        while (targetInstance == null)
        { yield return null; }
        StartCoroutine(delayExecuteMethod());
    }

    public IEnumerator WaitForGenerateBall()
    {
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        ground.SetActive(false);
        MainPlayerController mainPlayerController = mainChara.GetComponent<MainPlayerController>();
        BallController ballController = realBallInstance != null ? realBallInstance.GetComponent<BallController>() : null;

        while (mainPlayerController == null || (ballController != null && !ballController.isBallReady))
        {
            yield return null;
        }
        // ground.SetActive(true);
        if (!PhotonNetwork.IsMasterClient)
        {
            mainChara2Instance = PhotonNetwork.Instantiate(mainChara2.name, new Vector3(0, 0, 6f), inverseRotation);
            subChara2Instance = PhotonNetwork.Instantiate(subChara2.name, new Vector3(0, 0, -16f), Quaternion.identity);
            photonView.RPC("InitializeInstanceMainChara2", RpcTarget.Others, mainChara2Instance.GetPhotonView().ViewID);
            photonView.RPC("InitializeInstanceSubChara2", RpcTarget.Others, subChara2Instance.GetPhotonView().ViewID);
        }
        ground.SetActive(true);
    }
    public IEnumerator WaitFor()
    {
        if (mainCharaInstance != null && realBall != null)
        {
            GameObject initializedRealBall = PhotonNetwork.Instantiate(realBall.name, Vector3.zero, Quaternion.identity);
            realBallInstance = initializedRealBall;
            photonView.RPC("InitializeBall", RpcTarget.All, realBallInstance.GetPhotonView().ViewID);
        }
        yield return null;
    }


    [PunRPC]
    private void InitializeBall(int viewID)
    {
        PhotonView ballView = PhotonView.Find(viewID);
        if (ballView != null)
        {
            realBallInstance = ballView.gameObject;
            if (realBallInstance != null)
            {
                BallController playingBall = realBallInstance.GetComponent<BallController>();
                if (playingBall != null)
                {
                    realBallInstance.transform.SetParent(mainCharaInstance.transform, false);
                    realBallInstance.transform.localPosition = new Vector3(0f, 1f, 0.4f);
                    playingBall.isBallReady = true;
                }
            }
        }
    }

    [PunRPC]
    private void InitializeInstanceMainChara(int viewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView != null)
        {
            mainCharaInstance = targetPhotonView.gameObject;
        }
    }

    [PunRPC]
    private void InitializeInstanceSubChara(int viewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView != null)
        {
            subCharaInstance = targetPhotonView.gameObject;
        }
    }

    [PunRPC]
    private void InitializeInstanceMainChara2(int viewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView != null)
        {
            mainChara2Instance = targetPhotonView.gameObject;
        }
    }

    [PunRPC]
    private void InitializeInstanceSubChara2(int viewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView != null)
        {
            subChara2Instance = targetPhotonView.gameObject;
        }
    }


    // Update is called once per frame
    public bool CheckHaveBallAsChildren(GameObject targetObject)
    {
        Transform[] allChildren = targetObject.GetComponentsInChildren<Transform>();
        bool ballExists = false;
        if (allChildren.Length == 0) return false;
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
        bool isExecuted = false;
        if (ballControllerForMaster == null && !isExecuted)
        {
            isExecuted = true;
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            if (ball != null) ballControllerForMaster = ball.GetComponent<BallController>();
        }

        if (isBallholder)
        {
            if (CheckHaveBallAsChildren(mainCharaInstance) || ballControllerForMaster.ThrowMan == mainCharaInstance)
            {
                return mainCharaInstance;
            }
            else if (CheckHaveBallAsChildren(subCharaInstance) || ballControllerForMaster.ThrowMan == subCharaInstance)
            {
                return subCharaInstance;
            }
            else if (CheckHaveBallAsChildren(mainChara2Instance) || ballControllerForMaster.ThrowMan == mainChara2Instance)
            {
                return mainChara2Instance;
            }
            else if (CheckHaveBallAsChildren(subChara2Instance) || ballControllerForMaster.ThrowMan == subChara2Instance)
            {
                return subChara2Instance;
            }
            else
            {
                Debug.Log("Warrning: You maight get unitended result from GetBallHolderTeamPlayer");
                return enemyTeamMember;
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
            else if (CheckHaveBallAsChildren(mainChara2Instance))
            {
                return subChara2Instance;
            }
            else
            {
                return mainChara2Instance;
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
