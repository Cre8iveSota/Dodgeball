using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpsubController : MonoBehaviour
{
    SinglePlayManager singlePlayManager;
    SPballController sPballController;
    public bool iAmThrowing;



    // Start is called before the first frame update
    void Start()
    {
        singlePlayManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SinglePlayManager>();
        if (singlePlayManager.realBallInstance != null) sPballController = singlePlayManager.realBallInstance.GetComponent<SPballController>();
    }

    // Update is called once per frame
    void Update()
    {
        singlePlayManager.CountingTimeOfHoldingShiftKey();
        if (sPballController == null) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (singlePlayManager.CheckHaveBallAsChildren(this.gameObject))
            {
                iAmThrowing = true;
                StartCoroutine(sPballController.NormalPass(singlePlayManager.subCharaInstance, singlePlayManager.mainCharaInstance));
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

        if (sPballController.IsSomeoneThrowing())
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
