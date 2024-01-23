using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MainPlayerController : MonoBehaviour
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


    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        if (gameManager.realBallInstance != null) ballController = gameManager.realBallInstance.GetComponent<BallController>();
        isfaccingFront = true;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        gameManager.CountingTimeOfHoldingShiftKey();
        if (ballController == null) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {

            // Client側がボールを投げる時、マスター側でボールが消えないようにBallのパスを行うための所有権をボール所持側に譲渡する
            ballView = gameManager.realBallInstance.GetPhotonView();
            if (gameManager.GetBallHolderTeamPlayer(true) == gameManager.mainCharaInstance || gameManager.GetBallHolderTeamPlayer(true) == gameManager.subCharaInstance) ballView.TransferOwnership(PhotonNetwork.MasterClient);
            if (gameManager.GetBallHolderTeamPlayer(true) == gameManager.mainChara2Instance || gameManager.GetBallHolderTeamPlayer(true) == gameManager.subChara2Instance) ballView.TransferOwnership(PhotonNetwork.LocalPlayer);


            if (gameManager.CheckHaveBallAsChildren(this.gameObject))
            {
                iAmThrowing = true;
                StartCoroutine(ballController.NormalPass(gameManager.mainCharaInstance, gameManager.subCharaInstance));
            }
        }
        if (!iAmThrowing)
        {
            MoveMainPlayer();
            TurnMainPlayer();
        }

        // 自分がボールを持っていたら相手の方向を向く
        if (gameManager.GetBallHolderTeamPlayer(true) == gameManager.mainCharaInstance)
        {
            this.gameObject.transform.rotation = gameManager.normalRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0, -6f);
        }

    }

    private void MoveMainPlayer()
    {
        if (gameManager.duration > gameManager.Threshold) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 10)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -10)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        }
    }

    private void TurnMainPlayer()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        // ball持っていない時
        if (gameManager.Threshold < gameManager.duration && !gameManager.hasPlayer1TeamBall)
        {
            ClockWiseTurn();
        }
        else if (gameManager.Threshold > gameManager.duration && !gameManager.hasPlayer1TeamBall)
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

        bool isExecuted = false;
        if (ballController == null && !PhotonNetwork.IsMasterClient && !isExecuted)
        {
            isExecuted = true;
            ballController = gameManager.realBallInstance.GetComponent<BallController>();
        }
        Debug.Log("ballController.enableCatchBall sd" + ballController.enableCatchBall);
        Debug.Log("gameManager.hasPlayer1TeamBall 00: " + gameManager.hasPlayer1TeamBall);
        if (gameManager == null || gameManager.GetBallHolderTeamPlayer(true) == gameManager.enemyTeamMember) return;

        if (gameManager.GetBallHolderTeamPlayer(true) == this.gameObject) return;
        if (gameManager.hasPlayer1TeamBall) { ballController.isReceiverCatchSuccess = true; return; }
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
        gameManager.main2score++;
    }

    private void StandUpChara()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isHit", false);
            ballController.ChangeBallOwnerToMain1();
        }
    }
}
