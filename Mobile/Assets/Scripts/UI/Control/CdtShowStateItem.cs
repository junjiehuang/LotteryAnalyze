using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CdtShowStateItem : ListViewItem
{
    public Text label;
    public Toggle toggleShow;
    public Image imgColor;

    protected override void Awake()
    {
        label = transform.Find("Text").gameObject.GetComponent<Text>();
        toggleShow = transform.Find("Toggle").gameObject.GetComponent<Toggle>();
        imgColor = transform.Find("Image").gameObject.GetComponent<Image>();
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
