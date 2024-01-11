using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public List<GameObject> grounds;

    private void Start()
    {
        grounds.Sort(SortByNumber);
        // PrintListOrder();
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
    void PrintListOrder()
    {
        for (int i = 0; i < grounds.Count; i++)
        {
            Debug.Log("Index " + i + ": " + grounds[i].name);
        }
    }
}
