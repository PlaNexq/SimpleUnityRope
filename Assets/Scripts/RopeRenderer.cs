using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RopeRenderer : MonoBehaviour, IRopeRenderer
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

    private List<RopeNode> _ropeNodes;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = _material;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        _ropeNodes = new List<RopeNode>(_ropeTransforms.Length);
        foreach (Transform ropeTrans in _ropeTransforms)
        {
            RopeNode newNode = new RopeNode
            {
                transform = ropeTrans,
            };
            _ropeNodes.Add(newNode);
        }

        List<Vector3> vertices = new List<Vector3>(_ropeNodes.Count * _details);
        for (int i = 0; i < _ropeNodes.Count; i++)
        {
            vertices.AddRange(GetCircleCoords(_ropeNodes[i]));
        }
        mesh.vertices = vertices.ToArray();

        int[] triangles = new int[6 * _details * _ropeNodes.Count];

        // iterate through all pairs of neighbouring nodes
        for (int i = 0; i < _ropeNodes.Count - 1; i++)
        {
            List<int> currentVertices = new List<int>(2 * _details);

            // write all vertice indexes that we'll use
            for (int j = 0; j < currentVertices.Capacity; j++)
            {
                int verticeIndex = i * _details + j;
                currentVertices.Add(verticeIndex);
            }

            // calculate all faces (pairs of triangles or quads) between two nodes
            for (int j = 0; j < _details; j++)
            {
                int offset = currentVertices.Capacity / 2,
                    topL = currentVertices[j + offset], topR = currentVertices[(j + 1) % offset + offset],
                    lowL = currentVertices[j], lowR = currentVertices[(j + 1) % offset];

                triangles[i * 6 * _details + (j * 6)] = lowL;
                triangles[i * 6 * _details + (j * 6 + 1)] = lowR;
                triangles[i * 6 * _details + (j * 6 + 2)] = topL;

                triangles[i * 6 * _details + (j * 6 + 3)] = topR;
                triangles[i * 6 * _details + (j * 6 + 4)] = topL;
                triangles[i * 6 * _details + (j * 6 + 5)] = lowR;
            }

            currentVertices.Clear();
        }

        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector3[] GetCircleCoords(RopeNode node)
    {
        Vector3[] circleCoords = new Vector3[_details];
        for (int i = 0; i < _details; i++)
        {
            float poinRadians = i * (2 * Mathf.PI / _details);
            Vector3 coord = new Vector3(Mathf.Cos(poinRadians), Mathf.Sin(poinRadians), 0) * _radius + node.transform.position;
            circleCoords[i] = coord;
        }

        return circleCoords;
    }

    void IRopeRenderer.Render()
    {
        throw new System.NotImplementedException();
    }

    struct RopeNode
    {
        public Transform transform;
        public Vertice[] vertices;
    }
    struct Vertice
    {
        public Vector3 localPos;
        public Vector3 normal;
        public int index;
    }
}
