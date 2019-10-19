using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnalyze : MonoBehaviour
{

    public GraphBase graphUp;
    public GraphBase graphDown;

    GraphPainterKData graghPainterKData = new GraphPainterKData();
    public GraphPainterBase curGraphPainter;

    public int numIndex = 0;
    public CollectDataType cdt = CollectDataType.ePath0;
    public int endShowDataItemIndex = -1;


    static PanelAnalyze sInst;
    public static PanelAnalyze Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        curGraphPainter = graghPainterKData;
    }

    // Update is called once per frame
    void Update()
    {
        curGraphPainter.Update();
    }

    public void OnBtnClickRefreshData()
    {

    }

    public void OnBtnClickClose()
    {
        gameObject.SetActive(false);
    }
}
