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
            // ballcontrollerのspace key押したときと、ボール持ってるチェックを移動する
            if (Input.GetKeyDown(KeyCode.Space))
            {
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
        // 狩り 現在はPlayer2のみ反転可能
        if (gameManager.Threshold < gameManager.duration && Input.GetKeyDown(KeyCode.Space) && this.gameObject == gameManager.mainChara2Instance && !gameManager.CheckHaveBallAsChildren(this.gameObject) && !gameManager.CheckHaveBallAsChildren(gameManager.subChara2Instance))
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
        else if (Input.GetKeyDown(KeyCode.Space) && this.gameObject == gameManager.mainChara2Instance && !gameManager.CheckHaveBallAsChildren(this.gameObject) && !gameManager.CheckHaveBallAsChildren(gameManager.subChara2Instance))
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
        if (other.gameObject.tag == "Ball" && ballController != null && ballController.IsPlayerThrowing())
        {
            ballController.IsReceiverCatchSuccess = true;
        }
        if (other.gameObject.tag == "Ball" && ballController != null && !ballController.IsPlayerThrowing())
        {
            ballController.IsReceiverCatchSuccess = false;
        }
    }
}
