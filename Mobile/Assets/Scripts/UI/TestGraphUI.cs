using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGraphUI : UnityEngine.UI.Graphic
{
    List<UIVertex> verts = new List<UIVertex>();
    List<int> tris = new List<int>();


    private void Update()
    {
        verts.Clear();
        tris.Clear();

        DrawRect(0, 0, 100, 100, Color.red);
        DrawRect(Screen.width - 100, 0, 100, 100, Color.green);
        DrawLine(120, 120, 300, 300, Color.green);
    }

    void DrawRect(float x, float y, float w, float h, Color c)
    {
        UIVertex p0 = new UIVertex(), p1 = new UIVertex(), p2 = new UIVertex(), p3 = new UIVertex();
        p0.position = new Vector3(x, y, 0);
        p0.color = c;
        p1.position = new Vector3(x + w, y, 0);
        p1.color = c;
        p2.position = new Vector3(x, y - h, 0);
        p2.color = c;
        p3.position = new Vector3(x + w, y - h, 0);
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

    void DrawLine(float x1, float y1, float x2, float y2, Color c)
    {
        UIVertex p0 = new UIVertex(), p1 = new UIVertex(), p2 = new UIVertex(), p3 = new UIVertex();
        Vector3 v0 = new Vector3(x1, y1, 0), v1 = new Vector3(x2, y2, 0);
        Vector3 dir = (v1 - v0).normalized;
        Vector3 fow = Vector3.Cross(dir, new Vector3(0, 0, 1));

        p0.position = v0 + fow; p0.color = c;
        p1.position = v1 + fow; p1.color = c;
        p2.position = v0 - fow; p2.color = c;
        p3.position = v1 - fow; p3.color = c;

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

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (verts.Count == 0)
            return;
        vh.Clear();
        vh.AddUIVertexStream(verts, tris);
    }

}
