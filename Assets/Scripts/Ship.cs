using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Ship : MonoBehaviour
{
    bool Game;

    const float rotationSpeed = 5.0f;
    const float movementSpeed = 12.0f;
    const float acceleration = 10.0f;
    const float deceleration = 4.0f;
    const float kickBack = 10.0f;
    const int numberOfVertices = 4;
    const float tolerableBoundriesLimit = 1.0f;
    const int maxAmountOfAsteroids = 7;
    const float spawnTime = 3f;
    const float spawnBulletTime = 0.35f;

    public Text PressEnter;
    public GameObject GameOverScore;
    public GameObject HighScoreText;
    public GameObject GameOverText;
    public GameObject ScoreGO;
    public GameObject AsteroidGO;
    public GameObject Laser;
    public LineRenderer lines;

    public AudioSource LaserShot;
    public AudioSource ShipCrash;

    List<GameObject> bullets = new List<GameObject>();
    List<GameObject> asteroids = new List<GameObject>();

    Vector2 topMinSpeed = new Vector2(movementSpeed, 0.0f);
        
    Vector3[] points = new Vector3[numberOfVertices];

    Vector2 screenBoundriesHorizontal = new Vector2(-16, 16);
    Vector2 screenBoundriesVertical = new Vector2(-10, 10);

    float width;
    float height;

    Vector2 A;
    Vector2 B;
    Vector2 C;

    float rotationValue;

    public Vector2 position = new Vector2();

    bool toMove;
    bool toRotate;
    bool toShoot;
    int turnTo;

    int amountOfAsteroids;

    bool playedCrash;

    float time;
    float bulletTime;

    class DirectionVector
    {
        public Vector2 Dir { get; set; }
        public float PosChange { get; set; }

        public DirectionVector(Vector2 dir)
        {
            this.Dir = dir;
            this.PosChange = 0.0f;
        }

        public override bool Equals(object other)
        {
            return this.Dir == ((DirectionVector)other).Dir;
        }
        public override int GetHashCode()
        {
            return this.Dir.GetHashCode();
        }
    }

    List<DirectionVector> dirVector = new List<DirectionVector>();

    [System.Obsolete]
    void Start()
    {
        Game = true;
        playedCrash = false;
        PressEnter.enabled = false;

        lines.enabled = true;
        lines.SetWidth(0.05f, 0.05f);
        lines.SetVertexCount(numberOfVertices);

        width = 0.75f;
        height = 1.0f;

        position.x = 0;
        position.y = 0;

        A = new Vector2(0, height);
        B = new Vector2(width, -height);
        C = new Vector2(-width, -height);

        points[0] = points[3] = new Vector3(A.x, A.y);
        points[1] = new Vector3(B.x, B.y);
        points[2] = new Vector3(C.x, C.y);

        rotationValue = 0.0f;

        toMove = false;
        toRotate = false;
        toShoot = false;
        turnTo = 0;

        lines.SetPositions(points);

        time = 0.0f;
        bulletTime = 0.0f;

        while (amountOfAsteroids <= maxAmountOfAsteroids)
        {
            GameObject asteroid = Instantiate(AsteroidGO);
            asteroid.GetComponent<Asteroid>().CreateAsteroid();
            asteroids.Add(asteroid);
            amountOfAsteroids++;
        }
    }

    [Obsolete]
    void Update()
    {
        if (Game)
        {
            DirectionVector current = new DirectionVector(A);

            time += Time.deltaTime;
            bulletTime += Time.deltaTime;

            if (time >= spawnTime)
            {
                GameObject asteroid = Instantiate(AsteroidGO);
                asteroid.GetComponent<Asteroid>().CreateAsteroid();
                asteroids.Add(asteroid);
                time = 0.0f;
            }

            if (toShoot)
            {
                if (bulletTime >= spawnBulletTime)
                {
                    GameObject bullet = Instantiate(Laser);
                    bullet.GetComponent<LaserData>().SetValues(position + A, A);
                    bullets.Add(bullet);
                    LaserShot.Play();
                    bulletTime = 0.0f;
                }
            }
            CheckLaserCollision();
            CheckLaserBoundries();

            if (!dirVector.Contains(current))
                dirVector.Add(current);

            //CheckShipCollision();
            CheckForCollision();
            CheckInput();
            CheckBoundries();

            if (toMove)
            {
                Accelerate();
            }
            Decelerate(current);

            if (toRotate)
                UpdateRotation();

            ApplyAllForces();
        }

        else if (!Game)
        {
            GameOverText.GetComponent<GameOver>().WriteGameOverText();
            HighScoreText.GetComponent<HighScore>().CheckHighScore();
            HighScoreText.GetComponent<HighScore>().WriteHighScoreText();
            GameOverScore.GetComponent<GameOverScore>().WriteScoreText();

            PressEnter.enabled = true;

            ScoreGO.GetComponent<Score>().scoreText.enabled = false;

            if (!playedCrash)
            {
                time = 0;
                ShipCrash.Play();
                playedCrash = true;
            }

            time += Time.deltaTime;

            if (time >= 1.0f)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Application.Quit();
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ResetGame();
                }
            }
        }
    }

    [Obsolete]
    void ResetGame()
    {
        while(asteroids.Count != 0)
        {
            asteroids[0].GetComponent<Asteroid>().DestroyAsteroid();
            asteroids.Remove(asteroids[0]);
        }

        while (bullets.Count != 0)
        {
            bullets[0].GetComponent<LaserData>().DestroyBullet();
            bullets.Remove(bullets[0]);
        }

        while (dirVector.Count != 0)
        {
            dirVector.Remove(dirVector[0]);
        }

        Game = true;
        playedCrash = false;

        width = 0.75f;
        height = 1.0f;

        position.x = 0;
        position.y = 0;

        A = new Vector2(0, height);
        B = new Vector2(width, -height);
        C = new Vector2(-width, -height);

        points[0] = points[3] = new Vector3(A.x, A.y);
        points[1] = new Vector3(B.x, B.y);
        points[2] = new Vector3(C.x, C.y);

        rotationValue = 0.0f;

        toMove = false;
        toRotate = false;
        toShoot = false;
        turnTo = 0;

        lines.SetPositions(points);

        time = 0.0f;
        bulletTime = 0.0f;

        amountOfAsteroids = 0;

        while (amountOfAsteroids <= maxAmountOfAsteroids)
        {
            GameObject asteroid = Instantiate(AsteroidGO);
            asteroid.GetComponent<Asteroid>().CreateAsteroid();
            asteroids.Add(asteroid);
            amountOfAsteroids++;
        }

        GameOverText.GetComponent<GameOver>().gameOverText.enabled = false;
        HighScoreText.GetComponent<HighScore>().highScoreText.enabled = false;
        GameOverScore.GetComponent<GameOverScore>().gameOverScore.enabled = false;
        PressEnter.enabled = false;

        ScoreGO.GetComponent<Score>().scoreText.enabled = true;
        ScoreGO.GetComponent<Score>().score = 0;
        ScoreGO.GetComponent<Score>().WriteScore();
    }

    void CheckInput()
    {
        toMove = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? true : false;
        toRotate = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? true : false;
        turnTo = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? -1 : Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
        toShoot = Input.GetKeyDown(KeyCode.Space) ? true : false;
    }

    void ApplyAllForces()
    {
        for (int i = 0; i < dirVector.Count; i++)
        {
            position += dirVector[i].PosChange * dirVector[i].Dir * Time.deltaTime;
        }

        SetShipPointsCoordinates();
        lines.SetPositions(points);
    }

    void Accelerate()
    {
        dirVector[dirVector.Count - 1].PosChange += acceleration * Time.deltaTime;
        dirVector[dirVector.Count - 1].PosChange = Clamp(dirVector[dirVector.Count - 1].PosChange, topMinSpeed);
    }

    void Decelerate(DirectionVector current)
    {
        for (int i = 0; i < dirVector.Count; i++)
        {
            if (dirVector[i].Dir != current.Dir && toMove)
            {
                dirVector[i].PosChange -= deceleration * Time.deltaTime;
                dirVector[i].PosChange = Clamp(dirVector[i].PosChange, topMinSpeed);
            }
            else if (!toMove)
            {
                dirVector[i].PosChange -= deceleration * Time.deltaTime;
                dirVector[i].PosChange = Clamp(dirVector[i].PosChange, topMinSpeed);
            }

            if (dirVector[i].PosChange <= 0.01f)
            {
                dirVector.Remove(dirVector[i]);
            }
        }
    }

    float Clamp(float valueToClamp, Vector2 topMinSpeed)
    {
        if (valueToClamp > topMinSpeed.x)
            return topMinSpeed.x;
        else if (valueToClamp < topMinSpeed.y)
            return topMinSpeed.y;
        return valueToClamp;
    }

    void UpdateRotation()
    {
        rotationValue = turnTo * rotationSpeed * Time.deltaTime;

        A = RotationAnges(A.x, A.y);
        B = RotationAnges(B.x, B.y);
        C = RotationAnges(C.x, C.y);

        SetShipPointsCoordinates();

        lines.SetPositions(points);
    }

    Vector2 RotationAnges(float x, float y)
    {
        Vector2 newPos = new Vector2();

        newPos.x = x * Mathf.Cos(rotationValue) - y * Mathf.Sin(rotationValue);
        newPos.y = y * Mathf.Cos(rotationValue) + x * Mathf.Sin(rotationValue);

        return newPos;
    }

    void CheckBoundries()
    {
        if (position.x <= screenBoundriesHorizontal.x - tolerableBoundriesLimit)
        {
            position.x = screenBoundriesHorizontal.y + tolerableBoundriesLimit;
            SetShipPointsCoordinates();
        }
        else if (position.x >= screenBoundriesHorizontal.y + tolerableBoundriesLimit)
        {
            position.x = screenBoundriesHorizontal.x - tolerableBoundriesLimit;
            SetShipPointsCoordinates();
        }

        if (position.y <= screenBoundriesVertical.x - tolerableBoundriesLimit)
        {
            position.y = screenBoundriesVertical.y + tolerableBoundriesLimit;
            SetShipPointsCoordinates();
        }
        else if (position.y >= screenBoundriesVertical.y + tolerableBoundriesLimit)
        {
            position.y = screenBoundriesVertical.x - tolerableBoundriesLimit;
            SetShipPointsCoordinates();
        }
    }

    void SetShipPointsCoordinates()
    {
        points[0] = points[3] = A + position;
        points[1] = B + position;
        points[2] = C + position;
    }

    [Obsolete]
    void CheckLaserCollision()
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            Vector3 bulletPos = bullets[i].GetComponent<LaserData>().Pos;
            for (int j = 0; j < asteroids.Count; j++)
            {
                float asteroidRadius = asteroids[j].GetComponent<Asteroid>().Radius;
                Vector3 asteroidPos = asteroids[j].GetComponent<Asteroid>().Pos;
                Vector3 asteroidDir = asteroids[j].GetComponent<Asteroid>().Dir;
                if (bulletPos.x >= asteroidPos.x - asteroidRadius && bulletPos.x <= asteroidPos.x + asteroidRadius)
                {
                    if (bulletPos.y >= asteroidPos.y - asteroidRadius && bulletPos.y <= asteroidPos.y + asteroidRadius)
                    {
                        ScoreGO.GetComponent<Score>().score += (int)(4 / asteroidRadius);
                        ScoreGO.GetComponent<Score>().WriteScore();

                        asteroids[j].GetComponent<Asteroid>().DestroyAsteroid();
                        asteroids.Remove(asteroids[j]);

                        bullets[i].GetComponent<LaserData>().DestroyBullet();
                        bullets.Remove(bullets[i]);

                        if (asteroidRadius >= 1.2f)
                        {
                            for (int n = -1; n < 2; n+=2)
                            {
                                Vector3 offsetPos = new Vector3(0.5f, 0.5f);
                                Vector3 offsetDir = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-0.6f, 0.6f));

                                Vector3 newdDir = new Vector3(asteroidDir.x + offsetDir.x, asteroidDir.y + offsetDir.y, asteroidDir.z);

                                GameObject asteroid = Instantiate(AsteroidGO);
                                asteroid.GetComponent<Asteroid>().CreateAsteroid(asteroidRadius / 2.0f, asteroidPos + (n * offsetPos), newdDir);
                                asteroids.Add(asteroid);
                            }
                        }

                        break;
                    }
                }
            }
        }
    }

    void CheckLaserBoundries()
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            Vector3 bulletPos = bullets[i].GetComponent<LaserData>().Pos;
            if (bulletPos.x <= screenBoundriesHorizontal.x || bulletPos.x >= screenBoundriesHorizontal.y)
            {
                bullets[i].GetComponent<LaserData>().DestroyBullet();
                bullets.Remove(bullets[i]);
                break;
            }

            if (bulletPos.y <= screenBoundriesVertical.x || bulletPos.y >= screenBoundriesVertical.y)
            {
                bullets[i].GetComponent<LaserData>().DestroyBullet();
                bullets.Remove(bullets[i]);
                break;
            }
        }
    }

    // Square Collision
    void CheckShipCollision()
    {
        for (int i = 0; i < asteroids.Count; i++)
        {
            float asteroidRadius = asteroids[i].GetComponent<Asteroid>().Radius;
            Vector3 asteroidPos = asteroids[i].GetComponent<Asteroid>().Pos;

            if (position.x + A.x >= asteroidPos.x - asteroidRadius && position.x + A.x <= asteroidPos.x + asteroidRadius)
            {
                if (position.y + A.y >= asteroidPos.y - asteroidRadius && position.y + A.y <= asteroidPos.y + asteroidRadius)
                {
                    Game = false;
                }
            }

            else if (position.x + B.x >= asteroidPos.x - asteroidRadius && position.x + B.x <= asteroidPos.x + asteroidRadius)
            {
                if (position.y + B.y >= asteroidPos.y - asteroidRadius && position.y + B.y <= asteroidPos.y + asteroidRadius)
                {
                    Game = false;
                }
            }

            else if (position.x + C.x >= asteroidPos.x - asteroidRadius && position.x + C.x <= asteroidPos.x + asteroidRadius)
            {
                if (position.y + C.y >= asteroidPos.y - asteroidRadius && position.y + C.y <= asteroidPos.y + asteroidRadius)
                {
                    Game = false;
                }
            }
        }
    }

    // Line-line intersection
    void CheckForCollision()
    {
        foreach (GameObject asteroid in asteroids)
        {
            Vector3 asteroidPos = asteroid.GetComponent<Asteroid>().Pos;
            double distance = Math.Sqrt(Math.Abs(position.x - asteroidPos.x) + Math.Abs(position.y - asteroidPos.y));

            if (distance <= 3.0f)
            {
                List<Vector3> asteroidPoints = asteroid.GetComponent<Asteroid>().pointsPos;
                for (int i = 0; i < asteroidPoints.Count - 1; i++)
                {
                    for (int n = 0; n < numberOfVertices - 1; n++)
                    {
                        float x1 = asteroidPoints[i].x;
                        float y1 = asteroidPoints[i].y;
                        float x2 = asteroidPoints[i + 1].x;
                        float y2 = asteroidPoints[i + 1].y;

                        float x3 = points[n].x;
                        float y3 = points[n].y;
                        float x4 = points[n + 1].x;
                        float y4 = points[n + 1].y;

                        float den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4) * 1.0f;

                        if (den != 0.0f)
                        {
                            float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den * 1.0f;
                            float u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den * 1.0f;

                            if (t > 0.0f && t <= 1.0f && u > 0.0f && u <= 1.0f)
                            {
                                Game = false;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}

