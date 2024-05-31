using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryData 
{
    public string[] imagePath;
    public int[] itemID;
    public bool[] emptySlot;
    public int[] itemAmount;
    public InventoryData(int size)
    {
        imagePath = new string[size];
        itemID = new int[size];
        emptySlot = new bool[size];
        itemAmount = new int[size];


        // �� �迭�� �ʱⰪ ����
        for (int i = 0; i < size; i++)
        {
            imagePath[i] = "Resources/unity_builtin_extra";
            itemID[i] = 0;
            emptySlot[i] = true;
            itemAmount[i] = 0;
        }
    }
}