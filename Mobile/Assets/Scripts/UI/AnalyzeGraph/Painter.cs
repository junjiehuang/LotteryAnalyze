using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter
{
    string name = "Painter";
    Vector2 offset = Vector2.zero;
    List<UIVertex> verts = new List<UIVertex>();
    List<int> tris = new List<int>();

    public string Name
    {
        get { return name; }
    }

    public List<UIVertex> Verts
    {
        get { return verts; }
    }

    public List<int> Tris
    {
        get { return tris; }
    }

    public Painter(string _name = "Painter")
    {
        name = _name;
    }



    public void BeforeDraw(Vector2 _offset)
    {
        verts.Clear();
        tris.Clear();
        offset = _offset;
    }

    public void DrawRect(float x, float y, float w, float h, Color c, float lineWidth = 1)
    {
        DrawLine(x, y, x + w, y, c, lineWidth);
        DrawLine(x + w, y, x + w, y + h, c, lineWidth);
        DrawLine(x, y + h, x + w, y + h, c, lineWidth);
        DrawLine(x, y, x, y + h, c, lineWidth);
    }

    public void DrawFillRect(float x, float y, float w, float h, Color c)
    {
        x += offset.x;
        y += offset.y;
        DrawFillRectInCanvasSpace(x, y, w, h, c);
    }

    public void DrawLine(float x1, float y1, float x2, float y2, Color c, float lineWidth = 1)
    {
        x1 += offset.x;
        y1 += offset.y;
        x2 += offset.x;
        y2 += offset.y;
        DrawLineInCanvasSpace(x1, y1, x2, y2, c, lineWidth);
    }

    public void DrawRectInCanvasSpace(float x, float y, float w, float h, Color c, float lineWidth = 1)
    {
        DrawLineInCanvasSpace(x, y, x + w, y, c, lineWidth);
        DrawLineInCanvasSpace(x + w, y, x + w, y + h, c, lineWidth);
        DrawLineInCanvasSpace(x, y + h, x + w, y + h, c, lineWidth);
        DrawLineInCanvasSpace(x, y, x, y + h, c, lineWidth);
    }

    public void DrawFillRectInCanvasSpace(float x, float y, float w, float h, Color c)
    {
        UIVertex p0 = new UIVertex(), p1 = new UIVertex(), p2 = new UIVertex(), p3 = new UIVertex();
        p0.position = new Vector3(x, y, 0);
        p0.color = c;
        p1.position = new Vector3(x + w, y, 0);
        p1.color = c;
        p2.position = new Vector3(x, y + h, 0);
        p2.color = c;
        p3.position = new Vector3(x + w, y + h, 0);
        p3.color = c;

        int id = verts.Count;
        verts.Add(p0);
        verts.Add(p1);
        verts.Add(p2);
        verts.Add(p3);
        tris.Add(id);
        tris.Add(id + 1);
        tris.Add(id + 2);
        tris.Add(id + 2);
        tris.Add(id + 1);
        tris.Add(id + 3);
    }

    public void DrawLineInCanvasSpace(float x1, float y1, float x2, float y2, Color c, float lineWidth = 1)
    {
        UIVertex p0 = new UIVertex(), p1 = new UIVertex(), p2 = new UIVertex(), p3 = new UIVertex();
        Vector3 v0 = new Vector3(x1, y1, 0), v1 = new Vector3(x2, y2, 0);
        Vector3 dir = (v1 - v0).normalized;
        Vector3 fow = Vector3.Cross(dir, new Vector3(0, 0, 1));

        Vector3 ext = fow * lineWidth * 0.5f;
        p0.position = v0 + ext; p0.color = c;
        p1.position = v1 + ext; p1.color = c;
        p2.position = v0 - ext; p2.color = c;
        p3.position = v1 - ext; p3.color = c;

        int id = verts.Count;
        verts.Add(p0);
        verts.Add(p1);
        verts.Add(p2);
        verts.Add(p3);
        tris.Add(id);
        tris.Add(id + 1);
        tris.Add(id + 2);
        tris.Add(id + 2);
        tris.Add(id + 1);
        tris.Add(id + 3);
    }

    public float StandToCanvas(float v, bool isX)
    {
        if (isX)
            return v + offset.x;
        else
            return v + offset.y;
    }
    public float CanvasToStand(float v, bool isX)
    {
        if (isX)
            return v - offset.x;
        else
            return v - offset.y;
    }

}
