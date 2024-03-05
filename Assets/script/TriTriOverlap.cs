using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriTriOverlap : MonoBehaviour
{
    [System.Serializable] // Unity 인스펙터에서 편집 가능하게 만듭니다.
    public class Triangle
    {
        public Vector3 vertex0, vertex1, vertex2; // 삼각형의 꼭지점
        public Vector3 p_vertex0, p_vertex1, p_vertex2; // 삼각형의 이전 위치
        public Vector3 vel0, vel1, vel2;

        public Vector3 gravity = new Vector3(0.0f, -9.8f, 0.0f);
        public float deltaTime = 0.01f;

        public void update()
        {
            this.vel0 += this.gravity * this.deltaTime;
            this.vertex0 += this.vel0 * this.deltaTime;

            this.vel1 += this.gravity * this.deltaTime;
            this.vertex1 += this.vel1 * this.deltaTime;

            this.vel2 += this.gravity * this.deltaTime;
            this.vertex2 += this.vel2 * this.deltaTime;
        }

        public void setZeroGravity()
        {
            this.gravity = Vector3.zero;
        }

        public void setInverseGravity()
        {
            this.gravity *= -1.0f;
        }

        public Vector3 getAverageVelocity()
        {
            return (this.vel0 + this.vel1 + this.vel2) / 3.0f;
        }
    }

    [System.Serializable] // Unity 인스펙터에서 편집 가능하게 만듭니다.
    public class Line
    {
        public Vector3 p0, p1; // 삼각형의 꼭지점

        public Vector3 direction
        {
            get { return (p1 - p0).normalized; }
        }

        public Vector3 origin
        {
            get { return p0; }
        }
    }
    public List<Triangle> triangles;
    public Triangle triangle1;
    public Triangle triangle2;


    public Vector3 hitPoint = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        triangle1.vertex0 = new Vector3(-13.0f, 2.0f, 0.0f);
        triangle1.vertex1 = new Vector3(-0.0f, 3.0f, 10.0f);
        triangle1.vertex2 = new Vector3(13.0f, 2.0f, 0.0f);

        triangle1.deltaTime = 0.001f;

        triangle2.vertex0 = new Vector3(-13.0f, 0.0f, 0.0f);
        triangle2.vertex1 = new Vector3(-0.0f, 0.0f, 10.0f);
        triangle2.vertex2 = new Vector3(13.0f, 0.0f, 0.0f);

        triangle2.deltaTime = 0.001f;
        triangle2.setZeroGravity();

        triangles.Add(triangle1);
        triangles.Add(triangle2);
    }

    void UpdatePosition()
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i].update();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (CheckEdgeCollision(triangle1.vertex0, triangle1.vertex1) || CheckEdgeCollision(triangle1.vertex0, triangle1.vertex2) || CheckEdgeCollision(triangle1.vertex1, triangle1.vertex2))
        {
            // 충돌 지점과 삼각형의 각 정점 간의 평균을 이용하여 이동 벡터 계산
            Vector3 collisionPoint = hitPoint; // 충돌 지점

            // 충돌 지점과 가장 가까운 정점을 찾음
            Vector3 closestVertex = FindClosestVertex(triangle1, collisionPoint);

            Vector3 averageVel = triangle1.getAverageVelocity();

            //// 이동 벡터를 사용하여 충돌이 발생한 정점을 충돌 지점으로 이동시킴
            if (closestVertex == triangle1.vertex0)
            {
                triangle1.vertex0 -= (triangle1.vel0 * triangle1.deltaTime * 2.0f);
                triangle1.vel0 *= -1.0f;
            }
            else if (closestVertex == triangle1.vertex1)
            {
                triangle1.vertex1 -= (triangle1.vel1 * triangle1.deltaTime * 2.0f);
                triangle1.vel1 *= -1.0f;
            }
            else if (closestVertex == triangle1.vertex2)
            {
                triangle1.vertex2 -= (triangle1.vel2 * triangle1.deltaTime * 2.0f);
                triangle1.vel2 *= -1.0f;
            }
        }
        
        UpdatePosition();
    }

    bool IsPointInsideTriangle(Vector3 point, Triangle triangle)
    {
        Vector3 normal = Vector3.Cross(triangle.vertex1 - triangle.vertex0, triangle.vertex2 - triangle.vertex0); // 삼각형의 법선 벡터 계산

        // [v0, v1] 변에 대한 검사
        Vector3 edge1 = triangle.vertex1 - triangle.vertex0;
        Vector3 vp1 = point - triangle.vertex0;
        if (Vector3.Dot(Vector3.Cross(edge1, vp1), normal) < 0) return false;

        // [v1, v2] 변에 대한 검사
        Vector3 edge2 = triangle.vertex2 - triangle.vertex1;
        Vector3 vp2 = point - triangle.vertex1;
        if (Vector3.Dot(Vector3.Cross(edge2, vp2), normal) < 0) return false;

        // [v2, v0] 변에 대한 검사
        Vector3 edge3 = triangle.vertex0 - triangle.vertex2;
        Vector3 vp3 = point - triangle.vertex2;
        if (Vector3.Dot(Vector3.Cross(edge3, vp3), normal) < 0) return false;

        return true; // 모든 검사를 통과했다면, 점은 삼각형 내부에 있습니다.
    }

    Vector3 FindClosestVertex(Triangle triangle, Vector3 point)
    {
        float minDistance = Mathf.Infinity;
        Vector3 closestVertex = Vector3.zero;

        float distance0 = Vector3.Distance(triangle.vertex0, point);
        float distance1 = Vector3.Distance(triangle.vertex1, point);
        float distance2 = Vector3.Distance(triangle.vertex2, point);

        if (distance0 < minDistance)
        {
            minDistance = distance0;
            closestVertex = triangle.vertex0;
        }
        if (distance1 < minDistance)
        {
            minDistance = distance1;
            closestVertex = triangle.vertex1;
        }
        if (distance2 < minDistance)
        {
            minDistance = distance2;
            closestVertex = triangle.vertex2;
        }

        return closestVertex;
    }

    bool checkPointCollision(Vector3 p, Triangle triangle)
    {
        return IsPointInsideTriangle(p, triangle);
    }

    bool CheckEdgeCollision(Vector3 vertex1, Vector3 vertex2)
    {
        var edge = new Line();

        edge.p0 = vertex1;
        edge.p1 = vertex2;

        return Intersect(triangle2, edge, ref hitPoint);
    }

    public bool Intersect(Triangle triangle, Line ray, ref Vector3 hit)
    {

        // Vectors from p1 to p2/p3 (edges)
        //Find vectors for edges sharing vertex/point p1
        Vector3 e1 = triangle.vertex1 - triangle.vertex0;
        Vector3 e2 = triangle.vertex2 - triangle.vertex0;

        // Calculate determinant
        Vector3 p = Vector3.Cross(ray.direction, e2);

        //Calculate determinat
        float det = Vector3.Dot(e1, p);

        //if determinant is near zero, ray lies in plane of triangle otherwise not
        if (det > -Mathf.Epsilon && det < Mathf.Epsilon)
        {
            var coplanar = IsPointInsideTriangle(ray.p0, triangle);
            var coplanar2 = IsPointInsideTriangle(ray.p1, triangle);

            if(coplanar) hit = ray.p0;
            if (coplanar2) hit = ray.p1;

            return coplanar || coplanar2;
        }
        float invDet = 1.0f / det;

        //calculate distance from p1 to ray origin
        Vector3 t = ray.origin - triangle.vertex0;

        //Calculate u parameter
        float u = Vector3.Dot(t, p) * invDet;

        //Check for ray hit
        if (u < 0 || u > 1) { return false; }

        //Prepare to test v parameter
        Vector3 q = Vector3.Cross(t, e1);

        //Calculate v parameter
        float v = Vector3.Dot(ray.direction, q) * invDet;

        //Check for ray hit
        if (v < 0 || u + v > 1) { return false; }

        // intersection point
        hit = triangle.vertex0 + u * e1 + v * e2;

        if ((Vector3.Dot(e2, q) * invDet) > Mathf.Epsilon)
        {
            //ray does intersect            
            return true;
        }

        // No hit at all
        return false;
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
}
