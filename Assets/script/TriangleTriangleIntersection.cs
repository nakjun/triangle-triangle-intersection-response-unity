using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriangleTriangleIntersection : MonoBehaviour
{
    [System.Serializable] // Unity �ν����Ϳ��� ���� �����ϰ� ����ϴ�.
    public class Triangle
    {
        public Vector3 vertex0, vertex1, vertex2; // �ﰢ���� ������
    }

    public Triangle triangle1;
    public Triangle triangle2;

    public Vector3 gravity = new Vector3(0.0f, -9.8f, 0.0f);

    void Start()
    {
        triangle1.vertex0 = new Vector3(-12.0f, 15.0f, 0.0f);
        triangle1.vertex1 = new Vector3(-0.0f, 15.0f, 10.0f);
        triangle1.vertex2 = new Vector3(14.0f, 15.0f, 0.0f);

        triangle2.vertex0 = new Vector3(-13.0f, 0.0f, 0.0f);
        triangle2.vertex1 = new Vector3(-0.0f, 0.0f, 10.0f);
        triangle2.vertex2 = new Vector3(13.0f, 0.0f, 0.0f);
    }

    void UpdatePosition()
    {
        triangle1.vertex0 += gravity * 0.001f;
        triangle1.vertex1 += gravity * 0.001f;
        triangle1.vertex2 += gravity * 0.001f;
    }

    void Update()
    {
        UpdatePosition();

        var check_intersection = TrianglesIntersect(triangle1, triangle2);
        var check_inside = IsTriangleInsideTriangle(triangle1, triangle2) || IsTriangleInsideTriangle(triangle2, triangle1);
        var check_sharePos = IsSharingAtLeastOneVertex(triangle1, triangle2);

        Debug.Log("check_intersection:" + check_intersection);
        Debug.Log("check_inside:" + check_inside);
        Debug.Log("check_sharePos:" + check_sharePos);

        if (check_intersection || check_inside || check_sharePos)
        {
            Debug.Log("��¶�� �ϳ��� �����ϴ� �ɷ� �Ǵܵ�");
        }        
    }

    void OnDrawGizmos()
    {
        if (triangle1 != null)
        {
            DrawTriangle(triangle1, Color.red);
        }

        if (triangle2 != null)
        {
            DrawTriangle(triangle2, Color.blue);
        }
    }

    void DrawTriangle(Triangle triangle, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(triangle.vertex0, triangle.vertex1);
        Gizmos.DrawLine(triangle.vertex1, triangle.vertex2);
        Gizmos.DrawLine(triangle.vertex2, triangle.vertex0);
    }

    bool IsSharingAtLeastOneVertex(Triangle tri1, Triangle tri2)
    {
        Vector3[] verticesTri1 = new Vector3[] { tri1.vertex0, tri1.vertex1, tri1.vertex2 };
        Vector3[] verticesTri2 = new Vector3[] { tri2.vertex0, tri2.vertex1, tri2.vertex2 };

        foreach (var vertex1 in verticesTri1)
        {
            foreach (var vertex2 in verticesTri2)
            {
                if (vertex1 == vertex2) // Vector3 ��
                {                    
                    return true; // �����ϴ� ���� ã��
                }
            }
        }

        return false; // �����ϴ� �� ����
    }

    public bool CheckTrianglesEdgeIntersection(Triangle triangle1, Triangle triangle2)
    {
        // �� �ﰢ���� �������� �迭�� ǥ��
        Vector3[] triangle1Vertices = { triangle1.vertex0, triangle1.vertex1, triangle1.vertex2 };
        Vector3[] triangle2Vertices = { triangle2.vertex0, triangle2.vertex1, triangle2.vertex2 };

        // ��� �� �ֿ� ���� ���� �˻�
        for (int i = 0; i < 3; i++) // triangle1�� ��
        {
            for (int j = 0; j < 3; j++) // triangle2�� ��
            {
                Vector3 p1 = triangle1Vertices[i];
                Vector3 p2 = triangle1Vertices[(i + 1) % 3]; // ���� ���������� ��ȯ
                Vector3 q1 = triangle2Vertices[j];
                Vector3 q2 = triangle2Vertices[(j + 1) % 3]; // ���� ���������� ��ȯ

                if (LineSegmentsIntersect(p1, p2, q1, q2))
                {
                    return true; // �����ϴ� �� �� �߰�
                }
            }
        }

        // �����ϴ� �� �� ����
        return false;
    }


    bool LineSegmentsIntersect(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
    {
        Vector3 r = p2 - p1;
        Vector3 s = q2 - q1;
        float rCrossS = Vector3.Cross(r, s).magnitude;

        // �� ������ ������ ��� ó��
        if (Mathf.Approximately(rCrossS, 0))
        {
            // �� ������ ���� �� ���� �ִ��� Ȯ��
            Vector3 q1p1 = q1 - p1;
            if (Mathf.Approximately(Vector3.Cross(q1p1, r).magnitude, 0))
            {
                // �� ������ ���� �� ���� ����. �����ϴ��� ���θ� �߰������� �˻�
                // �� ������ ��Į�� ������ ����Ͽ� ��ġ���� Ȯ��
                float t0 = Vector3.Dot(q1 - p1, r) / Vector3.Dot(r, r);
                float t1 = Vector3.Dot(q2 - p1, r) / Vector3.Dot(r, r);

                // t0�� t1�� ������ ����(��ȯ)�Ͽ� �׻� t0�� t1���� �۰ų� ������ ��
                if (t0 > t1) (t0, t1) = (t1, t0);

                // �� ������ ��ġ�� ���� t0 <= 1 && t1 >= 0 �� ��
                return t0 <= 1 && t1 >= 0;
            }
            return false;
        }

        float t = Vector3.Cross(q1 - p1, s).magnitude / rCrossS;
        float u = Vector3.Cross(q1 - p1, r).magnitude / rCrossS;

        // t�� u�� ��� 0�� 1 ���̿� ������ �����մϴ�.
        return (t >= 0 && t <= 1 && u >= 0 && u <= 1);
    }


    bool TrianglesIntersect(Triangle tri1, Triangle tri2)
    {
        // �� �ﰢ���� �𼭸��� �������� ����Ͽ� �ٸ� �ﰢ������ ������ �˻�
        if (CheckEdgeAgainstTriangle(tri1, tri2) || CheckEdgeAgainstTriangle(tri2, tri1))
        {
            return true;
        }

        return false;
    }

    bool CheckEdgeAgainstTriangle(Triangle triEdge, Triangle triTarget)
    {
        Vector3[] edges = new Vector3[3]
        {
            triEdge.vertex1 - triEdge.vertex0,
            triEdge.vertex2 - triEdge.vertex1,
            triEdge.vertex0 - triEdge.vertex2
        };
        Vector3[] vertices = new Vector3[3] { triEdge.vertex0, triEdge.vertex1, triEdge.vertex2 };

        foreach (Vector3 edge in edges)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                if (RayIntersectsTriangle(vertices[i], edge.normalized, triTarget.vertex0, triTarget.vertex1, triTarget.vertex2, out float t, out float u, out float v))
                {
                    return true; // ���� �߰�
                }
            }
        }

        return false;
    }

    bool IsTriangleInsideTriangle(Triangle tri1, Triangle tri2)
    {
        // Check if all vertices of tri1 are inside tri2
        return IsPointInsideTriangle(tri1.vertex0, tri2.vertex0, tri2.vertex1, tri2.vertex2) &&
               IsPointInsideTriangle(tri1.vertex1, tri2.vertex0, tri2.vertex1, tri2.vertex2) &&
               IsPointInsideTriangle(tri1.vertex2, tri2.vertex0, tri2.vertex1, tri2.vertex2) &&
               IsYValueInsideRange(tri1.vertex0, tri1.vertex1, tri1.vertex2, tri2.vertex0, tri2.vertex1, tri2.vertex2);
    }

    bool IsPointInsideTriangle(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0); // �ﰢ���� ���� ���� ���

        // [v0, v1] ���� ���� �˻�
        Vector3 edge1 = v1 - v0;
        Vector3 vp1 = point - v0;
        if (Vector3.Dot(Vector3.Cross(edge1, vp1), normal) < 0) return false;

        // [v1, v2] ���� ���� �˻�
        Vector3 edge2 = v2 - v1;
        Vector3 vp2 = point - v1;
        if (Vector3.Dot(Vector3.Cross(edge2, vp2), normal) < 0) return false;

        // [v2, v0] ���� ���� �˻�
        Vector3 edge3 = v0 - v2;
        Vector3 vp3 = point - v2;
        if (Vector3.Dot(Vector3.Cross(edge3, vp3), normal) < 0) return false;

        return true; // ��� �˻縦 ����ߴٸ�, ���� �ﰢ�� ���ο� �ֽ��ϴ�.
    }

    bool IsYValueInsideRange(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5)
    {
        float minYTri2 = Mathf.Min(v3.y, v4.y, v5.y) - 0.1f;
        float maxYTri2 = Mathf.Max(v3.y, v4.y, v5.y) + 0.1f;

        // tri1�� ��� ������ y���� tri2�� y�� ���� ���� �ִ��� Ȯ��
        return (v0.y >= minYTri2 && v0.y <= maxYTri2) &&
               (v1.y >= minYTri2 && v1.y <= maxYTri2) &&
               (v2.y >= minYTri2 && v2.y <= maxYTri2);
    }

    public static bool RayIntersectsTriangle(Vector3 rayOrigin,
                                             Vector3 rayVector,
                                             Vector3 vertex0, Vector3 vertex1, Vector3 vertex2,
                                             out float t, out float u, out float v)
    {
        const float EPSILON = 5.0f;
        Vector3 edge1, edge2, h, s, q;
        float a, f;

        edge1 = vertex1 - vertex0;
        edge2 = vertex2 - vertex0;
        h = Vector3.Cross(rayVector, edge2);
        a = Vector3.Dot(edge1, h);

        if (a > -EPSILON && a < EPSILON)
        {
            t = u = v = 0;
            return false;    // �� ������ �ﰢ���� �����Դϴ�.
        }

        f = 1.0f / a;
        s = rayOrigin - vertex0;
        u = f * Vector3.Dot(s, h);

        if (u < 0.0 || u > 1.0)
        {
            t = u = v = 0;
            return false;
        }

        q = Vector3.Cross(s, edge1);
        v = f * Vector3.Dot(rayVector, q);

        if (v < 0.0 || u + v > 1.0)
        {
            t = u = v = 0;
            return false;
        }

        // �� �������� �츮�� ������ ����� �� ������, ���� ���������� �Ÿ��� t�� ����մϴ�.
        t = f * Vector3.Dot(edge2, q);
        return t > EPSILON; // ���� ����
    }
}
