using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class ParticleSpawner : MonoBehaviour
{
    public int NumOfParticles = 1;
    public int WidthOfStart = 5;
    public int HeightFromTop = 20;

    public float ParticleSize = 1f;
    public float ParticleSpacing = 0.5f;

    public Material circleMaterial; // Add this line

    private Vector2[] ParticlePositions;

    private void Awake()
    {
        int rows = Mathf.CeilToInt((float)NumOfParticles / WidthOfStart);
        ParticlePositions = new Vector2[NumOfParticles];
        int startX = Screen.mainWindowPosition.x + Mathf.CeilToInt((NumOfParticles * 0.5f) * (ParticleSize + ParticleSpacing));
        int startY = Screen.mainWindowPosition.y;
        int particleIndex = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < WidthOfStart; x++)
            {
                particleIndex = x * y;
                if (particleIndex >= NumOfParticles) break;
                ParticlePositions[particleIndex] = new Vector2(startX + Mathf.CeilToInt(x * (ParticleSize + ParticleSpacing)), startY - Mathf.CeilToInt(y * (ParticleSize + ParticleSpacing)));
            }
        }

        for (int i = 0; i < NumOfParticles; i++)
        {
            DrawCircle(ParticlePositions[i], ParticleSize / 2);
        }
    }

    private void Start()
    {
        
    }

    private void DrawCircle(Vector2 center, float radius, int numSegments = 100)
    {
        GameObject circleObject = new GameObject("FilledCircle_" + center);
        circleObject.transform.position = new Vector3(center.x, center.y, 0); // Ensure z = 0

        MeshFilter mf = circleObject.AddComponent<MeshFilter>();
        MeshRenderer mr = circleObject.AddComponent<MeshRenderer>();
        mr.material = circleMaterial;

        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        Vector3[] vertices = new Vector3[numSegments + 1];
        int[] triangles = new int[numSegments * 3];
        vertices[0] = Vector3.zero;

        float angle = 2 * Mathf.PI / numSegments;
        for (int i = 1; i <= numSegments; i++)
        {
            float x = Mathf.Cos(i * angle) * radius;
            float y = Mathf.Sin(i * angle) * radius;
            vertices[i] = new Vector3(x, y, 0);
        }

        for (int i = 0; i < numSegments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 2 > numSegments) ? 1 : i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
