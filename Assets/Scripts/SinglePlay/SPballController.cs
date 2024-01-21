using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPballController : MonoBehaviour
{
    [SerializeField] private GameObject tmpBallPosition;
    int cnt = 0;
    public Vector3 ballDestination = Vector3.zero;
    Vector3 defeinedSpeed;
    public float ballSpeed = 0.5f;
    public GameObject ThrowMan, Reciever;
    public GameObject tmpBallHolder;
    SpGroundController sPgroundController;
    public bool isMovingBall;
    public bool enableCatchBall, enableBallInterupt;// 相手のボーるをキャッチしたことを知らせるフラグと、パスをインタラプトしたときに知らせるフラグ
    public bool isReceiverCatchSuccess;
    SinglePlayManager singlePlayManager;
    SinglePlayMainPlayerController singlePlayMainPlayerController;
    GameObject ground;

    // Start is called before the first frame update
    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null) sPgroundController = ground.GetComponent<SpGroundController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingBall)
        {
            transform.position += defeinedSpeed;
        }
    }

    public IEnumerator NormalPass(GameObject passerGameObject, GameObject recieverGameObject)
    {
        cnt = 0;
        ThrowMan = passerGameObject;

        if (sPgroundController != null)
        {
            if (sPgroundController.defenciblePosition == sPgroundController.ballposition)
            {
                enableCatchBall = true;
            }
        }

        Reciever = recieverGameObject;
        ballDestination = recieverGameObject.transform.position;
        isMovingBall = true;
        tmpBallHolder = Instantiate(tmpBallPosition, passerGameObject.transform.position, Quaternion.identity);
        if (tmpBallHolder.transform.position.z == -6 && tmpBallHolder.transform.position.z == -16) transform.localPosition = new Vector3(0f, 1f, 0.4f);
        if (tmpBallHolder.transform.position.z == 16 && tmpBallHolder.transform.position.z == 6) transform.localPosition = new Vector3(0f, 1f, -0.4f);

        defeinedSpeed = new Vector3((ballDestination.x - transform.position.x) * ballSpeed * Time.deltaTime, 0f, (ballDestination.z - transform.position.z) * ballSpeed * Time.deltaTime);

        yield return new WaitForSeconds(1f);
    }

    private void FixBallPosition()
    {
        isMovingBall = false;
        if (singlePlayMainPlayerController != null && singlePlayMainPlayerController.iAmThrowing == true) singlePlayMainPlayerController.iAmThrowing = false;
        // if (subCharaController != null && subCharaController.iAmThrowing == true) subCharaController.iAmThrowing = false;
        // if (mainChara2Controller != null && mainChara2Controller.iAmThrowing == true) mainChara2Controller.iAmThrowing = false;
        // if (subChara2Controller != null && subChara2Controller.iAmThrowing == true) subChara2Controller.iAmThrowing = false;
        // photonView.RPC("DestroyTmpBallHolder", RpcTarget.All, Reciever.GetPhotonView().ViewID);
        isReceiverCatchSuccess = false;
        enableCatchBall = false;
        ThrowMan = null;
    }

    public void ChangeBallOwnerToMain1()
    {
        Reciever = singlePlayManager.mainCharaInstance;
        FixBallPosition();
    }
}
