using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetActive(PanelQuit.Instance.gameObject, true);
        }
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
        if(LOG_FI == null)
        {
            LOG_FI = new FileInfo(LOG_PATH);
            LOG_STREAM_WRITER = LOG_FI.CreateText();
        }
        string msg = Time.time + " - " + type.ToString() + " : " + condition + "\nStack : \n" + stackTrace + "\n";
        LOG_STREAM_WRITER.Write(msg);
        LOG_STREAM_WRITER.Flush();
    }

    string LOG_PATH = "";
    StreamWriter LOG_STREAM_WRITER;
    FileInfo LOG_FI;


    private void Init()
    {
        LotteryAnalyze.AutoUpdateUtil.sCallBackOnCollecting += CallBack_CollectResult;

#if UNITY_EDITOR_WIN || UNITY_EDITOR_WIN
        LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER = Application.persistentDataPath + "/LotteryAnalyze";
#elif UNITY_ANDROID
        LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER = "/mnt/sdcard/LotteryAnalyze";
#endif

        Debug.Log(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER);
        if (!Directory.Exists(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER))
            Directory.CreateDirectory(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER);

        Application.logMessageReceived += OnLog;
        LOG_PATH = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "/log.txt";
        LOG_PATH = LOG_PATH.Replace('\\', '/');
        File.Delete(LOG_PATH);

        LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER += "/Data/";
        if (!Directory.Exists(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER))
            Directory.CreateDirectory(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER);
        
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
