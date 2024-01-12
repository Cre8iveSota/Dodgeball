using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubPlayerController : MonoBehaviour
{
    GameManager gameManager;
    public bool isPositionAuto = true;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBallHolderTeamSubPlayer();
    }

    void MoveBallHolderTeamSubPlayer()
    {
        if (transform.position.x > 15 || transform.position.x < -15)
        {

        }
        else if (isPositionAuto && gameManager.mainChara != null && gameManager.mainChara.transform.position.x <= 10 && gameManager.mainChara.transform.position.x >= -10)
        {
            transform.position = new Vector3(gameManager.mainChara.transform.position.x, this.transform.position.y, this.transform.position.z);
        }
    }
}
