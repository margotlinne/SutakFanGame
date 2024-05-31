using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Coroutine wait;
    GameManager gameManager;
    public Image itemInSlot;
    public TextMeshProUGUI amountTxt;
    public int amount;
    public bool isEmpty = true;
    public int id;
    private bool isHover = false;

    void Start()
    {
        gameManager = GameManager.instance;

        setImageNText();
    }

    void setImageNText()
    {
        if (isEmpty) { itemInSlot.gameObject.SetActive(false); }
        else if (!isEmpty) { itemInSlot.gameObject.SetActive(true); }


        if (amount > 1 && !isEmpty)
        {
            amountTxt.text = amount.ToString();
        }
        else
        {
            amountTxt.text = "";
        }
    }

    void Update()
    {
        setImageNText();

        if (isHover)
        {
            // ��Ŭ�� �� ������ ������ �� �۾� â ����
            if (Input.GetMouseButtonDown(1) && !isEmpty)
            {
                Debug.Log("clicked");
                gameManager.inventoryManager.clickedSlot = this;
                gameManager.inventoryManager.setRightClickWindow();
            }



            // ȣ�� �� ȣ�� â ���� (wait�� null�� �� ������ �� �ִ� �̻�����)
            if (wait == null)
            {
                wait = StartCoroutine(ShowItemDetail());
            }
            
            // ��Ŭ�� �����쳪 ������/������ ���� ������ �����찡 �� ���� �� ȣ�� �����
            if (gameManager.inventoryManager.windowOn)
            {
                HideHover();
            }
            //Debug.Log("ȣ��");        
        }
        else
        {
            HideHover();
        }        
    }

    void HideHover()
    {
        if (gameManager.inventoryManager.hoverId == id)
        {
            gameManager.inventoryManager.hideHoverWindow();
        }

        if (wait != null)
        {
            StopCoroutine(wait);
            wait = null;
        }
    }

    public void resetSlot()
    {
        id = 0;
        isEmpty = true;
        amount = 0;
        amountTxt.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }


    IEnumerator ShowItemDetail()
    {
        // 1�� ���
        yield return new WaitForSeconds(1f);


        // 1�� �Ŀ� ������ �۾�
        gameManager.inventoryManager.showHoverWindow(id);

    }

}
