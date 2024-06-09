using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{   
    Coroutine wait;
    GameManager gameManager;
    [HideInInspector] public Image slotBox;
    public Image itemImage;
    public TextMeshProUGUI amountTxt;
    public int amount;
    public string equipType = "";
    public bool isEmpty = true;
    public int id;
    private bool isHover = false;
    private bool rightClicking = false;
    public int slotId;
    public bool isEquipSlot = false;
    public bool equipableInSlot = false;
    public bool isCraftSlot = false;
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

        // ������ �ƴ� �׳� �κ��丮��������
        if (!isEquipSlot)
        {
            if (amount > 1 && !isEmpty)
            {
                amountTxt.text = amount.ToString();
            }
            else
            {
                amountTxt.text = "";
            }
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
                    Debug.Log("clicked  " + isEquipSlot);
                    rightClicking = true;
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

            if (gameManager.inventoryManager.isGrappingItem)
            {
                changeTransparency(0.5f);
            }
           
        }
        else if (!isHover)
        {
            changeTransparency(1f);
            HideHover();
        }     
        
        if (Input.GetMouseButtonUp(1))
        {
            rightClicking = false;
        }

     


    }

    void HideHover()
    {
        // ���� �� �ִ� ȣ���� ���� ���̵� �� ���̵��� �� �� ���� ȣ�� â�� �� ���� ��
        if (gameManager.inventoryManager.hoverId == slotId)
        {
            gameManager.inventoryManager.closeHoverWindow();
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
        equipableInSlot = false;
        // ������ ��� Ÿ���� ����־ �� �ǹǷ�(÷���� ����)
        if (!isEquipSlot)
        {
            equipType = "";
        }
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
        gameManager.inventoryManager.showHoverWindow(id, slotId, equipableInSlot);

    }


    // �������� ���� ������ ���ն����� ���� ��ư�� ���� �� �κ��丮�� �� ĭ���� �̵�
    public void IntoEmptySlot(InventorySlot slot, ItemDragDrop item)
    {
        slot.id = item.droppedItemID;
        slot.amount = item.droppedItemAmount;
        slot.equipableInSlot = item.isEquipable;
        slot.itemImage.sprite = Resources.Load<Sprite>(item.droppedItemImagePath);
        slot.equipType = gameManager.inventoryManager.checkEquipType(item.droppedItemID).ToString();

        // ���� �������� �ִ� ���� �ʱ�ȭ
        item.parentObj.resetSlot();

        // �������� ���� �ش� ������ ������ ������Ʈ            
        slot.slotItem.setParentData();
        slot.isEmpty = false;
    }



    /* �κ��丮���� ��Ŭ�� �� ���� ��ư�� ������ ������ ��, Ȥ�� �巡�� �� ������� ���� �������� ������ ��
     * �Ű������� ���� ����(��� �������� �̵��� ����)�� ������ ���� ��ȯ */
    public void IntoEquipSlot(InventorySlot slot, ItemDragDrop item)
    {
        // �������� ���Կ� �������� ���ٸ�, ���� �������� ���� ����
        if (slot.isEmpty && item.isEquipable)
        {
            Debug.Log("�� ���Կ� ������ ���");
            slot.id = item.droppedItemID;
            slot.amount = item.droppedItemAmount;
            slot.equipableInSlot = item.isEquipable;
            slot.itemImage.sprite = Resources.Load<Sprite>(item.droppedItemImagePath);
            slot.equipType = gameManager.inventoryManager.checkEquipType(item.droppedItemID).ToString();

            // ���� �������� �ִ� ���� �ʱ�ȭ
            item.parentObj.resetSlot();

            // �������� ���� �ش� ������ ������ ������Ʈ            
            slot.slotItem.setParentData();
            slot.isEmpty = false;
            Debug.Log("������ �������� ���� ����?: " + slot.slotItem.isEquipable + "  �̵��� �������� ���� ����?: " + item.isEquipable);

        }
        // �������� �ִ� �����̰� ���� ������ �ƴ� ���
        else if (slot.slotId != item.parentObj.slotId)
        {
            SwapSlotDatas(slot, item);
        }
    }

    // �巡�� �� ������� �������� ���ҵ� ��
    public void SwapSlotDatas(InventorySlot slot, ItemDragDrop item)
    {
        Debug.Log("������ �ִ� ���Կ� ������ ���");
        int changeID = 0;
        int changeAmount = 0;
        string changeImagePath = "";
        bool changeIsEquipable;
        string changeEquipType;


        // ���� �����۰� �������� ���� �������� �����͸� ��ȯ (��: a ���� in ��������, b ���� in �κ��丮 -> b ���� in ��������, a ���� in �κ��丮)
        changeID = slot.id;
        changeAmount = slot.amount;
        string path = AssetDatabase.GetAssetPath(slot.itemImage.sprite);
        changeImagePath = path.Replace("Assets/Resources/", "").Replace(".png", "");
        changeIsEquipable = slot.equipableInSlot;
        changeEquipType = slot.equipType;

        slot.id = item.droppedItemID;
        slot.amount = item.droppedItemAmount;
        slot.equipableInSlot = item.isEquipable;
        slot.itemImage.sprite = Resources.Load<Sprite>(item.droppedItemImagePath);
        slot.equipType = gameManager.inventoryManager.checkEquipType(item.droppedItemID).ToString();
        Debug.Log(slot.equipType);

        Debug.Log(changeID + " after change: " + slot.id);
        item.droppedItemID = changeID;
        item.droppedItemAmount = changeAmount;
        item.droppedItemImagePath = changeImagePath;
        item.isEquipable = changeIsEquipable;
        item.parentObj.equipType = changeEquipType;

        item.afterSwapItems();
        slot.slotItem.setParentData();
    }

    // �������� �ش� ���Կ� ��ӵ� ��
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        ItemDragDrop itemDragDrop = droppedItem.GetComponent<ItemDragDrop>();

        if (itemDragDrop != null)
        {
            // ������ �ƴ� ��� �����۳��� ��ȯ �� (���� �� �ִ� �����ۿ� ���ؼ�) �ױ�
            if (!isEquipSlot)
            {
                // �������� ���Կ� �������� ���ٸ�
                if (isEmpty)
                {
                    Debug.Log("�� ���Կ� ������ ���");
                    id = itemDragDrop.droppedItemID;
                    amount = itemDragDrop.droppedItemAmount;
                    itemImage.sprite = Resources.Load<Sprite>(itemDragDrop.droppedItemImagePath);
                    equipableInSlot = itemDragDrop.isEquipable;
                    equipType = itemDragDrop.parentObj.equipType;
                    Debug.Log(equipableInSlot);

                    // ���� �������� �ִ� ���� �ʱ�ȭ
                    itemDragDrop.parentObj.resetSlot();

                    // �������� ���� �ش� ������ ������ ������Ʈ            
                    slotItem.setParentData();
                    isEmpty = false;


                    Debug.Log("id: " + id + " amount: " + amount);

                }
                /* �������� �ִ� �����̰� ���� ������ �ƴ� ���,
                 * �׸��� �������� �������� �巡���� �κ��丮 �����۰� ������ �� �븻 �������� ������ �����Ǵ� �� ���� */
                else if (slotId != itemDragDrop.parentObj.slotId && !itemDragDrop.parentObj.isEquipSlot)
                {

                    // �������� ������ �����۰� ���� �������� ������ ���ٸ�, �׸��� ���� �� �ִ� �������̶��
                    if (id == itemDragDrop.droppedItemID && gameManager.inventoryManager.checkStackability(id))
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
                        SwapSlotDatas(this, itemDragDrop);
                    }
                }

            }
            // ��� ���� �������� ���� ��� 
            else
            {
                // �ش� �������� ��� �������̾�� ��
                if (itemDragDrop.isEquipable)
                {
                    // �� ��� ���� ������ �������� �������� ��� ������ ���ƾ� ��. (��: ���� ����, ���� ������ -> ���� ����)
                    if (equipType == gameManager.inventoryManager.checkEquipType(itemDragDrop.droppedItemID).ToString())
                    {
                        Debug.Log("same type");

                        // �� ��������, ��ӵ� �������� �̵�
                        IntoEquipSlot(this, itemDragDrop);
                    }
                }
                else { Debug.Log("��� ���� �������� �ƴ�"); }
            }
        }       
    }
}
