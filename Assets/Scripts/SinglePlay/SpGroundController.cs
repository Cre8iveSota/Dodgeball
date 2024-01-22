using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpGroundController : MonoBehaviour
{
    public List<GameObject> grounds;
    public GameObject ballposition;
    public GameObject defenciblePosition;
    SinglePlayManager singlePlayManager;
    Transform playerDefenciblePoint, enemyDefenciblePoint;

    private void Start()
    {
        singlePlayManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SinglePlayManager>();
        grounds.Sort(SortByNumber);
    }

    private void Update()
    {
        StartCoroutine(WaitForRealBallInstanceAndContinue());
    }
    private IEnumerator WaitForRealBallInstanceAndContinue()
    {
        bool isExecuted = false;
        while (singlePlayManager.realBallInstance == null || singlePlayManager.enemyInstance == null)
        {
            yield return null;
        }
        if (!isExecuted)
        {
            enemyDefenciblePoint = singlePlayManager.enemyInstance.transform.Find("Defencivle Point");
            playerDefenciblePoint = singlePlayManager.mainCharaInstance.transform.Find("Defencivle Point");
            isExecuted = true;
        }
        if (singlePlayManager.realBallInstance != null && grounds.Find(i => Vector3.Distance(i.transform.position, singlePlayManager.realBallInstance.transform.position) < 8f))
        {
            ballposition = grounds.Find(i => Vector3.Distance(i.transform.position, singlePlayManager.realBallInstance.transform.position) < 8f);
            ballposition.transform.GetChild(0).gameObject.SetActive(true);
        }
        List<GameObject> noBallArea = grounds.FindAll(i => Vector3.Distance(i.transform.position, singlePlayManager.realBallInstance.transform.position) > 8f);
        noBallArea.ForEach(i => i.transform.GetChild(0).gameObject.SetActive(false));

        if (singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.mainCharaInstance || singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.subCharaInstance)
        {
            if (singlePlayManager.enemyInstance != null && grounds.Find(i => Vector3.Distance(i.transform.position, enemyDefenciblePoint.transform.position) < 3f))
            {
                defenciblePosition = grounds.Find(i => Vector3.Distance(i.transform.position, enemyDefenciblePoint.transform.position) < 3f);
                defenciblePosition.transform.GetChild(1).gameObject.SetActive(true);
            }
            List<GameObject> noDefencibleArea = grounds.FindAll(i => Vector3.Distance(i.transform.position, enemyDefenciblePoint.transform.position) > 3f);
            noDefencibleArea.ForEach(i => i.transform.GetChild(1).gameObject.SetActive(false));
        }
        else if (singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.enemyInstance || singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.subEnemyInstance)
        {
            if (singlePlayManager.mainCharaInstance != null && grounds.Find(i => Vector3.Distance(i.transform.position, playerDefenciblePoint.transform.position) < 3f))
            {
                defenciblePosition = grounds.Find(i => Vector3.Distance(i.transform.position, playerDefenciblePoint.transform.position) < 3f);
                defenciblePosition.transform.GetChild(1).gameObject.SetActive(true);
            }
            List<GameObject> noDefencibleArea = grounds.FindAll(i => Vector3.Distance(i.transform.position, playerDefenciblePoint.transform.position) > 3f);
            noDefencibleArea.ForEach(i => i.transform.GetChild(1).gameObject.SetActive(false));
        }
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

