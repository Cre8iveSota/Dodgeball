using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerController : MonoBehaviour
{
    public bool isActiveCharacter = true;
    private GameObject ball;
    BallController ballController;
    GameManager gameManager;

    SubPlayerController subPlayerController;
    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        ballController = ball.GetComponent<BallController>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        gameManager.CountingTimeOfHoldingShiftKey();
        MoveMainPlayer();
        Debug.Log("so far in main" + gameManager.subChara);
        StartCoroutine(ballController.NormalPass(gameManager.mainChara, gameManager.subChara));
        TurnMainPlayer();
    }
    // private void NormalPassOrTurn()
    // {
    //     if (!Input.GetKeyDown(KeyCode.Space)) return;
    //     if (ballController != null && gameManager.CheckHaveBallAsChildren(this.gameObject))
    //     {
    //         ballController.ballDestination = gameManager.subChara.transform.position;
    //         ballController.isMovingBall = true;
    //         ball.transform.SetParent(gameManager.subChara.transform, false);
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
        if (Input.GetKeyDown(KeyCode.Space) && !gameManager.CheckHaveBallAsChildren(this.gameObject) && !gameManager.CheckHaveBallAsChildren(gameManager.subChara))
        {

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
