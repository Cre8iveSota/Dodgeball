using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpsubController : MonoBehaviour
{
    SpManager spManager;
    SpBallController spBallController;
    public bool iAmThrowing;
    SpGroundController spGroundController;
    SpEnemyController spEnemyController;


    // Start is called before the first frame update
    void Start()
    {
        spManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SpManager>();
        if (spManager.realBallInstance != null) spBallController = spManager.realBallInstance.GetComponent<SpBallController>();
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null) spGroundController = ground.GetComponent<SpGroundController>();
        spEnemyController = spManager.enemyInstance.GetComponent<SpEnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        spManager.CountingTimeOfHoldingShiftKey();
        if (spBallController == null) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spManager.CheckHaveBallAsChildren(this.gameObject))
            {
                if (spGroundController.ballposition != spGroundController.defenciblePosition && !spBallController.enableCatchBall)
                {
                    spEnemyController.EnableDisplayCaution(true);
                }
                iAmThrowing = true;
                StartCoroutine(spBallController.NormalPass(spManager.subCharaInstance, spManager.mainCharaInstance));
            }
        }
        if (!iAmThrowing)
        {
            MoveSubPlayer();
        }
    }
    private void MoveSubPlayer()
    {
        if (spManager.duration < spManager.Threshold) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 15)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -15)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (spBallController.throwMan != null)
        {
            spBallController.reciever = spManager.subCharaInstance;
            spBallController.isReceiverCatchSuccess = true;
        }
        else
        {
            spBallController.isReceiverCatchSuccess = false;
        }
    }
}
