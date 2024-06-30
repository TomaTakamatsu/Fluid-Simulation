using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class ParticleSpawner : MonoBehaviour
{
    public Material lineMaterial;
    public Particle Prefab;
    private LineRenderer lineRenderer;

    public int NumOfParticles = 10;
    public float SizeOfParticles = 5;
    public float SpacingOfParticles = 1;

    public float BorderWidth = 30;
    public float BorderHeight = 14;


    Particle[] Particles;
    Vector3[] ParticlePositions;
    Vector3[] ParticleVelocities;

    private void Start()
    {

        DrawBorder();

        Particles = new Particle[NumOfParticles];
        ParticlePositions = new Vector3[NumOfParticles];
        ParticleVelocities = new Vector3[NumOfParticles];

        int particlesInRow = (int)math.sqrt(NumOfParticles);
        int particlesInCol = (NumOfParticles - 1) / particlesInRow + 1;
        float spacing = SizeOfParticles * 2 + SpacingOfParticles;

        SpawnParticle(new Vector3(Screen.width / 2, Screen.height / 2, 0), SizeOfParticles);
    }

    private void Update()
    {
        // Existing update logic
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

    private void MoveParticle(Vector3 velocity, int index)
    {
        Particles[index].transform.position += velocity * Time.deltaTime;
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

        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Define corners of the box
        Vector3 topLeft = center + new Vector3(-BorderWidth / 2, BorderHeight / 2, 0);
        Vector3 topRight = center + new Vector3(BorderWidth / 2, BorderHeight / 2, 0);
        Vector3 bottomRight = center + new Vector3(BorderWidth / 2, -BorderHeight / 2, 0);
        Vector3 bottomLeft = center + new Vector3(-BorderWidth / 2, -BorderHeight / 2, 0);

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
