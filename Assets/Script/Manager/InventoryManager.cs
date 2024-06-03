using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public enum equipSlotType { cape, boots, sword, bow }

public class InventoryManager : MonoBehaviour
{
    [HideInInspector]
    public InventorySlot clickedSlot;
    public InventorySlot[] inventorySlots;
    public InventorySlot[] equipSlots;

    public InventoryItem[] inventoryItems;
    public InventoryItem[] equipItems;

    private Dictionary<int, InventoryItem> itemDictionary;
    private Dictionary<int, InventoryItem> equipItemDictionary;

    public GameObject hoverWindow;
    public GameObject rightClickWindow;
    public GameObject itemControlWindow;
    public GameObject seperateBtn;
    public GameObject consumeBtn;
    public GameObject equipBtn;
    public GameObject craftBtn;
    public TextMeshProUGUI equipBtnTxt;
    public TextMeshProUGUI hoverTitleTxt;
    public TextMeshProUGUI descriptionTxt;
    public TextMeshProUGUI titleTxt;
    public TextMeshProUGUI amountTxt;

    private int selectedAmount = 0;
    private bool buttonClick = false;
    public bool windowOn = false;
    public bool grappedItem = false;



    [HideInInspector]
    public int hoverId = -1;

    GameManager gameManager;
    
    void Start()
    {
        hoverWindow.SetActive(false);
        rightClickWindow.SetActive(false);
        itemControlWindow.SetActive(false);

        itemDictionary = new Dictionary<int, InventoryItem>();

        foreach (var item in inventoryItems)
        {
            itemDictionary[item.id] = item;
        }

        equipItemDictionary = new Dictionary<int, InventoryItem>();

        foreach (var item in equipItems)
        {
            equipItemDictionary[item.id] = item;
        }

        gameManager = GameManager.instance;

        // ���� ���� �� ����� �����Ϳ��� �� ��������
        for (int i = 0; i < inventorySlots.Length; i++) 
        {
            inventorySlots[i].id = gameManager.dataManager.inventoryData.itemID[i];
            inventorySlots[i].itemImage.sprite = Resources.Load<Sprite>(gameManager.dataManager.inventoryData.imagePath[i]);
            inventorySlots[i].isEmpty = gameManager.dataManager.inventoryData.emptySlot[i];
            inventorySlots[i].isEquipSlot = false;
            inventorySlots[i].equipType = gameManager.dataManager.inventoryData.equipType[i];

            inventorySlots[i].amount = gameManager.dataManager.inventoryData.itemAmount[i];
            inventorySlots[i].equipType = gameManager.dataManager.inventoryData.equipType[i];
            inventorySlots[i].equipableInSlot = gameManager.dataManager.inventoryData.equipableInSlot[i];

            // ���� ���� ���� ������ ���̵� ����
            inventorySlots[i].slotId = i;
        }

        for (int i = 0; i < equipSlots.Length; i++)
        {
            equipSlots[i].id = gameManager.dataManager.equipData.itemID[i];
            equipSlots[i].itemImage.sprite = Resources.Load<Sprite>(gameManager.dataManager.equipData.imagePath[i]);
            equipSlots[i].isEmpty = gameManager.dataManager.equipData.emptySlot[i];
            equipSlots[i].isEquipSlot = true;

            // equiptype�� �� ���Կ� �ν����Ϳ��� ���� �Ҵ�

            equipSlots[i].amount = gameManager.dataManager.equipData.itemAmount[i];
            equipSlots[i].equipableInSlot = gameManager.dataManager.equipData.equipableInSlot[i];

            // ���� ���� ���� ������ ���̵� ����
            equipSlots[i].slotId = inventorySlots.Length + i;
        }
    }


    public void setDataValues()
    {
        // ������ �Է��ϱ�
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            gameManager.dataManager.inventoryData.itemID[i] = inventorySlots[i].id;

            string path = AssetDatabase.GetAssetPath(inventorySlots[i].itemImage.sprite);
            path = path.Replace("Assets/Resources/", "").Replace(".png", "");
            gameManager.dataManager.inventoryData.imagePath[i] = path;

            gameManager.dataManager.inventoryData.emptySlot[i] = inventorySlots[i].isEmpty;
            gameManager.dataManager.inventoryData.itemAmount[i] = inventorySlots[i].amount;

            gameManager.dataManager.inventoryData.equipType[i] = inventorySlots[i].equipType;
            gameManager.dataManager.inventoryData.equipableInSlot[i] = inventorySlots[i].equipableInSlot;
        }

        for (int i = 0; i < equipSlots.Length; i++)
        {
            gameManager.dataManager.equipData.itemID[i] = equipSlots[i].id;

            string path = AssetDatabase.GetAssetPath(equipSlots[i].itemImage.sprite);
            path = path.Replace("Assets/Resources/", "").Replace(".png", "");
            gameManager.dataManager.equipData.imagePath[i] = path;

            gameManager.dataManager.equipData.emptySlot[i] = equipSlots[i].isEmpty;
            gameManager.dataManager.equipData.itemAmount[i] = equipSlots[i].amount;

            gameManager.dataManager.equipData.equipableInSlot[i] = equipSlots[i].equipableInSlot;
        }
    }

    public bool AddItem(InventoryItem inventoryItem)
    {
        bool added = false;

        for (int i = 0; i < inventorySlots.Length; i++) 
        {
            InventorySlot slot = inventorySlots[i];
           // Debug.Log(i + "��° ��ȸ �� " + "ȹ���� �������� id: " + inventoryItem.id + " Ȯ������ ���� ������ �������� id: " + slot.id);

            // ���Կ� ���� �������� �߰����� ��        
            if (slot.id == inventoryItem.id)
            {
                Debug.Log("���� ������ Ȯ��");
                // ���� �� �ִ� �������� ��
                if (checkStackability(slot.id))
                {
                    Debug.Log("���� �� �ִ� ����������, ����");
                    slot.amount++;
                    slot.slotItem.setParentData();
                    added = true;
                    //setDataValues();
                    break;
                }
                // ���� �� ���� �������� ��
                else
                {
                    Debug.Log("���� �� ����: " + checkStackability(slot.id));
                    added = IntoEmptySlot(inventoryItem);
                    break;
                }
            }
            // ��� ���� �˻� ��� ���� �ְ� ���� �� 
            else if (i == inventorySlots.Length - 1)
            {
                added = IntoEmptySlot(inventoryItem);
                break;
            }
        }

        return added;
    }

    bool IntoEmptySlot(InventoryItem inventoryItem)
    {
        bool val = false;
        // �� ���Կ� �߰�
        for (int j = 0; j < inventorySlots.Length; j++)
        {
            InventorySlot slot = inventorySlots[j];
            if (slot.isEmpty)
            {
                slot.itemImage.sprite = inventoryItem.icon;
                slot.isEmpty = false;
                slot.id = inventoryItem.id;
                slot.amount = 1;
                slot.equipType = inventoryItem.equipType.ToString();
                val = true;
                //Debug.Log("���: "  + AssetDatabase.GetAssetPath(slots[j].itemInSlot.sprite));
                //setDataValues();
                if (inventoryItem.equipable)
                {
                    Debug.Log("��� ������ ȹ��");
                    slot.equipableInSlot = true;
                }
                else
                {
                    Debug.Log("�⺻ ������ ȹ��");
                    slot.equipableInSlot = false;
                }

                slot.slotItem.setParentData();
                break;
            }
            // �� ������ ���� ��
            else if (j == inventorySlots.Length - 1)
            {
                Debug.Log("inventory is full!");
                val = false;
                break;
            }
        }
        return val;
    }


    void Update()
    {
        // ȣ�� â �߰� �ش� ĭ���� ���콺�� ����� ȣ�� â ���ִ� �� ��� ����?

        // �ش� ui â�� Ȱ��ȭ�Ǿ� ���� ���� �ؽ�Ʈ ������Ʈ
        if (itemControlWindow.activeSelf)
        {
            amountTxt.text = selectedAmount.ToString();
        }


    }

    

    public bool checkStackability(int id)
    {
        if(itemDictionary.TryGetValue(id, out InventoryItem foundItem))
        {
            return foundItem.stackable;
        }
        else
        {
            return false;
        }
    }

    public string checkEquipType(int id)
    {
        if (equipItemDictionary.TryGetValue(id, out InventoryItem foundItem))
        {
            return foundItem.equipType.ToString();
        }
        else
        {
            return "none";
        }
    }

    public void showHoverWindow(int id, int slotId, bool equipable)
    {
        hoverId = slotId;
        Vector2 screenPosition = Input.mousePosition;
        Vector2 pos = new Vector2(screenPosition.x + 200, screenPosition.y - 100);

        hoverWindow.transform.position = pos;

        // �⺻ �κ��丮 ������ �̾ ������ id ���� ���������Ƿ�
        if (!equipable)
        {
            if (itemDictionary.TryGetValue(id, out InventoryItem foundItem))
            {
                hoverTitleTxt.text = foundItem.itemName;
                descriptionTxt.text = foundItem.description;
            }
        }
        else
        {
            if (equipItemDictionary.TryGetValue(id, out InventoryItem foundItem))
            {
                hoverTitleTxt.text = foundItem.itemName;
                descriptionTxt.text = foundItem.description;
            }
        }
        hoverWindow.SetActive(true);
        //Debug.Log("â ���� �Լ� ����");
        // Debug.Log(hoverId);
    }

    public void hideHoverWindow()
    {
        hoverWindow.SetActive(false);
        hoverId = -1;
    }

    public void setRightClickWindow()
    {
        gameManager.uiManager.activeUI.Add(rightClickWindow);
        buttonClick = false;
        windowOn = true;

        rightClickWindow.SetActive(true);
        Vector2 screenPosition = Input.mousePosition;
        Vector2 pos = new Vector2(screenPosition.x + 70, screenPosition.y - 70);

        rightClickWindow.transform.position = pos;

        // ������ ������ 1����� ������ ��ư�� ��ǥ��
        if (clickedSlot.amount == 1){ seperateBtn.SetActive(false); }
        else { seperateBtn.SetActive(true); }

        InventoryItem foundItem;

        // ��� �������� ��
        if (clickedSlot.equipableInSlot)
        {
            // ���� �Ҹ� �Ұ����ϹǷ� �Ҹ� ��ư ��Ȱ��ȭ
            consumeBtn.SetActive(false);            
            
            // ���� ������ �Ұ����ϹǷ� ���� ��ư ��Ȱ��ȭ
            craftBtn.SetActive(false);
        }
        else
        {
            // �Ҹ� ���� ���������� ���� Ȯ�� �� ��ư Ȱ/��Ȱ��ȭ
            if (itemDictionary.TryGetValue(clickedSlot.id, out foundItem))
            {
                if (foundItem.consumable) { consumeBtn.SetActive(true); }
                else { consumeBtn.SetActive(false); }
            }

            // ���� ���� ���������� ���� Ȯ�� �� ��ư Ȱ/��Ȱ��ȭ
            if (itemDictionary.TryGetValue(clickedSlot.id, out foundItem))
            {
                if (foundItem.craftable) { craftBtn.SetActive(true); }
                else { craftBtn.SetActive(false); }
            }

        }

        // ��� �����̶��
        if (clickedSlot.isEquipSlot)
        {
            // ���� ��ư Ȱ��ȭ �� �ؽ�Ʈ ����
            equipBtn.SetActive(true);
            equipBtnTxt.text = "��������";
        }
        // �⺻ �κ��丮 ���Կ��� �������� ������ ���� �ؽ�Ʈ ����
        else
        {
            if (clickedSlot.equipableInSlot)
            {
                if (clickedSlot.equipType == "cape")
                {
                    equipBtnTxt.text = "����(����)";
                }
                else if (clickedSlot.equipType == "boots")
                {
                    equipBtnTxt.text = "����(����)";
                }
                else if (clickedSlot.equipType == "sword")
                {
                    equipBtnTxt.text = "����(��)";
                }
                else if (clickedSlot.equipType == "bow")
                {
                    equipBtnTxt.text = "����(Ȱ)";
                }
                equipBtn.SetActive(true);


                Debug.Log("���� ���� �����ۿ� ��Ŭ��" + equipBtnTxt.text);
            }
            // ��� �������� �ƴ� ��� ���� ��ư ��Ȱ��ȭ
            else
            {
                equipBtn.SetActive(false);
            }
        }
        
    }
    
    void hideRightClickWindow()
    {
        rightClickWindow.SetActive(false);
        gameManager.uiManager.activeUI.Remove(rightClickWindow);
        windowOn = false;
    }

    void setItemControlWindow(string str)
    {
        windowOn = true;
        selectedAmount = 0;
        titleTxt.text = str;
        itemControlWindow.SetActive(true);
        gameManager.uiManager.activeUI.Add(itemControlWindow);
    }

    void hideItemControlWindow()
    {
        itemControlWindow.SetActive(false);
        gameManager.uiManager.activeUI.Remove(itemControlWindow);
        windowOn = false;

    }

    void SeperateItems(int num)
    {
        if (num > 0)
        {
            clickedSlot.amount -= num;

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];
                // ��� �ִ� ���Կ� �߰�
                if (slot.isEmpty)
                {
                    slot.id = clickedSlot.id;
                    slot.amount = num;
                    slot.itemImage.sprite = clickedSlot.itemImage.sprite;
                    slot.isEmpty = false;
                    slot.equipType = clickedSlot.equipType;

                    // ������ ���԰� ���� ����� �������� �߰��� ������ ������ �����͸� �θ� ���� ������ ����
                    slot.slotItem.setParentData();
                    clickedSlot.slotItem.setParentData();
                    //setDataValues();
                    break;
                }
            }
        }        

    }

    public void increaseAmountBtn()
    {
        if (titleTxt.text == "������")
        {
            if (selectedAmount < clickedSlot.amount - 1)
            {
                selectedAmount++;
            }
        }
        else
        {
            if (selectedAmount < clickedSlot.amount)
            {
                selectedAmount++;
            }
        }
        
    }

    public void decreaseAmountBtn()
    {
        if (selectedAmount > 0)
        {
            selectedAmount--;
        }
    }

    public void cancelBtn()
    {
        hideItemControlWindow();
    }

    public void confirmBtn()
    {
        if (titleTxt.text == "������")
        {
            clickedSlot.amount -= selectedAmount;
            if( clickedSlot.amount == 0) { clickedSlot.resetSlot(); }
            // ������ ���� amount ���� ������Ʈ�ϱ� ���� ������ ������ ���� �θ� ������ ������ ����
            clickedSlot.slotItem.setParentData();
        }
        else if (titleTxt.text == "������")
        {
            SeperateItems(selectedAmount);
        }

        //setDataValues();
        hideItemControlWindow();
    }

    public void DiscardBtn()
    {        
        buttonClick = true;

        // 1������ ���� ��� �� ���� �׼��� ���� �ٰ����� ����
        if (clickedSlot.amount > 1)
        {
            setItemControlWindow("������");
        }
        else
        {
            clickedSlot.resetSlot();
            windowOn = false;
        }

        hideRightClickWindow();

    }

    public void SeperateBtn()
    {
        buttonClick = true;

        // 1������ ���� ��� �� ���� �׼��� ���� �ٰ����� ����
        if (clickedSlot.amount > 1)
        {
            for (int i = 0; i < inventorySlots.Length; i++) 
            {
                InventorySlot slot = inventorySlots[i].GetComponent<InventorySlot>();
                if (slot.isEmpty) { break; }
                else if (i == inventorySlots.Length - 1) { Debug.Log("invetory is full, can't seperate the items!"); }
            }
            setItemControlWindow("������");
        }

        hideRightClickWindow();
    }

    public void ConsumeBtn()
    {
        buttonClick = true;

        clickedSlot.amount--;
        if (clickedSlot.amount == 0) { clickedSlot.resetSlot(); }
        Debug.Log("drank potion");

        hideRightClickWindow();            
        // �Ҹ��ϰ� ���� amount ���� ������Ʈ�ϱ� ���� ������ ������ ���� �θ� ������ ������ ����
        clickedSlot.slotItem.setParentData();
    }

    public void EquipBtn()
    {
        // ���� Ŭ���� ������ �Ϲ� �κ��丮 �����̶�� ���������� �̵��̹Ƿ�
        if (!clickedSlot.isEquipSlot)
        {
            if (clickedSlot.equipType == "cape")
            {
                // �ش� �������� (ù ��° �Ű�����) Ŭ���� ������ ��� ������(�� ��° �Ű�����)�� �̵�
                clickedSlot.IntoEquipSlot(equipSlots[(int)equipSlotType.cape], clickedSlot.slotItem);
            }
            else if (clickedSlot.equipType == "boots")
            {
                clickedSlot.IntoEquipSlot(equipSlots[(int)equipSlotType.boots], clickedSlot.slotItem);
            }
            else if (clickedSlot.equipType == "sword")
            {
                clickedSlot.IntoEquipSlot(equipSlots[(int)equipSlotType.sword], clickedSlot.slotItem);
            }
            else if (clickedSlot.equipType == "bow")
            {
                clickedSlot.IntoEquipSlot(equipSlots[(int)equipSlotType.bow], clickedSlot.slotItem);
            }
        }

        // �������� Ŭ���̶�� ���� �����̹Ƿ�
        else
        {
           for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isEmpty)
                {
                    clickedSlot.IntoEmptySlot(inventorySlots[i], clickedSlot.slotItem);
                    break;
                }
            }
        }

        hideRightClickWindow();
        
    }

    public void CraftBtn()
    {

        hideRightClickWindow();
    }
}
