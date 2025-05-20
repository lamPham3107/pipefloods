using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showMrec : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AdsManager.ins.ShowMrec(true);
        AdsManager.ins.HideMrec(true);
    }


}
