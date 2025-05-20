using UnityEngine;
using System.Collections;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class SingletonManager : MonoBehaviour
{
    public GameObject[] values;

    // Use this for initialization
    void Awake()
    {
        InstantiateValues();
    }

    /// <summary>
    /// Instantiates the values.
    /// </summary>
    private void InstantiateValues()
    {
        if (values == null)
        {
            return;
        }

        foreach (GameObject value in values)
        {
            try
            {
                if (GameObject.Find(value.name) == null)
                {
                    GameObject temp = Instantiate(value, Vector3.zero, Quaternion.identity) as GameObject;
                    temp.name = value.name;
                }
            }
            catch (System.Exception e) { }
        }
    }
}
