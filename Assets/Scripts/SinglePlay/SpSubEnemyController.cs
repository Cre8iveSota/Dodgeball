using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpSubEnemyController : MonoBehaviour
{
    SinglePlayManager singlePlayManager;
    SPballController sPballController;
    SpGroundController spGroundController;
    private bool pseudoPressSpase, pseudoPressRight, pseudoPressLeft = false;
    public bool iAmThrowing;
    void Start()
    {
        singlePlayManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SinglePlayManager>();
        if (singlePlayManager.realBallInstance != null) sPballController = singlePlayManager.realBallInstance.GetComponent<SPballController>();
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
            StartCoroutine(sPballController.NormalPass(singlePlayManager.subEnemyInstance, singlePlayManager.enemyInstance));
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

        if (sPballController.ThrowMan != null)
        {
            Debug.Log("Catch start");
            sPballController.isReceiverCatchSuccess = true;
        }
        else
        {
            sPballController.isReceiverCatchSuccess = false;
        }
    }
}
