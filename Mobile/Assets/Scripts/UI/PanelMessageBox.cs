using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMessageBox : MonoBehaviour
{
    public Button btnOK;
    public Button btnCancel;
    public Text txtInfo;

    public delegate void CallBack();

    CallBack funcOK;
    CallBack funcCancel;

    static PanelMessageBox sInst;
    public static PanelMessageBox Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;

        btnOK.onClick.AddListener(() =>
        {
            LotteryManager.SetActive(gameObject, false);
            if(funcOK != null)
            {
                funcOK.Invoke();
            }
            funcOK = null;
            funcCancel = null;
        });

        btnCancel.onClick.AddListener(() =>
        {
            LotteryManager.SetActive(gameObject, false);
            if (funcCancel != null)
            {
                funcCancel.Invoke();
            }
            funcOK = null;
            funcCancel = null;
        });
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(string info, CallBack ok, CallBack cancel)
    {
        txtInfo.text = info;
        funcOK += ok;
        funcCancel += cancel;
        LotteryManager.SetActive(gameObject, true);
    }

}
