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

        // ball持っていない時
        if (gameManager.Threshold < gameManager.duration && this.gameObject == gameManager.mainCharaInstance && !gameManager.CheckHaveBallAsChildren(this.gameObject) && !gameManager.CheckHaveBallAsChildren(gameManager.subCharaInstance))
        {
            if (this.gameObject.transform.localRotation == gameManager.normalRotation)
            {
                {
                    this.gameObject.transform.localRotation = gameManager.normalLeftRotation;
                }
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
        else if (this.gameObject == gameManager.mainCharaInstance && !gameManager.CheckHaveBallAsChildren(this.gameObject) && !gameManager.CheckHaveBallAsChildren(gameManager.subCharaInstance))
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

        if (gameManager.Threshold < gameManager.duration && this.gameObject == gameManager.mainChara2Instance && !gameManager.CheckHaveBallAsChildren(this.gameObject) && !gameManager.CheckHaveBallAsChildren(gameManager.subChara2Instance))
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
        else if (this.gameObject == gameManager.mainChara2Instance && !gameManager.CheckHaveBallAsChildren(this.gameObject) && !gameManager.CheckHaveBallAsChildren(gameManager.subChara2Instance))
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
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball" && ballController != null && ballController.IsSomeoneThrowing())
        {
            ballController.IsReceiverCatchSuccess = true;
        }
        if (other.gameObject.tag == "Ball" && ballController != null && !ballController.IsSomeoneThrowing())
        {
            ballController.IsReceiverCatchSuccess = false;
        }

        bool isExecuted = false;
        if (ballController == null && !PhotonNetwork.IsMasterClient && !isExecuted)
        {
            isExecuted = true;
            ballController = gameManager.realBallInstance.GetComponent<BallController>();
        }

        //main playerのキャッチ
        if ((ballController.ThrowMan == gameManager.mainCharaInstance
        || ballController.ThrowMan == gameManager.subCharaInstance)
         & this.gameObject == gameManager.mainChara2Instance)
        {
            // this.gameObject.SetActive(false);
        }
        if (ballController != null && ballController.enableCatchBall)
        {
            ballController.IsReceiverCatchSuccess = true;
            // ballController.enableBallInterupt = true;
        }


        //sub playerのキャッチ
        if ((ballController.ThrowMan == gameManager.mainChara2Instance
        || ballController.ThrowMan == gameManager.subChara2Instance)
      & this.gameObject == gameManager.mainCharaInstance)
        {
            // this.gameObject.SetActive(false);
        }
        if (ballController != null && ballController.enableCatchBall)
        {
            ballController.IsReceiverCatchSuccess = true;
            // ballController.enableBallInterupt = true;
        }
    }
}
