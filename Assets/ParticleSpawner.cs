using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;
using System;

public class ParticleSpawner : MonoBehaviour
{
    public Material lineMaterial;
    public Particle Prefab;
    private LineRenderer lineRenderer;

    public int NumOfParticles = 1;
    public float SizeOfParticles = 1;
    public float SpacingOfParticles = 1;
    public bool SpawnRandom = false;

    public float CollisionDamper = 1;
    public float Gravity = 0;

    public float SmoothingRadius = 1;
    public float Mass = 1;
    public float TargetDensity = 1;
    public float PressureMultiplier = 1;

    public float BorderWidth = 30;
    public float BorderHeight = 14;

    private float LeftBound;
    private float RightBound;
    private float LowerBound;
    private float UpperBound;

    private float DeltaTime = 1 / 120f;

    Particle[] Particles;
    Vector3[] ParticlePositions;
    Vector3[] ParticlePositionPredictions;
    Vector3[] ParticleVelocities;
    Vector3[] ParticleForces;
    float[] ParticleDensities;

    private void Start()
    {

        DrawBorder();

        Particles = new Particle[NumOfParticles];
        ParticlePositions = new Vector3[NumOfParticles];
        ParticlePositionPredictions = new Vector3[NumOfParticles];
        ParticleVelocities = new Vector3[NumOfParticles];
        ParticleForces = new Vector3[NumOfParticles];
        ParticleDensities = new float[NumOfParticles];

        if (!SpawnRandom)
        {
            int particlesInRow = (int)math.sqrt(NumOfParticles);
            int particlesInCol = (NumOfParticles - 1) / particlesInRow + 1;
            float spacing = SizeOfParticles * (1 + SpacingOfParticles);

            for (int i = 0; i < NumOfParticles; i++)
            {
                float x = (i % particlesInRow - particlesInRow / 2f + 0.5f) * spacing + (Screen.width / 2);
                float y = (i / particlesInRow - particlesInCol / 2f + 0.5f) * spacing + (Screen.height / 2);
                ParticlePositions[i] = new Vector3(x, y, 0);
                ParticlePositionPredictions[i] = new Vector3(x, y, 0);
                Particles[i] = SpawnParticle(ParticlePositions[i], SizeOfParticles / 2);
            }
        }
        else
        {
            for (int i = 0; i < NumOfParticles; i++)
            {
                ParticlePositions[i] = new Vector3(UnityEngine.Random.Range(LeftBound, RightBound), UnityEngine.Random.Range(LowerBound, UpperBound), 0);
                ParticlePositionPredictions[i] = new Vector3(ParticlePositions[i].x, ParticlePositions[i].y, 0);
                Particles[i] = SpawnParticle(ParticlePositions[i], SizeOfParticles / 2);
            }
        }
        
    }

    private void Update()
    {
        CalculateForces();
        for (int i = 0; i < NumOfParticles; i++)
        {
            ParticleVelocities[i] += (ParticleForces[i] / ParticleDensities[i] + Vector3.down * Gravity)* Time.deltaTime;
            ResolveCollision(i);
            Particles[i].transform.position += ParticleVelocities[i] * Time.deltaTime;
            ParticlePositions[i] = Particles[i].transform.position;
            ParticlePositionPredictions[i] = ParticlePositions[i] + ParticleVelocities[i] * DeltaTime;
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
            Particles[index].transform.position = new Vector3(LeftBound + SizeOfParticles / 2f, currentPosition.y, 0);
            ParticleVelocities[index].x *= -1;
            ParticleVelocities[index] *= CollisionDamper;
        }
        else if (currentPosition.x + SizeOfParticles / 2f > RightBound)
        {
            Particles[index].transform.position = new Vector3(RightBound - SizeOfParticles / 2f, currentPosition.y, 0);
            ParticleVelocities[index].x *= -1;
            ParticleVelocities[index] *= CollisionDamper;
        }

        if (currentPosition.y - SizeOfParticles / 2f < LowerBound)
        {
            Particles[index].transform.position = new Vector3(currentPosition.x, LowerBound + SizeOfParticles / 2f, 0);
            ParticleVelocities[index].y *= -1;
            ParticleVelocities[index] *= CollisionDamper;
        }
        else if (currentPosition.y + SizeOfParticles / 2f > UpperBound)
        {
            Particles[index].transform.position = new Vector3(currentPosition.x, UpperBound - SizeOfParticles / 2f, 0);
            ParticleVelocities[index].y *= -1;
            ParticleVelocities[index] *= CollisionDamper;
        }
    }

    private void CalculateForces()
    {
        CalculateParticleDensities();
        for (int i = 0; i < NumOfParticles; i++)
        {
            ParticleForces[i] = GetPressureForce(i);
        }
    }

    private float GetSmoothingRadius(float radius, float dist) {
        if (dist >= radius) return 0;
        float volume = (math.PI * math.pow(radius, 4)) / 6;
        return (radius - dist) * (radius - dist) / volume;
    }

    private float GetSmoothingDerivative(float radius, float dist)
    {
        if (dist >= radius) return 0;
        float scale = 12 / (math.pow(radius, 4) * math.PI);
        return (radius - dist) * scale;
    }

    private float GetDensity(Vector3 point)
    {
        float density = 0;

        for (int i = 0; i < NumOfParticles; i++)
        {
            float distance = (ParticlePositionPredictions[i] - point).magnitude;
            float influence = GetSmoothingRadius(SmoothingRadius, distance);
            density += Mass * influence;
        }
        return density;
    }

    private Vector3 GetPressureForce(int index)
    {
        Vector3 pressureForce = Vector3.zero;

        for (int i = 0; i < NumOfParticles; i++)
        {
            if (i == index) continue;

            Vector3 offset = ParticlePositionPredictions[i] - ParticlePositionPredictions[index];
            float dist = offset.magnitude;
            Vector3 direction = dist == 0 ? GetRandomDirection() : offset / dist;
            float slope = GetSmoothingDerivative(SmoothingRadius, dist);
            float density = ParticleDensities[i];
            float sharedPressure = GetSharedPressure(density, ParticleDensities[index]);
            pressureForce += Mass * sharedPressure * slope * direction / density;
        }
        return pressureForce;
    }

    private void CalculateParticleDensities()
    {
        for (int i = 0; i < NumOfParticles; i++)
        {
            ParticleDensities[i] = GetDensity(ParticlePositionPredictions[i]);
        }
    }

    private float DensityToPressure(float density)
    {
        float error = density - TargetDensity;
        return error * PressureMultiplier;
    }

    private float GetSharedPressure(float densityA, float densityB)
    {
        float pressureA = DensityToPressure(densityA);
        float pressureB = DensityToPressure(densityB);
        return (pressureA + pressureB) / 2;
    }


    private Vector3 GetRandomDirection()
    {
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random();
        float x = rand.NextFloat(-1, 1);
        float y = rand.NextFloat(-1, 1);
        return new Vector3(x, y, 0);
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
