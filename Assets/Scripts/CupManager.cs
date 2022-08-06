using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupManager : MonoBehaviour
{
    public static CupManager m_CupManager = null;
    public CupUnit object_Clone = null;
    public int GameHard = 1;//游戏难度
    public int Cup_Num = 5;//杯子数
    public int Ball_Num = 1;//球数
    public int Move_Times = 5;//每回合交换次数
    public int MovingNum = 1;//同时进行交换的杯子对数
    public List<CupUnit> All_Cup = new List<CupUnit>();
    public List<int> MovingCup = new List<int>();//储存正在移动的杯子的信息
    public int Rest_ball = 0;//剩余球的数量

    //生成杯子时x，y，z的默认移动间隔
    private float pace_x = 2.5f;
    private float pace_y = 3f;
    private float pace_z = 7f;
    private int Now_MovingNum = 0;//正在进行交换的杯子对数
    private int NowTimes = 0;//已经交换的次数
    private bool Moving_Flag_ball = false;//球藏好的信号
    private bool Moving_Flag_cup = false;//移动杯子的信号
    private bool All_Finish = false;//交换杯子是否结束

    private void Awake()
    {
        m_CupManager = this;
    }
    void Start()
    {
        Move_Times *= MovingNum;//保证多对交换时，交换次数也能有保证
        CreateObject();
    }

    void Update()
    {
        if (All_Finish)
        {
            click();
            return;
        }
        Detect_ball();
        IsChange();
    }

    public void RestData()
    {
        for (int i = 0; i < All_Cup.Count; i++)
        {
            All_Cup[i].ResetData();
        }
        Moving_Flag_ball = false;
        Moving_Flag_cup = false;
        All_Finish = false;
        NowTimes = 0;
        Rest_ball = 0;
        PlayerManager.m_PlayerManager.IsOver = false;
        // CreateObject();
        for (Rest_ball = 0; Rest_ball < Ball_Num;)//生成球
        {
            int r = Random.Range(0, All_Cup.Count);
            if (All_Cup[r].ISshowBall == false)
            {
                All_Cup[r].Init(true);
                All_Cup[r].name += "_ball";
                Rest_ball++;
            }
        }
    }
    private void click()//点击事件
    {
        if (PlayerManager.m_PlayerManager.IsOver == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Camera cam = Camera.main;
                Vector3 pos = Input.mousePosition;
                Ray r = cam.ScreenPointToRay(pos);
                if (Physics.Raycast(r, out RaycastHit hitInfor, 100))
                {
                    Debug.Log(hitInfor.collider.gameObject);
                    CupUnit hitObject = hitInfor.collider.GetComponentInParent<CupUnit>();
                    if (hitObject)
                    {
                        hitObject.Cup_Movement("up");
                        if (hitObject.ISshowBall)
                        {
                            Rest_ball--;
                            PlayerManager.m_PlayerManager.Score += 100 + GameHard * 50;
                        }
                        else
                        {
                            PlayerManager.m_PlayerManager.Health--;
                        }
                    }
                }
            }
        }
    }
    private void Detect_ball()//检测藏球动作是否结束
    {
        if (Moving_Flag_ball == false)
        {
            bool finish = true;
            for (int i = 0; i < All_Cup.Count; i++)
            {
                if (All_Cup[i].IScupMoving)
                {
                    finish = false;
                    break;
                }
            }
            if (finish)
            {
                Moving_Flag_ball = true;
            }
        }
    }
    private void IsChange()//是否进行杯子交换
    {
        if (Moving_Flag_ball)
        {
            if (!Moving_Flag_cup)
            {
                Moving_Flag_cup = true;
                StartCoroutine(ChangeEffect());
            }
        }

    }
    private IEnumerator ChangeEffect()//生成交换动作的信息
    {
        while (Moving_Flag_cup)
        {
            if (NowTimes < Move_Times)
            {
                while (Now_MovingNum < MovingNum)
                {
                    int r1 = Random.Range(0, All_Cup.Count);
                    int r2 = Random.Range(0, All_Cup.Count);
                    // while (r2 == r1)
                    // {
                    //     r2 = Random.Range(0, All_Cup.Count);
                    // }
                    while (MovingCup.Contains(r1))
                    {
                        r1 = Random.Range(0, All_Cup.Count);
                    }
                    while (MovingCup.Contains(r2) || r1 == r2)
                    {
                        r2 = Random.Range(0, All_Cup.Count);
                    }

                    CupUnit unit1 = All_Cup[r1];
                    CupUnit unit2 = All_Cup[r2];
                    unit1.Change_Init(unit2.transform.position);
                    unit2.Change_Init(unit1.transform.position);
                    MovingCup.Add(r1);
                    MovingCup.Add(r2);
                    Now_MovingNum++;
                    NowTimes++;
                }

                yield return StartCoroutine(WaitMoveFinish());
            }
            else
            {
                Moving_Flag_cup = false;
                All_Finish = true;
            }
        }
    }

    private IEnumerator WaitMoveFinish()//如果杯子还在移动则等待
    {
        int finishNum = 0;//交换完毕的对数
        List<int> temp = new List<int>();//暂存将要进行交换的数据

        while (finishNum < 1)
        {
            temp.Clear();
            for (int i = 0; i < MovingCup.Count; i++)
            {
                CupUnit unit = All_Cup[MovingCup[i]];
                if (!unit.ChangeStart)
                {
                    finishNum++;
                    temp.Add(MovingCup[i]);
                }
                if (finishNum >= 2)
                {
                    break;
                }
            }
            for (int i = 0; i < temp.Count; i++)
            {
                MovingCup.Remove(temp[i]);
            }
            yield return new WaitForSeconds(0.02f);
        }
        Now_MovingNum--;

        // while (unit1.ChangeStart || unit2.ChangeStart)
        // {
        //     yield return new WaitForSeconds(0.1f);
        // }
    }

    void CreateObject()
    {
        int line = GetLine();//行数
        int tempNum = 0;//目前生成的杯子数

        for (int j = 0; j < line; j++)//生产行
        {
            if (j == 3)//调整第四行的合适位置
            {
                pace_y = -1f;
                pace_z = -5 / 3f;
            }
            float origin_pos_x = originX_pos(tempNum);
            for (int i = 0; i < 5; i++)//生成列
            {
                if (tempNum >= Cup_Num)
                {
                    break;
                }
                Vector3 pos = new Vector3(origin_pos_x + i * pace_x, j * pace_y, j * pace_z);
                CupUnit unit = Instantiate<CupUnit>(object_Clone, pos, Quaternion.identity, GameObject.Find("Cups").GetComponent<Transform>());
                unit.name = j + "-" + i;
                All_Cup.Add(unit);
                unit.Init(false);
                tempNum++;
            }
        }

        for (Rest_ball = 0; Rest_ball < Ball_Num;)//生成球
        {
            int r = Random.Range(0, All_Cup.Count);
            if (All_Cup[r].ISshowBall == false)
            {
                All_Cup[r].Init(true);
                All_Cup[r].name += "_ball";
                Rest_ball++;
            }
        }
    }

    private int GetLine()//获取应该排列的行数
    {
        if (Cup_Num % 5 == 0)
        {
            return Cup_Num / 5;
        }
        else
        {
            return Cup_Num / 5 + 1;
        }
    }
    private float originX_pos(int tempNum)//获取每一行最左侧的生成位置
    {
        int Rest_Num;
        if (Cup_Num - tempNum > 5)
            Rest_Num = 5;
        else
            Rest_Num = Cup_Num - tempNum;

        //Debug.Log(Rest_Num);
        return (5 - Rest_Num) * (pace_x / 2);
    }
}
