using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PortraitCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isHover = false;


    void Update()
    {
        if (isHover)
        {
            GetComponent<Outline>().enabled = true;

            // ���� Ŭ������?
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("clicked portrait card");
            }
        }
        else
        {
            GetComponent<Outline>().enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }
}
