using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSelectColor : MonoBehaviour
{
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    public Slider sliderA;
    public Image img;
    public Button btnOK;
    public Button btnCancel;

    public delegate void CallBackApplyColor(Color col);
    public CallBackApplyColor onApplyColor;

    static PanelSelectColor sInst;
    public static PanelSelectColor Instance
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
        sliderR.onValueChanged.AddListener((v) =>
        {
            img.color = new Color(sliderR.value, sliderG.value, sliderB.value, sliderA.value);
        });
        sliderG.onValueChanged.AddListener((v) =>
        {
            img.color = new Color(sliderR.value, sliderG.value, sliderB.value, sliderA.value);
        });
        sliderB.onValueChanged.AddListener((v) =>
        {
            img.color = new Color(sliderR.value, sliderG.value, sliderB.value, sliderA.value);
        });
        sliderA.onValueChanged.AddListener((v) =>
        {
            img.color = new Color(sliderR.value, sliderG.value, sliderB.value, sliderA.value);
        });
        btnOK.onClick.AddListener(() => 
        {
            gameObject.SetActive(false);
            if(onApplyColor != null)
            {
                onApplyColor.Invoke(img.color);
            }
            onApplyColor = null;
        });
        btnCancel.onClick.AddListener(() => {
            gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnOpen(Color col, CallBackApplyColor func)
    {
        onApplyColor += func;
        img.color = col;
        sliderA.value = col.a;
        sliderB.value = col.b;
        sliderG.value = col.g;
        sliderR.value = col.r;
    }
}
