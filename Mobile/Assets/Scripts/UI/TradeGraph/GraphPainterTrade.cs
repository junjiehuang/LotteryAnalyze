using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterTrade : GraphPainterBase
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void OnGraphDragging(Vector2 offset, Painter painter)
    {
        if (painter == upPainter)
        {
            canvasUpOffset += offset;
            canvasDownOffset.x = canvasUpOffset.x;
        }
        upPainter.BeforeDraw(canvasUpOffset);
        downPainter.BeforeDraw(canvasDownOffset);
        float v = 0;
        if (canvasUpOffset.x < 0)
            v = -canvasUpOffset.x / canvasUpScale.x;

        PanelTrade.Instance.uiTrade.sliderSelectTradeItem.value = v;
        PanelTrade.Instance.NotifyRepaint();

    }

    public override void OnPointerClick(Vector2 pos, Painter g)
    {

    }

    public override void OnPointerDown(Vector2 pos, Painter g)
    {
        base.OnPointerDown(pos, g);
    }

    public override void OnPointerUp(Vector2 pos, Painter g)
    {
        base.OnPointerUp(pos, g);
    }

    public void OnGraphZoom(Painter g)
    {

    }

    public override void OnScrollToData(float dataIndex)
    {

    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {

    }
}
