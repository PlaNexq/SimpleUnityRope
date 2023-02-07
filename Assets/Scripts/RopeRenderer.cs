using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RopeRenderer : MonoBehaviour
{
    public Transform[] RopeTransforms { get => _ropeTransforms; set => _ropeTransforms = value; }
    public Material Material { get => _material; set => _material = value; }
    public int Details { get => _details; set => _details = value; }
    public float Radius { get => _radius; set => _radius = value; }

    [Range(3, 20)]
    [SerializeField] private int _details = 3;
    [SerializeField] private float _radius = 0.5f;

    [SerializeField] private Transform[] _ropeTransforms;
    [SerializeField] private Material _material;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<Vector3> _vertices;
    private List<Vector3> _normals;
    private List<int> _triangles;
    private List<Vector2> _uvs;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        CalcVertsAndNormals();
        CalcTris();
        CalcUVs();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        CalcVertsAndNormals();
        UpdateMesh();
    }

    private void Setup()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        meshRenderer.sharedMaterial = _material;
        meshFilter.mesh = mesh;

        _details++;
        _vertices = new List<Vector3>(new Vector3[_details * _ropeTransforms.Length]);
        _normals = new List<Vector3>(new Vector3[_vertices.Capacity]);
        _triangles = new List<int>(new int[6 * _vertices.Capacity]);
        _uvs = new List<Vector2>(new Vector2[_vertices.Capacity]); 
    }

    private void CalcVertsAndNormals()
    {
        int index;
        for (int i = 0; i < _ropeTransforms.Length; i++)
        {
            for (int j = 0; j < _details - 1; j++)
            {
                float poinRadians = j * (2 * Mathf.PI / _details);
                Vector3 normal = new Vector3(Mathf.Cos(poinRadians), Mathf.Sin(poinRadians), 0);
                Vector3 pos = normal * _radius + _ropeTransforms[i].position;

                index = i * _details + j;
                _vertices[index] = pos;
                _normals[index] = normal;
            }
            index = (i + 1) * _details - 1;
            _vertices[index] = _vertices[index - (_details - 1)];
            _normals[index] = _normals[index - (_details - 1)];
        }
    }

    private void CalcTris()
    {
        // iterate through all pairs of neighbouring nodes
        for (int i = 0; i < _ropeTransforms.Length - 1; i++)
        {
            List<int> currentVertices = new List<int>(2 * _details);

            // write all vertice indexes that we'll use
            for (int j = 0; j < currentVertices.Capacity; j++)
            {
                int verticeIndex = i * _details + j;
                currentVertices.Add(verticeIndex);
            }

            // calculate all faces between two nodes
            for (int j = 0; j < _details - 1; j++)
            {
                int offset = currentVertices.Capacity / 2,
                    topL = currentVertices[j + offset], topR = currentVertices[j + offset + 1],
                    lowL = currentVertices[j], lowR = currentVertices[j + 1];

                // calculate one quad
                _triangles[i * 6 * _details + (j * 6)] = lowL;
                _triangles[i * 6 * _details + (j * 6 + 1)] = lowR;
                _triangles[i * 6 * _details + (j * 6 + 2)] = topL;

                _triangles[i * 6 * _details + (j * 6 + 3)] = topR;
                _triangles[i * 6 * _details + (j * 6 + 4)] = topL;
                _triangles[i * 6 * _details + (j * 6 + 5)] = lowR;
            }
        }
    }

    private void CalcUVs()
    {
        for (int i = 0; i < _ropeTransforms.Length; i++)
        {
            for (int j = 0; j < _details; j++)
            {
                Vector2 uvValue = new Vector2( i/(float)(_ropeTransforms.Length - 1), j/(float)(_details - 1) );

                int index = i * _details + j;
                _uvs[index] = uvValue;
            }
        }
    }

    private void UpdateMesh()
    {
        mesh.SetVertices(_vertices);
        mesh.SetTriangles(_triangles, 0);
        mesh.SetNormals(_normals);
        mesh.SetUVs(0, _uvs);
    }
}
