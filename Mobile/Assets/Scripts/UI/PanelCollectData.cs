using LotteryAnalyze;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCollectData : MonoBehaviour
{
    List<DateTime> jobLst = new List<DateTime>();
    List<DateTime> jobUnFinishLst = new List<DateTime>();
    int curJobIndex = -1;

    static PanelCollectData sInst;
    public static PanelCollectData Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;
    }

    public UnityEngine.UI.InputField txtSY, txtSM, txtSD, txtEY, txtEM, txtED;
    public UnityEngine.UI.Text console;
    public UnityEngine.UI.Image progressBar;
    float MaxSize;


    // Start is called before the first frame update
    void Start()
    {
        MaxSize = (progressBar.transform.parent as RectTransform).rect.width;
        DateTime cd = DateTime.Now;
        txtSY.text = cd.Year.ToString();
        txtEY.text = cd.Year.ToString();
        txtSM.text = cd.Month.ToString();
        txtEM.text = cd.Month.ToString();
        txtSD.text = cd.Day.ToString();
        txtED.text = cd.Day.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        TickJobs();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void OnBtnClick_Close()
    {
        SetActive(false);
    }

    public void OnBtnClick_StartCollect()
    {
        int sy = int.Parse(txtSY.text);
        int sm = int.Parse(txtSM.text);
        int sd = int.Parse(txtSD.text);
        int ey = int.Parse(txtEY.text);
        int em = int.Parse(txtEM.text);
        int ed = int.Parse(txtED.text);

        console.text = "";
        //LotteryManager.Instance.CollectData(sy, sm, sd, ey, em, ed);
        DateTime startDate = new DateTime(sy, sm, sd);
        DateTime endDate = new DateTime(ey, em, ed);
        if (startDate == endDate)
            jobLst.Add(startDate);
        else
        {
            int diff = DateTime.Compare(startDate, endDate);
            DateTime curDate = diff < 0 ? startDate : endDate;
            while (DateTime.Compare(curDate, endDate) < 1)
            {
                jobLst.Add(curDate);
                curDate = curDate.AddDays(1);
            }
        }
        if (jobLst.Count > 0)
            curJobIndex = 0;

    }

    private void SetProgress(float p)
    {
        progressBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, p * MaxSize);
    }

    private void TickJobs()
    {
        if (jobLst.Count == 0 || curJobIndex == -1)
            return;
        DateTime date = jobLst[curJobIndex];
        ++curJobIndex;
        string error = "";
        int lotteryCount = AutoUpdateUtil.FetchData(date, ref error);
        SetProgress((float)curJobIndex / jobLst.Count);
        if (lotteryCount < 120)
        {
            jobUnFinishLst.Add(date);
            if (string.IsNullOrEmpty(error))
                console.text = date.ToString() + "--------------> " + lotteryCount + "\r\n" + console.text;
            else
                console.text = date.ToString() + "--------------> " + error + "\r\n" + console.text;
        }
        else
            console.text = date.ToString() + "\r\n" + console.text;
        if (jobLst.Count == curJobIndex)
        {
            console.text = "收集完毕!\r\n" + console.text;
            jobLst.Clear();
            curJobIndex = -1;
        }
    }

    public void AddConsoleText(string txt)
    {
        console.text = txt + "\n" + console.text;
    }
}
