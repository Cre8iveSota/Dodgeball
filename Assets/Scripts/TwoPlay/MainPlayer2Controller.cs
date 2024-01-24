using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MainPlayer2Controller : MonoBehaviour
{
    public bool isActiveCharacter = true;
    BallController ballController;
    GameManager gameManager;
    public PhotonView photonView;
    SubPlayerController subPlayerController;
    public bool iAmThrowing;
    public bool isfaccingFront;
    PhotonView ballView;
    public Animator animator;
    public bool pleaseEnable;
    GroundController groundController;
    public GameObject caution;
    public bool isCationActivated;

    public int cautionCount = 0; // 一回しか処理したくないのにいっぱい呼ばれるため、無理やりカウントで1回に収める

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        if (gameManager.realBallInstance != null) ballController = gameManager.realBallInstance.GetComponent<BallController>();
        isfaccingFront = true;
        animator = GetComponent<Animator>();
        subPlayerController = gameManager.subChara2Instance.GetComponent<SubPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameEnd) return;
        if (!photonView.IsMine) return;
        gameManager.CountingTimeOfHoldingShiftKey();
        if (ballController == null) return;

        ballView = gameManager.realBallInstance.GetPhotonView();
        if (gameManager.GetBallHolderTeamPlayer(true) == gameManager.mainChara2Instance || gameManager.GetBallHolderTeamPlayer(true) == gameManager.subChara2Instance) ballView.TransferOwnership(PhotonNetwork.LocalPlayer);

        // if (isCationActivated)
        // {
        //     caution.SetActive(true);
        // }
        // else
        // {
        //     caution.SetActive(false);
        // }

        // ActivateCautionForOpponent();

        if (Input.GetKeyDown(KeyCode.Space) && gameManager.canMove)
        {

            // Client側がボールを投げる時、マスター側でボールが消えないようにBallのパスを行うための所有権をボール所持側に譲渡する
            if (gameManager.CheckHaveBallAsChildren(this.gameObject))
            {
                iAmThrowing = true;
                StartCoroutine(ballController.NormalPass(gameManager.mainChara2Instance, gameManager.subChara2Instance));
            }
        }
        if (!iAmThrowing && gameManager.canMove)
        {
            MoveMainPlayer();
            TurnMainPlayer();
        }

        // 自分がボールを持っていたら相手の方向を向く
        if (gameManager.GetBallHolderTeamPlayer(true) == gameManager.mainChara2Instance)
        {
            this.gameObject.transform.rotation = gameManager.inverseRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0, 6f);
        }
    }


    private void ActivateCautionForOpponent()
    {
        if (ballController.enableCatchBall)
        {
            gameManager.mainCharaInstance.GetComponent<MainPlayerController>().isCationActivated = false;
            return;
        }
        else if (iAmThrowing || subPlayerController.iAmThrowing)
        {
            gameManager.mainCharaInstance.GetComponent<MainPlayerController>().isCationActivated = true;
        }
    }

    private void MoveMainPlayer()
    {
        if (gameManager.duration > gameManager.Threshold) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x > -10)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x < 10)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
        }
    }

    private void TurnMainPlayer()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        // ball持っていない時
        if (gameManager.Threshold < gameManager.duration && gameManager.hasPlayer1TeamBall)
        {
            ClockWiseTurn();
        }
        else if (gameManager.Threshold > gameManager.duration && gameManager.hasPlayer1TeamBall)
        {
            AntiClockWiseTurn();
        }
    }

    private void AntiClockWiseTurn()
    {
        if (this.gameObject.transform.localRotation == gameManager.normalRotation)
        {
            this.gameObject.transform.localRotation = gameManager.normalRightRotation;
        }
        else if (this.gameObject.transform.localRotation == gameManager.normalRightRotation)
        {
            this.gameObject.transform.localRotation = gameManager.inverseLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z + 2f);
        }
        else if (this.gameObject.transform.localRotation == gameManager.inverseLeftRotation)
        {
            this.gameObject.transform.localRotation = gameManager.inverseRotation;
        }
        else if (this.gameObject.transform.localRotation == gameManager.inverseRotation)
        {
            this.gameObject.transform.localRotation = gameManager.inverseRightRotation;
        }
        else if (this.gameObject.transform.localRotation == gameManager.inverseRightRotation)
        {
            this.gameObject.transform.localRotation = gameManager.normalLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z - 2f);

        }
        else if (this.gameObject.transform.localRotation == gameManager.normalLeftRotation)
        {
            this.gameObject.transform.localRotation = gameManager.normalRotation;
        }
    }

    private void ClockWiseTurn()
    {
        if (this.gameObject.transform.localRotation == gameManager.normalRotation)
        {
            this.gameObject.transform.localRotation = gameManager.normalLeftRotation;
        }
        else if (this.gameObject.transform.localRotation == gameManager.normalLeftRotation)
        {
            this.gameObject.transform.localRotation = gameManager.inverseRightRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z + 2f);
        }
        else if (this.gameObject.transform.localRotation == gameManager.inverseRightRotation)
        {
            this.gameObject.transform.localRotation = gameManager.inverseRotation;
        }
        else if (this.gameObject.transform.localRotation == gameManager.inverseRotation)
        {
            this.gameObject.transform.localRotation = gameManager.inverseLeftRotation;
        }
        else if (this.gameObject.transform.localRotation == gameManager.inverseLeftRotation)
        {
            this.gameObject.transform.localRotation = gameManager.normalRightRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z - 2f);
        }
        else if (this.gameObject.transform.localRotation == gameManager.normalRightRotation)
        {
            this.gameObject.transform.localRotation = gameManager.normalRotation;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        ballView = gameManager.realBallInstance.GetPhotonView();
        ballView.TransferOwnership(PhotonNetwork.LocalPlayer);
        bool isExecuted = false;
        if (ballController == null && !PhotonNetwork.IsMasterClient && !isExecuted)
        {
            isExecuted = true;
            ballController = gameManager.realBallInstance.GetComponent<BallController>();
        }
        Debug.Log("ballController.enableCatchBall sdss" + ballController.enableCatchBall); // false
        Debug.Log("gameManager.hasPlayer1TeamBall 0a0: " + gameManager.hasPlayer1TeamBall); //true
        if (gameManager.GetBallHolderTeamPlayer(true) == this.gameObject) return;
        if (!gameManager.hasPlayer1TeamBall) { ballController.isReceiverCatchSuccess = true; return; }
        if (ballController.enableCatchBall)
        {
            ballController.enableBallInterupt = true;
        }
        else
        {
            photonView.RPC("ProcedureOfHit", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ProcedureOfHit()
    {
        SoundManager.instance.PlaySE(2);
        animator.SetBool("isHit", true);
    }

    private void StandUpChara()
    {
        if (photonView.IsMine)
        {
            ballView.TransferOwnership(PhotonNetwork.LocalPlayer);
            animator.SetBool("isHit", false);
            ballController.isMain2Hit = true;
        }
    }
}
