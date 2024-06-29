using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class ParticleSpawner : MonoBehaviour
{
    public Particle Prefab;

    public int NumOfParticles = 10;
    public float SizeOfParticles = 5;
    public float SpacingOfParticles = 1;

    public float BorderWidth = 50;
    public float BorderHeight = 28;

    Particle[] Particles;
    Vector3[] ParticlePositions;
    Vector3[] ParticleVelocities;

    private void Start()
    {
        Particles = new Particle[NumOfParticles];
        ParticlePositions = new Vector3[NumOfParticles];
        ParticleVelocities = new Vector3[NumOfParticles];

        int particlesInRow = (int)math.sqrt(NumOfParticles);
        int particlesInCol = (NumOfParticles - 1) / particlesInRow + 1;
        float spacing = SizeOfParticles * 2 + SpacingOfParticles;

        SpawnParticle(new Vector3(Screen.width / 2, Screen.height / 2, 0), SizeOfParticles);
    }

    private void DrawBorder()
    {
        Debug.DrawLine(new Vector3(-1 * BorderWidth / 2, BorderHeight / 2, 0), new Vector3(BorderWidth / 2, BorderHeight / 2, 0), Color.green);
    }

    private void Update()
    {
        
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
}
