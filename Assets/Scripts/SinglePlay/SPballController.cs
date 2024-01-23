using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpBallController : MonoBehaviour
{
    [SerializeField] private GameObject tmpBallPosition;
    int cnt = 0;
    public Vector3 ballDestination = Vector3.zero;
    Vector3 defeinedSpeed;
    public float ballSpeed = 0.5f;
    public GameObject throwMan, reciever;
    SpGroundController sPgroundController;
    public bool isMovingBall;
    public bool enableCatchBall, enableBallInterupt;// 相手のボーるをキャッチしたことを知らせるフラグと、パスをインタラプトしたときに知らせるフラグ
    public bool isReceiverCatchSuccess;
    SpManager singlePlayManager;
    SpMainPlayerController spMainPlayerController;
    GameObject ground;
    SpsubController spsubController;
    SpEnemyController spEnemyController;
    SpSubEnemyController spSubEnemyController;
    [SerializeField] private float ballSpeedAdjusterForSp = 0.003f;

    GameObject ballTrail;
    // Start is called before the first frame update
    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null) sPgroundController = ground.GetComponent<SpGroundController>();
        GameObject gammeManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        singlePlayManager = gammeManagerObj.GetComponent<SpManager>();
        spMainPlayerController = singlePlayManager.mainCharaInstance.GetComponent<SpMainPlayerController>();
        spsubController = singlePlayManager.subCharaInstance.GetComponent<SpsubController>();
        spEnemyController = singlePlayManager.enemyInstance.GetComponent<SpEnemyController>();
        spSubEnemyController = singlePlayManager.subEnemyInstance.GetComponent<SpSubEnemyController>();
        ballTrail = GameObject.FindGameObjectWithTag("BallTrail");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z > 50f)
        {
            //subchara
            reciever = singlePlayManager.subCharaInstance;
            FixBallPosition();
        }
        else if (transform.position.z < -50f)
        {
            //aub2
            reciever = singlePlayManager.subEnemyInstance;
            FixBallPosition();
        }

        if (isMovingBall)
        {
            ballTrail.SetActive(true);
            transform.position += defeinedSpeed;
        }
        else
        {
            ballTrail.SetActive(false);
        }

        if (enableBallInterupt)
        {
            if (throwMan == singlePlayManager.mainCharaInstance || throwMan == singlePlayManager.subCharaInstance) reciever = singlePlayManager.enemyInstance;
            if (throwMan == singlePlayManager.enemyInstance || throwMan == singlePlayManager.subEnemyInstance) reciever = singlePlayManager.mainCharaInstance;
            FixBallPosition();
            enableBallInterupt = false;
        }
        else if (isReceiverCatchSuccess)
        {
            FixBallPosition();
        }
    }

    public IEnumerator NormalPass(GameObject passerGameObject, GameObject recieverGameObject)
    {
        cnt = 0;
        throwMan = passerGameObject;

        SoundManager.instance.PlaySE(4);
        if (sPgroundController != null)
        {
            if (sPgroundController.defenciblePosition == sPgroundController.ballposition)
            {
                enableCatchBall = true;
            }
        }

        reciever = recieverGameObject;
        ballDestination = recieverGameObject.transform.position;
        isMovingBall = true;
        // defeinedSpeed = new Vector3((ballDestination.x - transform.position.x) * ballSpeed * Time.deltaTime, 0f, (ballDestination.z - transform.position.z) * ballSpeed * Time.deltaTime);
        defeinedSpeed = new Vector3((ballDestination.x - transform.position.x) * ballSpeed * ballSpeedAdjusterForSp, 0f, (ballDestination.z - transform.position.z) * ballSpeed * ballSpeedAdjusterForSp);
        yield return new WaitForSeconds(1f);
    }

    private void FixBallPosition()
    {
        isMovingBall = false;
        if (spMainPlayerController != null && spMainPlayerController.iAmThrowing == true) spMainPlayerController.iAmThrowing = false;
        if (spsubController != null && spsubController.iAmThrowing == true) spsubController.iAmThrowing = false;
        if (spEnemyController != null && spEnemyController.iAmThrowing == true) spEnemyController.iAmThrowing = false;
        if (spSubEnemyController != null && spSubEnemyController.iAmThrowing == true) spSubEnemyController.iAmThrowing = false;

        GameObject tmpReciever = reciever;
        transform.SetParent(tmpReciever.transform, false);
        transform.localPosition = new Vector3(0f, 1f, 0.4f);

        isReceiverCatchSuccess = false;
        enableCatchBall = false;
        throwMan = null;
    }

    public void ChangeBallOwnerToPlayer()
    {
        reciever = singlePlayManager.mainCharaInstance;
        FixBallPosition();
        singlePlayManager.ResetPosition();
        StartCoroutine(WaitSetStartPrepareForTarget(singlePlayManager.mainCharaInstance));
    }
    public void ChangeBallOwnerToEnemy()
    {
        reciever = singlePlayManager.enemyInstance;
        FixBallPosition();
        singlePlayManager.ResetPosition();
        StartCoroutine(WaitSetStartPrepareForTarget(singlePlayManager.enemyInstance));
    }

    IEnumerator WaitSetStartPrepareForTarget(GameObject targetInstance)
    {
        if (targetInstance == singlePlayManager.mainCharaInstance)
        {
            while (singlePlayManager.GetBallHolderTeamPlayer(targetInstance) != singlePlayManager.mainCharaInstance)
            {
                yield return new WaitForSeconds(1f);
            }
            singlePlayManager.enemyScore++;
        }
        else if (targetInstance == singlePlayManager.enemyInstance)
        {
            while (singlePlayManager.GetBallHolderTeamPlayer(targetInstance) != singlePlayManager.enemyInstance)
            {
                yield return new WaitForSeconds(1f);
            }
            singlePlayManager.playerScore++;
        }
    }
}
