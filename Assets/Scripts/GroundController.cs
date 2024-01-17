using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public List<GameObject> grounds;
    public GameObject ballposition;
    GameObject defenciblePosition;
    GameManager gameManager;
    Transform mainCharaDefenciblePoint, mainChara2DefenciblePoint;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        grounds.Sort(SortByNumber);
    }

    private void Update()
    {

        StartCoroutine(WaitForRealBallInstanceAndContinue());
        // Debug.Log("realBallInstance position from client perspective : " + gameManager.realBallInstance.transform.position);

    }
    private IEnumerator WaitForRealBallInstanceAndContinue()
    {
        bool isExecuted = false;
        // while (gameManager.realBallInstance == null)
        while (gameManager.realBallInstance == null || gameManager.mainChara2Instance == null)
        {
            yield return null;
        }
        if (!isExecuted)
        {
            Debug.Log("gameManager.mainChara2Instance " + gameManager.mainChara2Instance);
            Debug.Log("gameManager.mainChara2Instance " + gameManager.mainChara2Instance.transform.Find("Defencivle Point"));
            mainChara2DefenciblePoint = gameManager.mainChara2Instance.transform.Find("Defencivle Point");
            isExecuted = true;
        }

        if (gameManager.realBallInstance != null && grounds.Find(i => Vector3.Distance(i.transform.position, gameManager.realBallInstance.transform.position) < 10f))
        {
            ballposition = grounds.Find(i => Vector3.Distance(i.transform.position, gameManager.realBallInstance.transform.position) < 10f);
            ballposition.transform.GetChild(0).gameObject.SetActive(true);
        }
        List<GameObject> noBallArea = grounds.FindAll(i => Vector3.Distance(i.transform.position, gameManager.realBallInstance.transform.position) > 10f);
        noBallArea.ForEach(i => i.transform.GetChild(0).gameObject.SetActive(false));

        if (gameManager.mainChara2Instance != null && grounds.Find(i => Vector3.Distance(i.transform.position, mainChara2DefenciblePoint.transform.position) < 3f))
        {
            defenciblePosition = grounds.Find(i => Vector3.Distance(i.transform.position, mainChara2DefenciblePoint.transform.position) < 3f);
            defenciblePosition.transform.GetChild(1).gameObject.SetActive(true);
        }
        List<GameObject> noDefencibleArea = grounds.FindAll(i => Vector3.Distance(i.transform.position, mainChara2DefenciblePoint.transform.position) > 3f);
        noDefencibleArea.ForEach(i => i.transform.GetChild(1).gameObject.SetActive(false));
    }
    int SortByNumber(GameObject a, GameObject b)
    {
        int aNumber = GetNumberFromName(a.name);
        int bNumber = GetNumberFromName(b.name);

        return aNumber.CompareTo(bNumber);
    }
    int GetNumberFromName(string name)
    {
        string numberPart = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
        return int.Parse(numberPart);
    }
}
