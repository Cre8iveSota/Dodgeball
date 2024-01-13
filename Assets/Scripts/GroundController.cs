using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public List<GameObject> grounds;
    private GameObject ball;
    public GameObject ballposition;

    private void Start()
    {
        grounds.Sort(SortByNumber);
        ball = GameObject.FindGameObjectWithTag("Ball");
        ballposition = grounds.Find(i => Vector3.Distance(i.transform.position, ball.transform.position) < 10f);
    }

    private void Update()
    {
        if (grounds.Find(i => Vector3.Distance(i.transform.position, ball.transform.position) < 10f) != null)
        {
            ballposition = grounds.Find(i => Vector3.Distance(i.transform.position, ball.transform.position) < 10f);
            ballposition.transform.GetChild(0).gameObject.SetActive(true);
        }
        List<GameObject> noBallArea = grounds.FindAll(i => Vector3.Distance(i.transform.position, ball.transform.position) > 10f);
        noBallArea.ForEach(i => i.transform.GetChild(0).gameObject.SetActive(false));
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
