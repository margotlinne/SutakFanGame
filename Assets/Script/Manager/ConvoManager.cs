using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConvoManager : MonoBehaviour
{
    public static ConvoManager instance;
    GameManager gameManager;

    [HideInInspector]
    public bool clickToTalk;
    [HideInInspector]
    public Transform target;

    public bool isTalking = false;

    public GameObject convoCanvas;
    public TextMeshProUGUI convoTxt;
    public TextMeshProUGUI npcNameTxt;

    private int count = 0;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this.gameObject); }
    }

    void Start()
    {
        gameManager = GameManager.instance;

    }

    void Update()
    {
        if (isTalking)
        {
            if (target.gameObject.tag == "Amtak")
            {
                npcNameTxt.text = "Amtak";
            }


            if (count == 0 || Input.GetKeyDown(KeyCode.Space))
            {
                convoCanvas.SetActive(true);
                Debug.Log(count);
                nextConvo("Amtak", count);
                Debug.Log("next" + count);
            }
        }
        else
        {
            convoCanvas.SetActive(false);
        }
    }

    void nextConvo(string name, int num)
    {
        Debug.Log("�Լ� ȣ��");
        if (name == "Amtak")
        {
            if (!gameManager.acceptedQuest && !gameManager.firstConvoDone)
            {
                Debug.Log("ù ��° ��ȭ ������");
                // ù ��° ������� ��ȭ ����
                if (num < gameManager.convoData.datas.FirstTalk.Length)
                {
                    convoTxt.text = gameManager.convoData.datas.FirstTalk[num].convo;
                    count++;
                }
                else 
                {
                    isTalking = false;
                    gameManager.firstConvoDone = true;
                    // �Ű����� �̸��� ���Ƽ� �Լ� ���� count���� �����Ǵ� �� �ƴϾ���..
                    count = 0;
                    Debug.Log("ī��Ʈ �� 0���� ����");
                }
            }
            else if (gameManager.firstConvoDone && !gameManager.acceptedQuest)
            {
                Debug.Log("�� ��° ��ȭ ������");
                // �� ��° ������� ��ȭ ����
                if (num < gameManager.convoData.datas.SecondTalk.Length)
                {
                    convoTxt.text = gameManager.convoData.datas.SecondTalk[num].convo;
                    count++;
                }
                else
                {
                    isTalking = false;
                    gameManager.acceptedQuest = true;
                    count = 0;
                    Debug.Log("ī��Ʈ �� 0���� ����");
                }
            }
            else if (gameManager.getReward)
            {
                Debug.Log("�� ��° ��ȭ ������");
                if (num < gameManager.convoData.datas.RewardTalk.Length)
                {
                    convoTxt.text = gameManager.convoData.datas.RewardTalk[num].convo;
                    count++;
                }
                else
                {
                    isTalking = false;
                    gameManager.getReward = false;
                    count = 0;
                    Debug.Log("ī��Ʈ �� 0���� ����");
                }
            }
        }
    }
}
