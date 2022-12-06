using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public LineRenderer AsteroidLR;
    public GameObject AsteroidGO;

    List<Vector3> pointsList = new List<Vector3>();
    public List<Vector3> pointsPos = new List<Vector3>();

    Vector2 screenBoundriesHorizontal = new Vector2(-16, 16);
    Vector2 screenBoundriesVertical = new Vector2(-10, 10);

    const float tolerableRange = 8.0f;

    public float Radius;
    float Speed;
    int Segments;

    public Vector3 Pos;
    public Vector3 Dir;

    [System.Obsolete]
    public void CreateAsteroid()
    {
        this.Pos.x = Random.Range(screenBoundriesHorizontal.x - tolerableRange, screenBoundriesHorizontal.y + tolerableRange);
        this.Pos.y = Random.Range(screenBoundriesVertical.x - tolerableRange, screenBoundriesVertical.y + tolerableRange);

        while (this.Pos.x >= screenBoundriesHorizontal.x - 2 && this.Pos.x <= screenBoundriesHorizontal.y + 2 && this.Pos.y >= screenBoundriesVertical.x - 2 && this.Pos.y <= screenBoundriesVertical.y + 2)
        {
            this.Pos.x = Random.Range(screenBoundriesHorizontal.x - tolerableRange * 2, screenBoundriesHorizontal.y + tolerableRange * 2);
            this.Pos.y = Random.Range(screenBoundriesVertical.x - tolerableRange * 2, screenBoundriesVertical.y + tolerableRange * 2);
        }
            
        this.Dir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        this.Radius = Random.Range(1.0f, 4.0f);
        this.Speed = Random.Range(4.0f, 7.0f);
        this.Segments = (int)Random.Range(18, 36); 

        for(int i = 0; i < 361; i += Segments)
        {
            float offsetX = Random.Range(Radius / 3.0f, Radius / 3.0f);
            float offsetY = Random.Range(-0.5f, 0.5f);
            var rad = Mathf.Deg2Rad * (i * 360f / 360);
            pointsList.Add(new Vector3(Mathf.Sin(rad) * Radius + offsetX, Mathf.Cos(rad) * Radius + offsetY));
            pointsPos.Add(pointsList[pointsList.Count - 1] + Pos);
        }
        pointsList.Add(pointsList[0]);
        pointsPos.Add(pointsPos[0]);

        AsteroidLR.SetWidth(0.05f, 0.05f);
        AsteroidLR.SetVertexCount(pointsList.Count);
        AsteroidLR.enabled = true;

        for(int i = 0; i < pointsPos.Count; i++)
            AsteroidLR.SetPosition(i, pointsPos[i]);
    }

    [System.Obsolete]
    public void CreateAsteroid(float radius, Vector3 pos, Vector3 dir)
    {
        this.Pos = pos;
        this.Dir = dir;
        this.Radius = radius;
        this.Speed = Random.Range(3.0f, 5.0f);
        this.Segments = (int)Random.Range(18 * radius, 36 * radius);

        for (int i = 0; i < 361; i += Segments)
        {
            float offsetX = Random.Range(Radius / -5.0f, Radius / 5.0f);
            float offsetY = Random.Range(Radius / -5.0f, Radius / 5.0f);
            var rad = Mathf.Deg2Rad * (i * 360f / 360);
            pointsList.Add(new Vector3(Mathf.Sin(rad) * Radius + offsetX, Mathf.Cos(rad) * Radius + offsetY));
            pointsPos.Add(pointsList[pointsList.Count - 1] + Pos);
        }
        pointsList.Add(pointsList[0]);
        pointsPos.Add(pointsPos[0]);

        AsteroidLR.SetWidth(0.05f, 0.05f);
        AsteroidLR.SetVertexCount(pointsList.Count);
        AsteroidLR.enabled = true;

        for (int i = 0; i < pointsPos.Count; i++)
            AsteroidLR.SetPosition(i, pointsPos[i]);
    }

    void Update()
    {
        Move();
        CheckBoundries();
    }

    void Move()
    {
        Vector3 dir = Dir;
        Pos += dir * Speed * Time.deltaTime;

        for (int i = 0; i < pointsList.Count; i++)
            pointsPos[i] = Pos + pointsList[i];

        for (int i = 0; i < pointsList.Count; i++)
            AsteroidLR.SetPosition(i, pointsPos[i]);
    }

    void CheckBoundries()
    {
        if (Pos.x <= screenBoundriesHorizontal.x - tolerableRange)
            Pos.x = screenBoundriesHorizontal.y + tolerableRange;
        else if (Pos.x >= screenBoundriesHorizontal.y + tolerableRange)
            Pos.x = screenBoundriesHorizontal.x - tolerableRange;

        if (Pos.y <= screenBoundriesVertical.x - tolerableRange)
            Pos.y = screenBoundriesVertical.y + tolerableRange;
        else if (Pos.y >= screenBoundriesVertical.y + tolerableRange)
            Pos.y = screenBoundriesVertical.x - tolerableRange;
    }

    public void DestroyAsteroid()
    {
        Destroy(AsteroidGO);
    }
}
