using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    [Range(3, 10)]
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

        foreach (Transform ropeTrans in _ropeTransforms)
        {
            RopeNode newNode = new RopeNode
            {
                transform = ropeTrans,
                radius = _radius
            };
            _ropeNodes.Add(newNode);
        }

        List<Vector3> vertices = new List<Vector3>(_ropeNodes.Count * _details);
        for (int i = 0; i < _ropeNodes.Count; i++)
        {
            vertices.AddRange(GetCircleCoords(_ropeNodes[i]));
        }
        mesh.vertices = vertices.ToArray();

            int[] tris = new int[3 * _details];

        for (int i = 0; i < _ropeNodes.Count - 1; i++)
        {
            List<int> currentNodes = new List<int>(2 * _details);

            for (int j = 0; j < currentNodes.Count; j++)
            {
                currentNodes.Add(i * _details + j);
            }

            for (int j = 0; j < _details; j++)
            {
                int lowL = j % (_details * 2),
                    lowR = (j + 1) % (_details * 2),
                    topL = (_details * j) % (_details * 2),
                    topR = (_details * j + 1) % (_details * 2);

                tris[j * 3] = lowL;
                tris[j * 3 + 1] = topL;
                tris[j * 3 + 2] = lowR;
                     
                tris[j * 3 + 3] = topR;
                tris[j * 3 + 4] = lowR;
                tris[j * 3 + 5] = topL;
            }
        }

        mesh.triangles = tris;
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
            Vector3 coord = new Vector3(Mathf.Cos(poinRadians) * _radius, Mathf.Sin(poinRadians) * _radius, 0) + node.transform.position;
            circleCoords[i] = coord;
        }
        
        return circleCoords;
    }

    struct RopeNode
    {
        public Transform transform;
        public float radius;
    }
}
