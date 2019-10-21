using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelQuit : MonoBehaviour
{
    static PanelQuit sInst;
    public static PanelQuit Instance
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

    public void OnBtnClickQuit()
    {
        Application.Quit();
    }

    public void OnBtnClickCancel()
    {
        LotteryManager.SetActive(gameObject, false);
    }
}
