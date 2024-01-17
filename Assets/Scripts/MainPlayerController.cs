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

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        if (gameManager.realBallInstance != null) ballController = gameManager.realBallInstance.GetComponent<BallController>();
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
            }
            TurnMainPlayer();
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
        if (Input.GetKeyDown(KeyCode.Space) && !gameManager.CheckHaveBallAsChildren(this.gameObject) && !gameManager.CheckHaveBallAsChildren(gameManager.subCharaInstance))
        {

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
