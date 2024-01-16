using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class SubPlayerController : MonoBehaviour
{
    GameManager gameManager;
    public bool isPositionAuto = true;
    BallController ballController;
    PhotonView photonView;
    GroundController groundController;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManagerObj != null) gameManager = gameManagerObj.GetComponent<GameManager>();
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null) ballController = ball.GetComponent<BallController>();
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null) groundController = ground.GetComponent<GroundController>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (groundController != null)
            {
                gameManager.CountingTimeOfHoldingShiftKey();
                MoveSubPlayer();
                // StartCoroutine(ballController.NormalPass(gameManager.subChara, gameManager.mainChara));
                // MoveBallHolderTeamSubPlayer();
                // Catch();
            }
        }
    }
    private void MoveSubPlayer()
    {
        if (gameManager.duration < gameManager.Threshold) return;
        if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 15)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -15)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        }
    }

    private void Catch()
    {
        if (Vector3.Distance(groundController.ballposition.transform.position, transform.position) < 6f)
        {
            Debug.Log("You can catch");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Ball" && ballController != null && ballController.IsPlayerThrowing())
        {
            ballController.IsReceiverCatchSuccess = true;
        }
        if (other.gameObject.name == "Ball" && ballController != null && !ballController.IsPlayerThrowing())
        {
            ballController.IsReceiverCatchSuccess = false;
        }
    }
}
