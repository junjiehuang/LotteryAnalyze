using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCollectData : MonoBehaviour
{
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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        LotteryManager.Instance.CollectData(sy, sm, sd, ey, em, ed);
    }
}
