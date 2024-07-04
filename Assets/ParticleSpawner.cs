using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;

public class ParticleSpawner : MonoBehaviour
{
    public Material lineMaterial;
    public Particle Prefab;
    private LineRenderer lineRenderer;

    public int NumOfParticles = 1;
    public float SizeOfParticles = 1;
    public float SpacingOfParticles = 1;

    public float CollisionDamper = 1;
    public float Gravity = 10;

    public float BorderWidth = 30;
    public float BorderHeight = 14;

    private float LeftBound;
    private float RightBound;
    private float LowerBound;
    private float UpperBound;


    Particle[] Particles;
    Vector3[] ParticlePositions;
    Vector3[] ParticleVelocities;
    Vector3[] ParticleForces;

    private void Start()
    {

        DrawBorder();

        Particles = new Particle[NumOfParticles];
        ParticlePositions = new Vector3[NumOfParticles];
        ParticleVelocities = new Vector3[NumOfParticles];
        ParticleForces = new Vector3[NumOfParticles];

        int particlesInRow = (int)math.sqrt(NumOfParticles);
        int particlesInCol = (NumOfParticles - 1) / particlesInRow + 1;
        float spacing = SizeOfParticles * (1 + SpacingOfParticles);

        for (int i = 0; i < NumOfParticles; i++)
        {
            float x = (i % particlesInRow - particlesInRow / 2f + 0.5f) * spacing + (Screen.width / 2);
            float y = (i / particlesInRow - particlesInCol / 2f + 0.5f) * spacing + (Screen.height / 2);
            ParticlePositions[i] = new Vector3(x, y, 0);
            Particles[i] = SpawnParticle(ParticlePositions[i], SizeOfParticles / 2);
        }
    }

    private void Update()
    {
        for (int i = 0; i < NumOfParticles; i++)
        {
            ResolveCollision(i);
            CalculateForces(i);
            ParticleVelocities[i] += ParticleForces[i] * Time.deltaTime;
            Particles[i].transform.position += ParticleVelocities[i] * Time.deltaTime;
        }
    }

    private Particle SpawnParticle(Vector3 position, float radius)
    {
        Particle particleToSpawn = Instantiate(
            Prefab,
            position,
            Quaternion.identity,
            transform
        );
        particleToSpawn.transform.localScale = new Vector3(radius, radius, 1);

        return particleToSpawn;
    }

    private void ResolveCollision(int index)
    {
        Vector3 currentPosition = Particles[index].transform.position;
        if (currentPosition.x - SizeOfParticles / 2f < LeftBound)
        {
            Particles[index].transform.position.Set(LeftBound + SizeOfParticles / 2f, currentPosition.y, 0);
            ParticleVelocities[index].x *= -1 * CollisionDamper;
        }
        else if (currentPosition.x + SizeOfParticles / 2f > RightBound)
        {
            Particles[index].transform.position.Set(RightBound - SizeOfParticles / 2f, currentPosition.y, 0);
            ParticleVelocities[index].x *= -1 * CollisionDamper;
        }

        if (currentPosition.y - SizeOfParticles / 4f < LowerBound)
        {
            Particles[index].transform.position.Set(currentPosition.x, LowerBound + SizeOfParticles / 2f, 0);
            ParticleVelocities[index].y *= -1 * CollisionDamper;
        }
        else if (currentPosition.y + SizeOfParticles / 2f > UpperBound)
        {
            Particles[index].transform.position.Set(currentPosition.x, UpperBound - SizeOfParticles / 2f, 0);
            ParticleVelocities[index].y *= -1 * CollisionDamper;
        }
    }

    private void CalculateForces(int index)
    {

    }

    private void DrawBorder()
    {
        
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        if (lineMaterial == null)
        {
            Debug.LogError("Line Material is not assigned. Please assign it in the Inspector.");
            return;
        }

        lineRenderer.positionCount = 5;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        LeftBound = (Screen.width / 2) - (BorderWidth / 2);
        RightBound = (Screen.width / 2) + (BorderWidth / 2);
        UpperBound = (Screen.height / 2) + (BorderHeight / 2);
        LowerBound = (Screen.height / 2) - (BorderHeight / 2);

        // Define corners of the box
        Vector3 topLeft = new Vector3(LeftBound, UpperBound, 0);
        Vector3 topRight = new Vector3(RightBound, UpperBound, 0);
        Vector3 bottomRight = new Vector3(RightBound, LowerBound, 0);
        Vector3 bottomLeft = new Vector3(LeftBound, LowerBound, 0);

        lineRenderer.SetPosition(0, topLeft);
        lineRenderer.SetPosition(1, topRight);
        lineRenderer.SetPosition(2, bottomRight);
        lineRenderer.SetPosition(3, bottomLeft);
        lineRenderer.SetPosition(4, topLeft);
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Mathematics;

//public class ParticleSpawner : MonoBehaviour
//{
//    public Particle Prefab;

//    public int NumOfParticles = 10;
//    public float SizeOfParticles = 5;
//    public float SpacingOfParticles = 1;

//    public float BorderWidth = 30;
//    public float BorderHeight = 14;

//    Particle[] Particles;
//    Vector3[] ParticlePositions;
//    Vector3[] ParticleVelocities;

//    private void Start()
//    {
//        DrawBorder();

//        Particles = new Particle[NumOfParticles];
//        ParticlePositions = new Vector3[NumOfParticles];
//        ParticleVelocities = new Vector3[NumOfParticles];

//        int particlesInRow = (int)math.sqrt(NumOfParticles);
//        int particlesInCol = (NumOfParticles - 1) / particlesInRow + 1;
//        float spacing = SizeOfParticles * 2 + SpacingOfParticles;

//        SpawnParticle(new Vector3(Screen.width / 2, Screen.height / 2, 0), SizeOfParticles);
//    }

//    private void DrawBorder()
//    {

//    }

//    private void Update()
//    {

//    }

//    private Particle SpawnParticle(Vector3 position, float radius)
//    {
//        Particle particleToSpawn = Instantiate(
//            Prefab,
//            position,
//            Quaternion.identity,
//            transform
//        );
//        particleToSpawn.transform.localScale = new Vector3(radius, radius, 1);

//        return particleToSpawn;
//    }

//    private void MoveParticle(Vector3 velocity, int index)
//    {
//        Particles[index].transform.position += velocity * Time.deltaTime;
//    }
//}
