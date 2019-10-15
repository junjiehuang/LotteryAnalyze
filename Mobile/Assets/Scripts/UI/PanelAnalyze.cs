using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnalyze : MonoBehaviour
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBtnClickRefreshData()
    {

    }

    public void OnBtnClickClose()
    {
        gameObject.SetActive(false);
    }
}
