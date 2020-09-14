using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject UIobject;
    public int weight;
    public string name;
    public int id;
    public type _type;

    void Awake()
    {
        name = transform.name;
    }
}

public enum type
{
    type1 = 0,
    type2 = 1,
    type3 = 2
}