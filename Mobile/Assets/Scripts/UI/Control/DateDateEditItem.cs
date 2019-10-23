using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateDateEditItem : MonoBehaviour
{
    public Text label;
    public InputField inputField;

    public RectTransform rectTransform;

    public void SetData(DataItem item)
    {
        label.text = item.idTag;
        inputField.text = item.lotteryNumber;
    }

    private void Awake()
    {
        rectTransform = transform as RectTransform;
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
