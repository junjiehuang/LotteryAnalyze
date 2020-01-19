using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LotteryAnalyze;
using System;

public class LotteryManager : MonoBehaviour
{
    static LotteryManager sInstance = null;

    public static LotteryManager Instance
    {
        get { return sInstance; }
    }

    private void Awake()
    {
        sInstance = this;

        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetActive(PanelMain.Instance.gameObject, true);

        SetActive(PanelCollectData.Instance.gameObject, false);
        SetActive(PanelGlobalSetting.Instance.gameObject, false);
        SetActive(PanelAnalyze.Instance.gameObject, false);
        SetActive(PanelDataView.Instance.gameObject, false);
        SetActive(PanelQuit.Instance.gameObject, false);
        SetActive(PanelCalculator.Instance.gameObject, false);
        SetActive(PanelSelectColor.Instance.gameObject, false);
        SetActive(PanelStatisticCollect.Instance.gameObject, false);
        SetActive(PanelMessageBox.Instance.gameObject, false);
        SetActive(PanelTrade.Instance.gameObject, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetActive(PanelQuit.Instance.gameObject, true);
        }

        GlobalSetting.SaveCfg();
    }

    private void OnDestroy()
    {
        if(LOG_STREAM_WRITER != null)
        {
            LOG_STREAM_WRITER.Close();
            LOG_STREAM_WRITER.Dispose();
        }
    }

    void OnLog(string condition, string stackTrace, LogType type)
    {
        if(LOG_FI == null || LOG_STREAM_WRITER == null)
        {
            LOG_FI = new FileInfo(LOG_PATH);
            try
            {
                if (File.Exists(LOG_PATH))
                {
                    LOG_STREAM_WRITER = LOG_FI.CreateText();
                }
                else
                {
                    LOG_STREAM_WRITER = new StreamWriter(LOG_PATH);
                }
            }
            catch(Exception e)
            {
                Console.Write("Create Log file StreamWriter failed! - " + e.ToString());
            }
        }
        if (LOG_STREAM_WRITER != null)
        {
            string msg = Time.time + " - " + type.ToString() + " : " + condition + "\nStack : \n" + stackTrace + "\n";
            LOG_STREAM_WRITER.Write(msg);
            LOG_STREAM_WRITER.Flush();
        }
    }

    string LOG_PATH = "";
    StreamWriter LOG_STREAM_WRITER;
    FileInfo LOG_FI;


    private void Init()
    {
        LotteryAnalyze.AutoUpdateUtil.sCallBackOnCollecting += CallBack_CollectResult;

        //        string persistentDataPath = Application.persistentDataPath;
        //#if UNITY_EDITOR_WIN || UNITY_EDITOR_WIN
        //        LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER = Application.persistentDataPath + "/LotteryAnalyze";
        //#elif UNITY_ANDROID
        //        LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER = "/mnt/sdcard/LotteryAnalyze";
        //#endif
        //        LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER = Application.persistentDataPath + "/LotteryAnalyze";
        LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER = GlobalSetting.ROOT_FOLDER;

        Debug.Log(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER);
        if (!Directory.Exists(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER))
            Directory.CreateDirectory(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER);

        LOG_PATH = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "/log.txt";
        LOG_PATH = LOG_PATH.Replace('\\', '/');
        Application.logMessageReceived += OnLog;
        if(File.Exists(LOG_PATH))
            File.Delete(LOG_PATH);
        Debug.Log("Create Log File : " + LOG_PATH);

        LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER += "/Data/";
        if (!Directory.Exists(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER))
            Directory.CreateDirectory(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER);
        Debug.Log("Create Data Folder : " + LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER);

        LotteryAnalyze.GlobalSetting.ReadCfg();
    }


    public void CollectData(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
    {
        LotteryAnalyze.AutoUpdateUtil.FetchDatas(startYear, startMonth, startDay, endYear, endMonth, endDay);
    }

    public static void SetActive(GameObject go, bool active)
    {
        if(go != null)
        {
            Debug.Log("set " + go.name + " active = " + active);
            go.SetActive(active);
            if (active)
            {
                go.transform.SetSiblingIndex(go.transform.parent.childCount - 1);
            }
        }
    }

    public static void CallBack_CollectResult(string info)
    {
        Debug.LogWarning(info);
    }
}
