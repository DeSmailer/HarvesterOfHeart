using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [TextArea]
    public string info;


    public string ReturnString()
    {       
        return info;
    }
}
    