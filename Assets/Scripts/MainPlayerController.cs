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
            MoveMainPlayer();
            Debug.Log("so far in main" + gameManager.subCharaInstance);
            Debug.Log("ballControllerExist: " + ballController != null);
            if (ballController != null) StartCoroutine(ballController.NormalPass(gameManager.mainCharaInstance, gameManager.subCharaInstance));
            TurnMainPlayer();
        }
    }
    // private void NormalPassOrTurn()
    // {
    //     if (!Input.GetKeyDown(KeyCode.Space)) return;
    //     if (ballController != null && gameManager.CheckHaveBallAsChildren(this.gameObject))
    //     {
    //         ballController.ballDestination = gameManager.subChara.transform.position;
    //         ballController.isMovingBall = true;
    //         gameManager.realBallInstance.transform.SetParent(gameManager.subChara.transform, false);
    //     }
    // }

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
