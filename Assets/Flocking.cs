using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    public int boidsToSpawn = 20;
    public float coh, sep, ali, personalSpace;
    public float vLimit;
    [SerializeField] Bounds bounds;
    [SerializeField] GameObject boidPrefab;
    List<GameObject> boids = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InitialiseBoids();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBoids();
    }

    void MoveBoids()
    {
        Vector2 v1, v2, v3, v4;
        
        foreach (GameObject b in boids)
        {
            Boid boid = b.GetComponent<Boid>();
            v1 = Cohesion(b) * coh;
            v2 = Seperation(b) * sep;
            v3 = Alignment(b) * ali;
            v4 = BoundPosition(b);

            boid.velocity += (v1 + v2 + v3 + v4);
            if (boid.velocity.magnitude > vLimit)
            {
                boid.velocity = (boid.velocity / boid.velocity.magnitude) * vLimit;
            }

            b.transform.position += (Vector3)boid.velocity * Time.deltaTime;
            float angle = Mathf.Atan2(boid.velocity.y, boid.velocity.x) * Mathf.Rad2Deg;
            b.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
    }

    Vector2 Cohesion(GameObject thisBoid)
    {
        Vector2 centre = Vector2.zero;

        foreach (GameObject boid in boids)
        {
            if (boid != thisBoid)
            {
                centre += (Vector2)boid.transform.position;
            }
        }
        
        centre = centre / (boids.Count - 1);
        return (centre - (Vector2)thisBoid.transform.position);
    }

    Vector2 Seperation(GameObject thisBoid)
    {
        Vector2 c = Vector2.zero;

        foreach (GameObject boid in boids)
        {
            if (boid != thisBoid)
            {
                if ((boid.transform.position - thisBoid.transform.position).magnitude < personalSpace)
                {
                    c -= (Vector2)(boid.transform.position - thisBoid.transform.position);
                }
            }
        }

        return c;
    }

    Vector2 Alignment(GameObject thisBoid)
    {
        Vector2 velocity = Vector2.zero;

        foreach (GameObject boid in boids)
        {
            if (boid != thisBoid)
            {
                velocity += boid.GetComponent<Boid>().velocity;
            }
        }

        velocity = velocity / (boids.Count - 1);

        return (velocity - thisBoid.GetComponent<Boid>().velocity) / 8;
    }

    void InitialiseBoids()
    {
        for (int i = 0; i < boidsToSpawn; i++)
        {
            Vector2 randomPos = new(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            GameObject newBoid = Instantiate(boidPrefab);
            newBoid.transform.position = randomPos;
            boids.Add(newBoid);
        }
    }

    Vector2 BoundPosition(GameObject b)
    {
        Vector2 v = Vector2.zero;

        if (b.transform.position.x < bounds.center.x - bounds.extents.x)
        {
            v.x = 10;
        }
        else if (b.transform.position.x > bounds.center.x + bounds.extents.x)
        {
            v.x = -10;
        }
        if (b.transform.position.y < bounds.center.y - bounds.extents.y)
        {
            v.y = 10;
        }
        else if (b.transform.position.y > bounds.center.y + bounds.extents.y)
        {
            v.y = -10;
        }
        return v;
    }
}
