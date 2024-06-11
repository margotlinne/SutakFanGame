using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface IUnitData
{
    int Initiative { get; }
    Sprite Portrait { get; }
}

public class BattleManager : MonoBehaviour
{
    public bool toMove = false;
    public bool clickedToMove = false;
    public bool inBattle = false;

    public TextMeshProUGUI unitNameTxt;
    public TextMeshProUGUI unitDamageTxt;
    public TextMeshProUGUI unitResistanceTxt;
    public TextMeshProUGUI unitInitiativeTxt;
    public TextMeshProUGUI unitTraitTxt;

    public List<GameObject> units = new List<GameObject>();

    public GameObject unitInfoWindow;
    public GameObject battleCanvas;

    void Update()
    {
        battleCanvas.SetActive(inBattle);

        if (!inBattle)
        {
            units.Clear();
        }
        else
        {
            if (units.Count > 0)
            {
                // �ֵ��� ���� �������� ������������ ����
                units.Sort((x, y) =>
                {
                    IUnitData xInitiative = x.GetComponent<IUnitData>();
                    IUnitData yInitiative = y.GetComponent<IUnitData>();
                    return xInitiative.Initiative.CompareTo(yInitiative.Initiative);
                });
            }
        }
    }

    public void showInfoWindow(string name, string damage, string initiative, string resistance, string trait)
    {
        unitInfoWindow.SetActive(true);
        GameManager.instance.uiManager.activeUI.Add(unitInfoWindow);

        unitInitiativeTxt.text = "�ֵ���: " + initiative;
    }

    public void moveBtn()
    {
        toMove = true;
        Debug.Log("clicked move button");
    }


}
