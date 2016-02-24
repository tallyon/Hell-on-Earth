using UnityEngine;
using System.Collections;

public class Building
{
    private int index;
    public int ID
    {
        get { return index; }
        set { index = value; }
    }

    private int cost;
    public int Cost
    {
        get { return cost; }
        set { cost = value; }
    }

    private GameObject prefab;
    public GameObject Prefab
    {
        get { return prefab; }
        set { prefab = value; }
    }

    private GameObject silhouettePrefab;
    public GameObject SilhouettePrefab
    {
        get { return silhouettePrefab; }
        set { silhouettePrefab = value; }
    }

    // Constructor
    public Building(int _id, int _cost, GameObject _prefab, GameObject _silhouettePrefab)
    {
        ID = _id;
        Cost = _cost;
        Prefab = _prefab;
        SilhouettePrefab = _silhouettePrefab;
    }
}
