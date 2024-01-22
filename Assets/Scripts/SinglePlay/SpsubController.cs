using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpsubController : MonoBehaviour
{
    SpManager singlePlayManager;
    SpBallController spBallController;
    public bool iAmThrowing;



    // Start is called before the first frame update
    void Start()
    {
        singlePlayManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SpManager>();
        if (singlePlayManager.realBallInstance != null) spBallController = singlePlayManager.realBallInstance.GetComponent<SpBallController>();
    }

    // Update is called once per frame
    void Update()
    {
        singlePlayManager.CountingTimeOfHoldingShiftKey();
        if (spBallController == null) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (singlePlayManager.CheckHaveBallAsChildren(this.gameObject))
            {
                iAmThrowing = true;
                StartCoroutine(spBallController.NormalPass(singlePlayManager.subCharaInstance, singlePlayManager.mainCharaInstance));
            }
        }
        if (!iAmThrowing)
        {
            MoveSubPlayer();
        }
    }
    private void MoveSubPlayer()
    {
        if (singlePlayManager.duration < singlePlayManager.Threshold) return;

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
