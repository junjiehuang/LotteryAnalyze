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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
            go.SetActive(active);
        }
    }

    public static void CallBack_CollectResult(string info)
    {
        Debug.LogWarning(info);
    }
}
