using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDataView : MonoBehaviour
{
    static PanelDataView sInst;
    public static PanelDataView Instance
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

    public void OnBtnClickImportData()
    {

    }

    public void OnBtnClickClose()
    {
        gameObject.SetActive(false);
    }
}
