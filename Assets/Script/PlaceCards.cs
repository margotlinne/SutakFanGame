using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PlaceCasrds : MonoBehaviour
{
    public GameObject portraitCard;
    public GameObject endOfTurn;
    public Transform parent;

    private bool generatedCards;
    
    GameManager gameManager;
    

    void Start()
    {
        gameManager = GameManager.instance;
    }

    void Update()
    {
        if (gameManager.battleManager.inBattle && !generatedCards && gameManager.battleManager.addedTurns)
        {
            placingCards();               
        }

        
        reorderCards();
        
        
    }


    void reorderCards()
    {
        for(int i = 0; i < gameManager.battleManager.cards.Count; i++)
        {
            PortraitCard card = gameManager.battleManager.cards[i];
            // ù ��° ��
            if (i < gameManager.battleManager.units.Count)
            {
                card.transform.SetSiblingIndex(i);
                Debug.Log(card);
                Debug.Log(card.transform.GetSiblingIndex());
            }
            // �� ��° �� (�� ���� ǥ�� ������Ʈ�� ���� �α� ����)
            else
            {
                Debug.Log("?");
                card.transform.SetSiblingIndex(i + 1);
                Debug.Log(card);
                Debug.Log(card.transform.GetSiblingIndex());
            }
            
        }
    }

    void placingCards()
    {
        for (int i = 0; i < gameManager.battleManager.turns.Count; i++)
        {

            GameObject card = gameManager.battleManager.turns[i];
            GameObject newCard = Instantiate(portraitCard);

            newCard.transform.SetParent(parent, false);
            newCard.GetComponent<Image>().sprite = card.GetComponent<IUnitData>().Portrait;

            PortraitCard _card = newCard.GetComponent<PortraitCard>();
            _card.unitInitiative = card.GetComponent<IUnitData>().Initiative.ToString();
            _card.id = card.GetComponent<IUnitData>().ID;
            gameManager.battleManager.cards.Add(newCard.GetComponent<PortraitCard>());

            if (i >= gameManager.battleManager.turns.Count / 2)
            {
                newCard.GetComponent<Image>().color = Color.gray;
            }

        }
        
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                GameObject endCard = Instantiate(endOfTurn);
                endCard.transform.SetParent(parent, false);
                // ù ��° �� ���� �Ŀ� �� ���������� ���ϴ� ������Ʈ �߰�
                endCard.transform.SetSiblingIndex(gameManager.battleManager.units.Count);
            }
            else
            {
                GameObject endCard = Instantiate(endOfTurn);
                endCard.transform.SetParent(parent, false);
            }
        }       
        
        generatedCards = true;
    }

    void addOneTurn()
    {
        //for (int i = 0; i < gameManager.battleManager.units.Count; i++)
        //{
        //    GameObject card = gameManager.battleManager.units[i];
        //    GameObject newCard = Instantiate(portraitCard);

        //    newCard.transform.SetParent(parent, false);
        //    newCard.GetComponent<Image>().sprite = card.GetComponent<IUnitData>().Portrait;

        //    PortraitCard _card = newCard.GetComponent<PortraitCard>();
        //    _card.unitInitiative = card.GetComponent<IUnitData>().Initiative.ToString();
        //    _card.id = card.GetComponent<IUnitData>().ID;
        //    gameManager.battleManager.cards.Add(newCard.GetComponent<PortraitCard>());
        //}
        //GameObject endCard = Instantiate(endOfTurn);
        //endCard.transform.SetParent(parent, false);

        // �ռ� ��Ȱ��ȭ�� �� ���� ���� ���� ������� (units) �ڷ� ��ġ
    }
}
