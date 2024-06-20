using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : MonoBehaviour, IUnitData
{
    NavMeshAgent agent;
    ConvoManager convoManager;
    GameManager gameManager;
    BattleManager battleManager;
    LineRenderer lr;
    Coroutine draw;

    private bool isHover;
    private bool collidedSomething;
    private bool moveFreely;

    [HideInInspector] public int initiative;
    public int Initiative => initiative;

    public Sprite portrait;
    public Sprite Portrait => portrait;

    [HideInInspector] public int id;
    public int ID => id;

    public GameObject outlineObj;

    public bool isInBattle;
    public bool IsInBattle => isInBattle;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material.color = Color.white;
        lr.enabled = false;

        id = 3;

        GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    void Start()
    {
        convoManager = ConvoManager.instance;
        gameManager = GameManager.instance;
        battleManager = gameManager.battleManager;

        moveFreely = true;

        initiative = gameManager.dataManager.playerData.stat_initiative;


        gameManager.cameraTarget = this.gameObject;
    }

    void FaceCamera()
    {
        // ī�޶� �ٶ󺸵��� �÷��̾��� ȸ�� ����
        Vector3 cameraDirection = Camera.main.transform.position - transform.position;
        cameraDirection.y = 0; // y �� ȸ���� ���ֱ� ���� y �� ����

        // ��������Ʈ�� �ո��� ī�޶� �ٶ󺸵��� �ϱ� ���� ������ �ݴ�� ����
        cameraDirection = -cameraDirection;

        if (cameraDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void Update()
    {
        FaceCamera();

        if (convoManager.isTalking || battleManager.inBattle || gameManager.uiManager.isCanvasOn) moveFreely = false;
        else moveFreely = true;

        // �⺻ ������
        if (Input.GetMouseButtonDown(0) && moveFreely)
        {
            if (collidedSomething) { collidedSomething = false; }
            agent.isStopped = false;

            Ray ray = convoManager.clickToTalk ? Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(convoManager.target.position)) : Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.velocity = Vector3.zero;
                agent.SetDestination(hit.point);

                draw = StartCoroutine(DrawPath());
            }
        }
        // �������� ��
        else if ((agent.remainingDistance < 0.1f && agent.remainingDistance > 0) && moveFreely)
        {
            Debug.Log("arrived destination");
            arrivedDestination();
        }

        // Ŭ���� ��ȭ ��뿡�� �������� �� ---- ��ȭ��� Ŭ�� �� �ٽ� ��� �ٲ��� ���� �̰� �ذ��ؾ� ��
        if (convoManager.clickToTalk && (agent.remainingDistance <= 5f && agent.remainingDistance > 0))
        {
            Debug.Log("arrived" + agent.remainingDistance);
            if (!gameManager.firstConvoDone || !gameManager.acceptedQuest || gameManager.getReward)
            {
                convoManager.isTalking = true;
            }
            arrivedDestination();
            convoManager.clickToTalk = false;
        }





        //*************************************************** ���� **********************************************************//


        if (gameManager.battleManager.inBattle)
        {
            if (isHover)
            {
                gameManager.battleManager.idHoverOnCharacter = id;
                outlineObj.SetActive(true);
            }
            

            // ���� �� ���ʰ� �ڽ��� ��
            if (gameManager.battleManager.addedTurns)
            {
                GameObject unit = gameManager.battleManager.turns[gameManager.battleManager.currentTurn];
                if (unit.GetComponent<IUnitData>().ID == id)
                {
                    gameManager.battleManager.showActionGroup();
                }
                else
                {
                    gameManager.battleManager.hideActionGroup();
                }
            }    
            
            // ȣ���ϰ� �ִ� �� ī�尡 �ڽ��� ����Ű�� ���̸� �ƿ����� Ȱ��ȭ
            if (gameManager.battleManager.idHoverOnCard == id)
            {
                outlineObj.SetActive(true);
            }
            else if (!isHover)
            {
                outlineObj.SetActive(false);
            }

            if (gameManager.battleManager.idDoubleClick == id)
            {
                gameManager.cameraTarget = this.gameObject;
            }
        }


        // ������ ����
        if (battleManager.toMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (!battleManager.clickedToMove)
                {
                    // �̵� Ȯ�� �� ���� ��� ǥ��
                    NavMeshPath path = new NavMeshPath();
                    if (agent.CalculatePath(hit.point, path))
                    {
                        if (draw != null)
                        {
                            StopCoroutine(draw);
                        }
                        draw = StartCoroutine(DrawPathPreview(path));
                    }

                    // Ŭ���Ͽ� �ش� ��δ�� �̵�
                    if (Input.GetMouseButtonDown(0))
                    {
                        agent.isStopped = false;
                        agent.SetDestination(hit.point);
                        if (draw != null)
                        {
                            StopCoroutine(draw);
                        }
                        draw = StartCoroutine(DrawPath());
                        battleManager.clickedToMove = true;
                    }
                }                
            }

            // ��� ���� ��
            if (agent.remainingDistance < 0.1f && agent.remainingDistance > 0)
            {
                Debug.Log("arrived during battle, move action");
                arrivedDestination();                
                battleManager.toMove = false;
            }
        }        
    }

    IEnumerator DrawPathPreview(NavMeshPath path)
    {
        lr.enabled = true;
        lr.positionCount = path.corners.Length;
        for (int i = 0; i < path.corners.Length; i++)
        {
            lr.SetPosition(i, path.corners[i]);
        }
        yield return null;
    }

    IEnumerator DrawPath()
    {
        lr.enabled = true;
        yield return null;
        while(true)
        {
            int count = agent.path.corners.Length;
            lr.positionCount = count;
            for (int i = 0;i < count; i++)
            {
                lr.SetPosition(i, agent.path.corners[i]);
            }
            yield return null;
        }
    }

    void arrivedDestination()
    {
        lr.enabled = false;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.isStopped = true;

        if (draw != null)
        {
            StopCoroutine(draw);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided something?");
        if (collision.gameObject.CompareTag("object"))
        {
            Debug.Log("collided");
            collidedSomething = true;
        }

        //if (TalkManager.instance.clickToTalk)
        //{
        //    if (collision.gameObject.CompareTag(TalkManager.instance.target))
        //    {
        //        Debug.Log("arrvied talk npc");
        //        freeze();
        //    }
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "range" && !battleManager.inBattle)
        {
            Debug.Log("fight!");
            arrivedDestination();
            isInBattle = true;
            gameManager.battleManager.inBattle = true;
            gameManager.battleManager.units.Add(this.gameObject);
        }

        
    }

    public void OnMouseEnter()
    {
        isHover = true;
    }

    public void OnMouseExit()
    {
        isHover = false;
        gameManager.battleManager.idHoverOnCharacter = -1;
        outlineObj.SetActive(false);
    }
}
