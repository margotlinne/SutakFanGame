using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    Image slotBox;
    Coroutine wait;
    GameManager gameManager;
    public Image itemImage;
    public TextMeshProUGUI amountTxt;
    public int amount;
    public bool isEmpty = true;
    public int id;
    private bool isHover = false;
    public int slotId;
    [HideInInspector] public ItemDragDrop slotItem;


    void Awake()
    {
        slotItem = itemImage.GetComponent<ItemDragDrop>();
        slotBox = GetComponent<Image>();
    }

    void Start()
    {
        gameManager = GameManager.instance;

        setImageNText();
    }

    void setImageNText()
    {
        if (isEmpty) { itemImage.gameObject.SetActive(false); }
        else if (!isEmpty) { itemImage.gameObject.SetActive(true); }


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
            if (!isEmpty)
            {
                // ��Ŭ�� �� ������ ������ �� �۾� â ����
                if (Input.GetMouseButtonDown(1))
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
            }

            if (gameManager.inventoryManager.grappedItem)
            {
                changeTransparency(0.5f);
            }
           
        }
        else if (!isHover)
        {
            changeTransparency(1f);
            HideHover();
        }        
    }

    void HideHover()
    {
        // ���� �� �ִ� ȣ���� ���� ���̵� �� ���̵��� �� �� ���� ȣ�� â�� �� ���� ��
        if (gameManager.inventoryManager.hoverId == slotId)
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
    
    void changeTransparency(float value)
    {
        Color slotColor = slotBox.color;
        slotColor.a = value;
        slotBox.color = slotColor;
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
        gameManager.inventoryManager.showHoverWindow(id, slotId);

    }

    // �������� �ش� ���Կ� ��ӵ� ��
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        ItemDragDrop itemDragDrop = droppedItem.GetComponent<ItemDragDrop>();


        // �������� ���Կ� �������� ���ٸ�
        if (isEmpty)
        {
            Debug.Log("�� ���Կ� ������ ���");
            id = itemDragDrop.droppedItemID;
            amount = itemDragDrop.droppedItemAmount;
            itemImage.sprite = Resources.Load<Sprite>(itemDragDrop.droppedItemImagePath);

            // ���� �������� �ִ� ���� �ʱ�ȭ
            itemDragDrop.parentObj.resetSlot();

            // �������� ���� �ش� ������ ������ ������Ʈ            
            slotItem.setParentData();
            isEmpty = false;


            Debug.Log("id: " + id + " amoount: " + amount);
            
        }
        // �������� �ִ� �����̳� ���� ������ �ƴ� ���
        else if (slotId != itemDragDrop.parentObj.slotId)
        {
            Debug.Log("������ �ִ� ���Կ� ������ ���");
            int changeID = 0;
            int changeAmount = 0;
            string changeImagePath = "";

            // �������� ������ �����۰� ���� �������� ������ ���ٸ�
            if (id == itemDragDrop.droppedItemID)
            {
                amount++;

                // ���� �������� �ִ� ���� �ʱ�ȭ
                itemDragDrop.parentObj.resetSlot();

                // �������� ���� �ش� ������ ������ ������Ʈ
                slotItem.setParentData();
            }
            else
            {
                // ���� �����۰� �������� ���� �������� �����͸� ��ȯ
                changeID = id;
                changeAmount = amount;
                string path = AssetDatabase.GetAssetPath(itemImage.sprite);
                changeImagePath = path.Replace("Assets/Resources/", "").Replace(".png", "");

                id = itemDragDrop.droppedItemID;
                amount = itemDragDrop.droppedItemAmount;
                itemImage.sprite = Resources.Load<Sprite>(itemDragDrop.droppedItemImagePath);

                Debug.Log(changeID + " after change: " + id);
                itemDragDrop.droppedItemID = changeID;
                itemDragDrop.droppedItemAmount = changeAmount;
                itemDragDrop.droppedItemImagePath = changeImagePath;

                itemDragDrop.afterSwapItems();
                slotItem.setParentData();
            }          
        }


    }
}
