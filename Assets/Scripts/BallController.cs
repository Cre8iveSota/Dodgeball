using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private GameObject tmpBallPosition;
    public bool isMovingBall;
    public float ballSpeed = 0.01f;
    public Vector3 ballDestination = Vector3.zero;
    GameObject[] players;
    GameObject gammeManagerObj;
    GameManager gameManager;
    public Rigidbody rb;
    public Vector3 ballForce;
    public bool IsReceiverCatchSuccess;
    GameObject tmpBallHolder;
    GameObject ThrowMan;
    GameObject Reciever;

    Vector3 defeinedSpeed;



    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        gammeManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gammeManagerObj.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        SetDestination(ballDestination);
        if (IsReceiverCatchSuccess)
        {
            isMovingBall = false;
            transform.SetParent(Reciever.transform, false);
            transform.localPosition = new Vector3(0f, 1f, 0.4f);
            Destroy(tmpBallHolder);
            IsReceiverCatchSuccess = false;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        if (isMovingBall)
        {
            transform.position += defeinedSpeed;
        }
    }

    public IEnumerator NormalPass(GameObject passerGameObject, GameObject recieverGameObject)
    {
        if (gameManager.CheckHaveBallAsChildren(passerGameObject))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThrowMan = passerGameObject;
                Reciever = recieverGameObject;
                ballDestination = recieverGameObject.transform.position;
                isMovingBall = true;
                tmpBallHolder = Instantiate(tmpBallPosition, passerGameObject.transform.position, Quaternion.identity);
                transform.SetParent(tmpBallHolder.transform, false);
                if (ThrowMan == gameManager.mainChara) transform.localPosition = new Vector3(0f, 1f, 0.4f);
                if (ThrowMan == gameManager.subChara) transform.localPosition = new Vector3(0f, 1f, -0.4f);
                defeinedSpeed = new Vector3((ballDestination.x - transform.position.x) * ballSpeed, 0f, (ballDestination.z - transform.position.z) * ballSpeed);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ballllll");
    }

    public bool IsPlayerThrowing()
    {
        if (tmpBallHolder == null)
        {
            Debug.Log("tmpBallHolder Not find Be carefull this might be not intended" + tmpBallHolder);
            return false;
        }
        Debug.Log("Player throw ball; the player name is" + ThrowMan);
        if (tmpBallHolder.transform.position.z == -6 || tmpBallHolder.transform.position.z == 16)
        {
            return true;
        }
        return false;
    }
}
