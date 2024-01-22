using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpSubEnemyController : MonoBehaviour
{
    SpManager singlePlayManager;
    SpBallController spBallController;
    SpGroundController spGroundController;
    private bool pseudoPressSpase, pseudoPressRight, pseudoPressLeft = false;
    public bool iAmThrowing;
    void Start()
    {
        singlePlayManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SpManager>();
        if (singlePlayManager.realBallInstance != null) spBallController = singlePlayManager.realBallInstance.GetComponent<SpBallController>();
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null) spGroundController = ground.GetComponent<SpGroundController>();
    }

    public void EnemySubThrow()
    {
        Debug.Log("enemy sub throw");
        pseudoPressSpase = true;
        if (pseudoPressSpase && !iAmThrowing)
        {
            pseudoPressSpase = false;
            iAmThrowing = true;
            StartCoroutine(spBallController.NormalPass(singlePlayManager.subEnemyInstance, singlePlayManager.enemyInstance));
        }
    }

    public void MoveEnemySubToRight()
    {
        if (iAmThrowing) return;
        pseudoPressRight = true;
        if (pseudoPressRight && transform.position.x > -20)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
            pseudoPressRight = false;
        }
    }

    public void MoveEnemySubToLeft()
    {
        if (iAmThrowing) return;
        pseudoPressLeft = true;
        if (pseudoPressLeft && transform.position.x < 20)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
            pseudoPressLeft = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (spBallController.ThrowMan != null)
        {
            Debug.Log("Catch start");
            spBallController.isReceiverCatchSuccess = true;
        }
        else
        {
            spBallController.isReceiverCatchSuccess = false;
        }
    }
}
