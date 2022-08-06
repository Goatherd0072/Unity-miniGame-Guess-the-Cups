using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupUnit : MonoBehaviour
{
    public GameObject Cup = null;
    public GameObject Ball = null;
    public bool ISshowBall = false;//杯子是否有球
    public bool IScupMoving = false;//是否杯子在向下移动遮住球,false表示未移动
    public float Wait_Time = 1;//遮住球的时间
    private float DeltaTime = 0.0f;
    public Vector3 TargetPos;
    public bool ChangeStart = false;//是否开始进行交换
    void Start()
    {

    }

    void Update()
    {
        if (IScupMoving)
        { Cup_Movement("down"); }
        Change_Start();
    }

    public void ResetData()
    {
        ISshowBall = false;
        IScupMoving = false;
        ChangeStart = false;
        Cup.transform.localPosition = new Vector3(0, 0, 0);
        Init(false);
    }
    public void Init(bool ISshow)//初始化
    {
        ISshowBall = ISshow;
        Ball.gameObject.SetActive(ISshow);
        IScupMoving = true;
        Cup.transform.localPosition = new Vector3(0, 6f, 0);
        DeltaTime = Wait_Time + Time.time;
    }
    public void Change_Init(Vector3 Tpos)//将交换目标的信息传入
    {
        TargetPos = Tpos;
        ChangeStart = true;
    }


    private void Change_Start()//进行杯子交换的动作
    {
        if (ChangeStart)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = TargetPos;
            if (Vector3.Distance(startPos, endPos) > 0.01f)
            {
                float speed = CupManager.m_CupManager.GameHard * 0.01f;
                speed = Mathf.Min(0.1f, speed);
                Vector3 pos = Vector3.Lerp(startPos, endPos, speed);
                transform.position = pos;
            }
            else
            {
                transform.position = TargetPos;
                ChangeStart = false;//交换结束
            }
        }
    }

    public void Cup_Movement(string t)//杯子的移动方式
    {
        if (t == "down")//向下遮住球
        {
            if (DeltaTime < Time.time)
            {
                if (Cup.transform.localPosition.y > 3)
                {
                    float posY = Cup.transform.localPosition.y - Time.deltaTime;
                    Cup.transform.localPosition = new Vector3(0, posY, 0);
                }
                else
                {
                    IScupMoving = false;//此杯子已经遮住球
                }
            }
        }
        if (t == "up")//打开展示
        {
            Cup.transform.localPosition = new Vector3(0, 6f, 0);
        }
    }

}
