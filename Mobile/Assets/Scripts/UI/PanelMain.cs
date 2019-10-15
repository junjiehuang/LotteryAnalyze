using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelMain : MonoBehaviour
{
    static PanelMain sInst;
    public static PanelMain Instance
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBtnClickCollectData()
    {
        LotteryManager.SetActive(PanelCollectData.Instance.gameObject, true);
    }
    public void OnBtnClickGlobalSetting()
    {
        LotteryManager.SetActive(PanelGlobalSetting.Instance.gameObject, true);
    }
}
