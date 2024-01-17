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
    private int threshold = 10;

    public int Threshold { get => threshold; }
    PhotonView photonView;
    public Quaternion normalRotation, normalRightRotation, normalLeftRotation, inverseRotation, inverseRightRotation, inverseLeftRotation;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera subCamera;

    void Start()
    {
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
            BallController playingBall = realBallInstance.GetComponent<BallController>();
            if (playingBall != null)
            {
                realBallInstance.transform.SetParent(mainCharaInstance.transform, false);
                realBallInstance.transform.localPosition = new Vector3(0f, 1f, 0.4f);
                playingBall.isBallReady = true;
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
        if (!PhotonNetwork.IsMasterClient) return false; // 狩り
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
            if (CheckHaveBallAsChildren(mainCharaInstance))
            {
                return mainCharaInstance;
            }
            if (CheckHaveBallAsChildren(subCharaInstance))
            {
                return subCharaInstance;
            }
            return enemyTeamMember;
        }
        else
        {
            if (!CheckHaveBallAsChildren(subCharaInstance))
            {
                return subCharaInstance;
            }
            else if (!CheckHaveBallAsChildren(mainCharaInstance))
            {
                return mainCharaInstance;
            }
            else
            {
                // return mainChara as default
                Debug.LogWarning("You may get inappropriate gameObject");
                return mainCharaInstance;
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
