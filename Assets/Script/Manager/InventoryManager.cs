using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using static UnityEditor.Progress;
using UnityEngine.ProBuilder.MeshOperations;

public enum equipSlotType { cape, boots, sword, bow }

public class InventoryManager : MonoBehaviour
{
    [HideInInspector]
    public InventorySlot clickedSlot;
    public InventorySlot[] inventorySlots;
    public InventorySlot[] equipSlots;
    public InventorySlot[] craftSlots;

    public InventoryItem[] inventoryItems;
    public InventoryItem[] equipItems;
    public InventoryItem[] craftItems;
    public InventoryItem[] allItems;

    private Dictionary<int, InventoryItem> invenItemDictionary;
    private Dictionary<int, InventoryItem> equipItemDictionary;
    private Dictionary<CraftType, InventoryItem> craftItemDictionary;
    private Dictionary<int, InventoryItem> allItemDictionary;

    public GameObject hoverWindow;
    public GameObject rightClickWindow;
    public GameObject itemControlWindow;
    public GameObject seperateBtn;
    public GameObject consumeBtn;
    public GameObject equipBtn;
    public GameObject craftBtn;
    public TextMeshProUGUI craftBtnTxt;
    public TextMeshProUGUI equipBtnTxt;
    public TextMeshProUGUI hoverTitleTxt;
    public TextMeshProUGUI descriptionTxt;
    public TextMeshProUGUI titleTxt;
    public TextMeshProUGUI amountTxt;

    private int selectedAmount = 0;
    private bool buttonClick = false;
    public bool windowOn = false;
    public bool grappedItem = false;
    private int availableCraftAmount = 0;
    private int totalCraftAmount = 0;
    private int craftItemID = 0;


    [HideInInspector]
    public int hoverId = -1;

    GameManager gameManager;
    
    void Start()
    {
        hoverWindow.SetActive(false);
        rightClickWindow.SetActive(false);
        itemControlWindow.SetActive(false);

        allItemDictionary = new Dictionary<int, InventoryItem>();

        foreach (var item in allItems)
        {
            allItemDictionary[item.id] = item;
        }

        invenItemDictionary = new Dictionary<int, InventoryItem>();

        foreach (var item in inventoryItems)
        {
            invenItemDictionary[item.id] = item;
        }

        equipItemDictionary = new Dictionary<int, InventoryItem>();

        foreach (var item in equipItems)
        {
            equipItemDictionary[item.id] = item;
        }

        craftItemDictionary = new Dictionary<CraftType, InventoryItem>();

        foreach (var item in craftItems)
        {
            craftItemDictionary[item.craftType] = item;
        }

        gameManager = GameManager.instance;

        // ���� ���� �� ����� �����Ϳ��� �� ��������
        for (int i = 0; i < inventorySlots.Length; i++) 
        {
            inventorySlots[i].id = gameManager.dataManager.inventoryData.itemID[i];
            inventorySlots[i].itemImage.sprite = Resources.Load<Sprite>(gameManager.dataManager.inventoryData.imagePath[i]);
            inventorySlots[i].isEmpty = gameManager.dataManager.inventoryData.emptySlot[i];
            inventorySlots[i].isEquipSlot = false;
            inventorySlots[i].isCraftSlot = false;
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
            equipSlots[i].isCraftSlot = false;

            // equiptype�� �� ���Կ� �ν����Ϳ��� ���� �Ҵ�

            equipSlots[i].amount = gameManager.dataManager.equipData.itemAmount[i];
            equipSlots[i].equipableInSlot = gameManager.dataManager.equipData.equipableInSlot[i];

            // ���� ���� ���� ������ ���̵� ����
            equipSlots[i].slotId = inventorySlots.Length + i;
        }

        // ���� ������ ������ ������ �ش� ���� ����Ǿ� ���� �ʿ䰡 �����Ƿ� ������ ������ �� ��
        for (int i = 0; i < craftSlots.Length; i++) 
        {
            craftSlots[i].resetSlot();
            craftSlots[i].isEquipSlot = false;
            craftSlots[i].isCraftSlot = true;

            // ���� ���� ���� ������ ���̵� ����
            craftSlots[i].slotId = (inventorySlots.Length + equipSlots.Length) + i;
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

    // �ֿ� �������� ���ο� �� ���Կ� �߰��ϴ� �޼���. �������� �巡�׾� ������� �� ���Կ� ���� �Ͱ� �ٸ� ����
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

        // �κ��丮 â�� ������� ��, ���ն��� �ִ� �ֵ��� �ٽ� �κ��丮������ �̵�
        if (!gameManager.uiManager.inventoryCanvas.activeSelf)
        {
            for (int i = 0; i < craftSlots.Length; i++)
            {
                // ���ն��� ��� ���� ������ 
                if (!craftSlots[i].isEmpty)
                {
                    Debug.Log("���ն����� �����۶�����");
                    CraftslotToInventory(craftSlots[i]);
                }
            }
        }

    }

    public void CraftslotToInventory(InventorySlot slot)
    {
        // ���ն��� �������� �ױ� ������ �������� ��
        if (checkStackability(slot.id))
        {
            //Debug.Log("�ױ� ������ �������� ���ն����� �κ��丮������");
            for (int j = 0; j < inventorySlots.Length; j++)
            {
                // �κ��丮�� ���� ���� �������� �ִٸ� �ױ�
                if (inventorySlots[j].id == slot.id)
                {
                    // Debug.Log("���� ������ ã��-----------");
                    inventorySlots[j].amount++;
                    inventorySlots[j].slotItem.setParentData();
                    slot.resetSlot();
                    break;
                }
                // ������ ���Ա��� ���� �������� �� ã�Ҵٸ�
                else if (j == inventorySlots.Length - 1)
                {
                    // Debug.Log("���� ������ �� ã��**********");
                    for (int p = 0; p < inventorySlots.Length; p++)
                    {
                        // ��� �ִ� ������ ã�Ƽ� �ش� ���ն��� ������ �̵� 
                        if (inventorySlots[p].isEmpty)
                        {
                            slot.IntoEmptySlot(inventorySlots[p], slot.slotItem);
                            break;
                        }
                        // ��� �ִ� ������ ���� ���� ������ �ʵ��� �� ������ �ϳ� ���� �� ������ ��ư�� ���ϰ� ���� ����                                    
                    }
                    break;
                }
            }
        }
        // �ױ� �Ұ����� �������� �� �� ��������
        else
        {
            for (int j = 0; j < inventorySlots.Length; j++)
            {
                // ��� �ִ� ������ ã�Ƽ� �ش� ���ն��� ������ �̵� 
                if (inventorySlots[j].isEmpty)
                {
                    slot.IntoEmptySlot(inventorySlots[j], slot.slotItem);
                    break;
                }
                // ��� �ִ� ������ ���� ���� ������ �ʵ��� �� ������ �ϳ� ���� �� ������ ��ư�� ���ϰ� ���� ����                                    
            }
        }
    }

    public InventoryItem returnInventoryItem(int id)
    {
        InventoryItem item = null;
        if (invenItemDictionary.TryGetValue(id, out InventoryItem foundItem))
        {
            item = foundItem;
        }
        return item;
    }
    
    // ���� ��� �������� ���̵� ��ȯ
    public int checkCraftID(CraftType type)
    {
        int returnVal = 0;
        if (craftItemDictionary.TryGetValue(type, out InventoryItem foundItem))
        {
            returnVal = foundItem.id;
            
        }

        return returnVal;
    }


    // �ش� ���� ���� �������� ���� ��� Ÿ�� ��ȯ
    public CraftType checkCraftType(int id)
    {
        CraftType returnVal = CraftType.None;
        if (invenItemDictionary.TryGetValue(id, out InventoryItem foundItem))
        {
            returnVal = foundItem.craftType;
        }

        return returnVal;
    }

    // �ش� ���� ����� ������ Ÿ���� ���� ��� ���� ��ȯ
    public int checkCraftAmount(CraftType type)
    {
        int returnVal = 0;
        if (craftItemDictionary.TryGetValue(type, out InventoryItem foundItem))
        {
            if (foundItem.craftType == type)
            {
                returnVal = foundItem.numForCraft;
            }
        }

        return returnVal;
    }
    

    public bool checkStackability(int id)
    {
        bool returnVal = false;
        if(invenItemDictionary.TryGetValue(id, out InventoryItem foundItem))
        {
            returnVal = foundItem.stackable;
        }

        return returnVal;
    }

    public bool checkCraftability(int id)
    {
        bool returnVal = false;
        if (allItemDictionary.TryGetValue(id, out InventoryItem foundItem))
        {
            returnVal = foundItem.craftable;
        }

        return returnVal;
    }

    public string checkEquipType(int id)
    {
        string returnVal = "none";
        if (equipItemDictionary.TryGetValue(id, out InventoryItem foundItem))
        {
            returnVal = foundItem.equipType.ToString();
        }

        return returnVal;
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
            if (invenItemDictionary.TryGetValue(id, out InventoryItem foundItem))
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
        }
        else
        {
            // �Ҹ� ���� ���������� ���� Ȯ�� �� ��ư Ȱ/��Ȱ��ȭ
            if (invenItemDictionary.TryGetValue(clickedSlot.id, out foundItem))
            {
                if (foundItem.consumable) { consumeBtn.SetActive(true); }
                else { consumeBtn.SetActive(false); }
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

        // ���� �����̶��
        if (clickedSlot.isCraftSlot)
        {
            craftBtnTxt.text = "����";
        }
        else
        {
            craftBtnTxt.text = "����";
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
        else if (titleTxt.text == "������")
        {
            if (selectedAmount < clickedSlot.amount)
            {
                selectedAmount++;
            }
        }
        else if (titleTxt.text == "����")
        {
            if (selectedAmount < availableCraftAmount)
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
        else if (titleTxt.text == "����")
        {
            totalCraftAmount = selectedAmount;
            craftingItems();          
        }

        //setDataValues();
        hideItemControlWindow();
    }

    public void craftConfirmBtn()
    {
        bool isCraftable = false;
        bool checkFull = true;
        int count = 0;
        CraftType targetType = CraftType.None;
        int targetAmount = 0;
        int amount = 0;
        List<InventorySlot> slot = new List<InventorySlot>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].isEmpty)
            {
                checkFull = false;
            }
        }

        // �κ��丮�� ���� ���� �ʾ��� �� ���� ����
        if (!checkFull)
        {
            for (int i = 0; i < craftSlots.Length; i++)
            {
                if (!craftSlots[i].isEmpty)
                {
                    count++;
                    slot.Add(craftSlots[i]);
                }
            }

            // ���ն��� ������ 2�� �̻� ���� ��
            if (count > 1)
            {
                for (int i = 0; i < slot.Count; i++)
                {
                    // ���ն��� �ִ� �������� ���ʿ� ���� ������, craftable �������̾�� ��
                    if (checkCraftability(slot[i].id))
                    {
                        isCraftable = true;
                        /* ù ��° ��� ���� ���� ���ն��� CraftType�� Ÿ������ ����, 
                        * �ش� �����ۿ� �ʿ��� ����� ���� ������ */
                        if (i == 0)
                        {
                            targetType = checkCraftType(slot[i].id);
                            craftItemID = checkCraftID(targetType);
                            targetAmount = checkCraftAmount(targetType);
                            Debug.Log(targetType.ToString());
                            amount++;
                            Debug.Log("�ʿ��� ��� ����: " + targetAmount + "���ն� �� �ִ� ��: " + count);
                            // ���ʿ� ���� ��� 3�� ��ᰡ �ʿ��ѵ� count�� 2��� ������ ���̹Ƿ� ���� �Ұ���
                            if (count != targetAmount)
                            {
                                Debug.Log("ingredients are wrong, can't craft");
                            }
                        }
                        else
                        {
                            // ������ ���ն��� �����ۿ� ���� Ÿ���� �������� �ִٸ�
                            if (checkCraftType(slot[i].id) == targetType)
                            {
                                amount++;
                            }
                            // �ƴ϶�� Ÿ�� �����ۿ� �ʿ��� ��ᰡ �ƴϹǷ� ���� �Ұ���
                            else
                            {
                                Debug.Log("ingredients are wrong, can't craft");
                            }
                        }
                    }

                    // ���� �Ұ����� �������̶��
                    else
                    {
                        Debug.Log("the item is not ingredient");
                        isCraftable = false;
                        break;
                    }
                    
                }

                // ���� ����
                if (amount == targetAmount && isCraftable)
                {
                    bool overTwo = false;

                    // ���ն��� �������� �� �� �̻��� ��쿡 �� �� �����ϰ� ������ �����ϵ���
                    for (int i = 0; i < slot.Count; i++)
                    {
                        // ���� ���ն��� �������� 2�� �̻��̶�� 
                        if (slot[i].amount > 1)
                        {     
                            // ���� ���� ������ ����
                            if (availableCraftAmount == 0)
                            {
                                availableCraftAmount = slot[i].amount;
                            }
                            else if (slot[i].amount < availableCraftAmount)
                            {
                                availableCraftAmount = slot[i].amount;
                            }
                            // ��� ���ն��� �������� �� �� �̻��̶��
                            if (i == slot.Count -1)
                            {
                                Debug.Log(availableCraftAmount);
                                overTwo = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    // ���ն��� �������� �� �� �̻��� �ƴ϶�� �������� 1�� �ִ� ���ն��� ��� ���� ����
                    if (!overTwo)
                    {
                        totalCraftAmount = 1;
                        craftingItems();
                    }
                    // ���ն��� �������� �� �� �̻��̶�� �� �� ������ ������ �����ϵ���
                    else
                    {
                        // ���� â���� ���� ������ �ϰ� ���� craftingItems �޼��忡�� ���յ� ������ �κ��丮�� �߰� �۾�
                        setItemControlWindow("����");
                    }                                   
                }
            }
        }
        else { Debug.Log("inventory is full, can't craft"); }         
    }

    // totalCraftAmount ����ŭ �������� ����� ���ն� ����
    void craftingItems()
    {
        // ���ն� ����, �� �����ϱ�
        for (int i = 0; i < craftSlots.Length; ++i)
        {
            // ���ն��� ������ ������ ���� �Ϸ��� ������ ������ ������ ���ն� ����
            if (craftSlots[i].amount - totalCraftAmount == 0)
            {
                craftSlots[i].resetSlot();
            }
            else if (craftSlots[i].amount > 1)
            {
                craftSlots[i].amount -= totalCraftAmount;
                craftSlots[i].slotItem.setParentData();
            }
        }
        // ���յ� ������ �߰��ϱ�
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].isEmpty)
            {
                for (int j = 0; j < totalCraftAmount; j++)
                {
                    AddItem(returnInventoryItem(craftItemID));
                }
                // ���� �Ŀ� �ش� �� 0���� �ʱ�ȭ
                availableCraftAmount = 0;
                totalCraftAmount = 0;
                break;
            }
        }
    }


    #region ��Ŭ�� â ��ư��
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
        int countInventorySlot = 0;
        bool emptyCraftSlot = true;
        /* ���� ����ִ� ������ 1ĭ �ۿ� �����鼭 ���ն��� ��� ��� ���� ������ 
         * �����⸦ �ؼ� �� �κ��丮���� ���� �� �κ��丮�� �ݾ� ���ն��� �������� �� ���� �Ҵ� ��츦 ���� ���� Ȯ��*/
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (!inventorySlots[i].isEmpty)
            {
                countInventorySlot++;
            }
        }

        for (int i = 0; i < craftSlots.Length; i++)
        {
            if (!craftSlots[i].isEmpty)
            {
                emptyCraftSlot = false;
                break;
            }
        }

        // ��� �ִ� ������ 1�� �ۿ� ���� ���ն��� ��� ���� �ʾҴٸ�
        if (countInventorySlot == inventorySlots.Length - 1 && !emptyCraftSlot)
        {
            Debug.Log("empty craft slot first to seperate items, your inventory is almost full");
        }
        // ���� ��찡 �ƴ� ������ ���
        else
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
        // ���ն����� ��Ŭ���ؼ� ���� ��ư�� �����ٸ� �ش� �������� �ٽ� �κ��丮������ �Űܾ� ��
        if (clickedSlot.isCraftSlot)
        {
            CraftslotToInventory(clickedSlot);
        }
        else
        {
            // ���ն��� �������� �־�� �ϹǷ� ���� �����̳ʸ� ���
            gameManager.uiManager.craftBtn();
            for (int i = 0; i < craftSlots.Length; i++)
            {
                if (craftSlots[i].isEmpty)
                {
                    clickedSlot.IntoEmptySlot(craftSlots[i], clickedSlot.slotItem);
                    break;
                }
                else if (i == craftSlots.Length - 1)
                {
                    Debug.Log("carft slot is full!");
                }
            }
        }

        hideRightClickWindow();
    }
    #endregion
}
