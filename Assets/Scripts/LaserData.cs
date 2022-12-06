using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LaserData : MonoBehaviour
{
    public GameObject laserGO;
    public new LineRenderer renderer;

    const float bulletSpeed = 50.0f;

    Vector3[] points = new Vector3[2];

    Vector2 screenBoundriesHorizontal = new Vector2(-16, 16);
    Vector2 screenBoundriesVertical = new Vector2(-10, 10);

    public Vector3 Pos;
    public Vector3 Dir;

    [System.Obsolete]
    public void SetValues(Vector2 pos, Vector2 dir)
    {
        this.Pos = pos;
        this.Dir = dir;

        points[0] = new Vector3(pos.x, pos.y);
        points[1] = new Vector3(pos.x + dir.x, pos.y + dir.y);

        renderer.SetWidth(0.07f, 0.07f);
        renderer.SetVertexCount(2);
        renderer.enabled = true;
        renderer.SetPositions(points);
    }

    public Vector2 GetPos()
    {
        return this.Pos;
    }

    public Vector2 GetDir()
    {
        return this.Dir;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Pos.x += Dir.x * bulletSpeed * Time.deltaTime;
        Pos.y += Dir.y * bulletSpeed * Time.deltaTime;

        points[0] = new Vector3(Pos.x, Pos.y);
        points[1] = new Vector3(Pos.x + Dir.x, Pos.y + Dir.y);

        renderer.SetPositions(points);
    }

    public void DestroyBullet()
    {
        Destroy(laserGO);
    }
}
