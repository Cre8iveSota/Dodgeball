using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isChangeActiveCharacter;
    GameObject[] playerTeamMembers;
    GameObject enemyTeamMember;
    public GameObject mainChara, subChara, mainChara2, subChara2;
    // public GameObject ball;
    public GameObject realBall;
    public GameObject realBallInstance;
    public GameObject mainCharaInstance, subCharaInstance, mainChara2Instance, subChara2Instance;

    public int duration = 0;
    private int threshold = 30;

    public int Threshold { get => threshold; }
    PhotonView photonView;

    void Start()
    {
        Debug.Log("Start method called on client. IsMasterClient: " + PhotonNetwork.IsMasterClient);

        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            subChara = PhotonNetwork.Instantiate(subChara.name, new Vector3(0, 0, 16f), Quaternion.identity);
            mainCharaInstance = PhotonNetwork.Instantiate(mainChara.name, new Vector3(0, 0, -6f), Quaternion.identity);

            photonView.RPC("InitializeInstance", RpcTarget.Others, mainCharaInstance.GetPhotonView().ViewID);
            StartCoroutine(JustWaitForCreationTargetInstance(mainCharaInstance, WaitFor));
        }
        else
        {
            StartCoroutine(WaitForRealBallInstanceAndContinue());
            if (realBallInstance != null)
            {
                Debug.Log("Not null realBall");
            }
        }
    }
    public IEnumerator WaitForRealBallInstanceAndContinue()
    {
        Debug.Log("WaitForRealBallInstanceAndContinue method called on client.");

        while (realBallInstance == null)
        {
            yield return null;
        }
        // ボールが生成されるのを待つ
        StartCoroutine(WaitForGenerateBall());
    }

    IEnumerator JustWaitForCreationTargetInstance(GameObject targetInstance, Func<IEnumerator> delayExecuteMethod)
    {
        while (targetInstance == null)
        { yield return null; }
        StartCoroutine(delayExecuteMethod());
    }

    public IEnumerator WaitForGenerateBall()
    {
        Debug.Log("WaitForGenerateBall method called on client.");

        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        ground.SetActive(false);
        MainPlayerController mainPlayerController = mainChara.GetComponent<MainPlayerController>();
        BallController ballController = realBallInstance != null ? realBallInstance.GetComponent<BallController>() : null;

        while (mainPlayerController == null || (ballController != null && !ballController.isBallReady))
        {
            yield return null;
        }
        ground.SetActive(true);
        if (!PhotonNetwork.IsMasterClient)
        {
            mainChara2 = PhotonNetwork.Instantiate(mainChara2.name, new Vector3(0, 0, 6f), Quaternion.identity);
            subChara2 = PhotonNetwork.Instantiate(subChara2.name, new Vector3(0, 0, -16f), Quaternion.identity);
        }
    }
    public IEnumerator WaitFor()
    {
        if (mainCharaInstance != null && realBall != null)
        {
            GameObject initializedRealBall = PhotonNetwork.Instantiate(realBall.name, Vector3.zero, Quaternion.identity);
            realBallInstance = initializedRealBall;
            photonView.RPC("InitializeBall", RpcTarget.All, realBallInstance.GetPhotonView().ViewID);
            Debug.Log("Not null realBall");
        }
        else
        {
            Debug.LogError("mainChara or realBall is null.");
        }
        yield return null;
    }


    [PunRPC]
    private void InitializeBall(int viewID)
    {
        Debug.Log("InitializeBall RPC called on client. ViewID: " + viewID);

        PhotonView ballView = PhotonView.Find(viewID);
        if (ballView != null)
        {
            realBallInstance = ballView.gameObject;
            BallController playingBall = realBallInstance.GetComponent<BallController>();
            Debug.Log("mainCharaTransform" + mainCharaInstance.transform);
            if (playingBall != null)
            {
                realBallInstance.transform.SetParent(mainCharaInstance.transform, false);
                realBallInstance.transform.localPosition = new Vector3(0f, 1f, 0.4f);
                playingBall.isBallReady = true;
                Debug.Log("isBallReady? " + playingBall.isBallReady);
            }
            else
            {
                Debug.LogError("mainChara is null.");
            }
        }
        else
        {
            Debug.LogError("Ball PhotonView not found.");
        }
    }

    [PunRPC]
    private void InitializeInstance(int viewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView != null)
        {
            mainCharaInstance = targetPhotonView.gameObject;
        }
    }



    private void CreateBallRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("InitializeBall", RpcTarget.All);
        }
    }

    [PunRPC]
    private void InitializeBall()
    {
        if (realBall == null) { Debug.Log("real ball is null"); return; }

        realBallInstance = PhotonNetwork.Instantiate(realBall.name, Vector3.zero, Quaternion.identity);
        // realBallInstance.transform.localScale = realBall.transform.localScale * 0.5f;

        // ボールの PhotonView から所有権の設定を行う
        PhotonView photonView = realBall.GetComponent<PhotonView>();
        photonView.OwnershipTransfer = OwnershipOption.Takeover;
        BallController playingBall = realBallInstance.GetComponent<BallController>();
        if (playingBall != null)
        {
            playingBall.isBallReady = true;
            Debug.Log("isBallReady? " + playingBall.isBallReady);
        }
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
