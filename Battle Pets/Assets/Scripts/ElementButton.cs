using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementButton : MonoBehaviour
{
    public Elements Element;

    public void UseCreationElement()
    {
        GameObject.Find("CreationManager").GetComponent<CreationManager>().GetElementFromButton(gameObject);
    }
}
