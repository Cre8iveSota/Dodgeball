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


    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        if (gameManager.realBallInstance != null) ballController = gameManager.realBallInstance.GetComponent<BallController>();
        isfaccingFront = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
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
                    if (this.gameObject == gameManager.mainCharaInstance) StartCoroutine(ballController.NormalPass(gameManager.mainCharaInstance, gameManager.subCharaInstance));
                    if (this.gameObject == gameManager.mainChara2Instance) StartCoroutine(ballController.NormalPass(gameManager.mainChara2Instance, gameManager.subChara2Instance));
                }
            }
            if (!iAmThrowing)
            {
                MoveMainPlayer();
                TurnMainPlayer();
            }

            // 自分がボールを持っていたら相手の方向を向く
            if (gameManager.GetBallHolderTeamPlayer(true) == gameManager.mainCharaInstance && gameManager.mainCharaInstance == this.gameObject)
            {
                this.gameObject.transform.rotation = gameManager.normalRotation;
                this.gameObject.transform.position = new Vector3(transform.position.x, 0, -6f);
            }
            else if (gameManager.GetBallHolderTeamPlayer(true) == gameManager.mainChara2Instance && gameManager.mainChara2Instance == this.gameObject)
            {
                this.gameObject.transform.rotation = gameManager.inverseRotation;
                this.gameObject.transform.position = new Vector3(transform.position.x, 0, 6f);
            }
        }
    }

    private void MoveMainPlayer()
    {
        if (gameManager.duration > gameManager.Threshold) return;
        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 10)
            {
                transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -10)
            {
                transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x > -10)
            {
                transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x < 10)
            {
                transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
            }
        }
    }

    private void TurnMainPlayer()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (this.gameObject == gameManager.mainChara2Instance)
        {
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

        if (this.gameObject == gameManager.mainCharaInstance)
        {
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

        if (other.gameObject.tag == "Ball" && ballController != null && !ballController.IsSomeoneThrowing() && !ballController.enableCatchBall)
        {
            ballController.IsReceiverCatchSuccess = false;
        }

        Debug.Log("ballController.enableCatchBall" + ballController.enableCatchBall);
        if (other.gameObject.tag == "Ball" && ballController != null && ballController.enableCatchBall && gameManager.hasPlayer1TeamBall && this.gameObject == gameManager.mainChara2Instance)
        {
            ballController.enableBallInterupt = true;
        }
        else if (other.gameObject.tag == "Ball" && ballController != null && ballController.enableCatchBall && !gameManager.hasPlayer1TeamBall && this.gameObject == gameManager.mainCharaInstance)
        {
            ballController.enableBallInterupt = true;
        }
        else if (other.gameObject.tag == "Ball" && ballController != null && ballController.IsSomeoneThrowing() && gameManager.hasPlayer1TeamBall && this.gameObject == gameManager.mainCharaInstance)
        {
            ballController.IsReceiverCatchSuccess = true;
        }
        else if (other.gameObject.tag == "Ball" && ballController != null && ballController.IsSomeoneThrowing() && !gameManager.hasPlayer1TeamBall && this.gameObject == gameManager.mainChara2Instance)
        {
            ballController.IsReceiverCatchSuccess = true;
        }
        else
        {
            Debug.Log("Hit");
        }
    }
}
