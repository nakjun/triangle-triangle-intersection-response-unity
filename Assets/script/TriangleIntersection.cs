using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleIntersection : MonoBehaviour
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
            this.vel0 += gravity * deltaTime;
            this.vertex0 += this.vel0 * deltaTime;

            this.vel1 += gravity * deltaTime;
            this.vertex1 += this.vel0 * deltaTime;

            this.vel2 += gravity * deltaTime;
            this.vertex2 += this.vel0 * deltaTime;
        }
    }

    // Moller-Trumbore 교차 검사 알고리즘

    bool TriTriCollisionDetection(Triangle A, Triangle B)
    {
        return TriangleTriangleOverlap(A.vertex0, A.vertex1, A.vertex2, B.vertex0, B.vertex1, B.vertex2);
    }

    public static float Orient2D(Vector2 a, Vector2 b, Vector2 c)
    {
        return (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);
    }

    public static bool IntersectionTestVertex(Vector2 P1, Vector2 Q1, Vector2 R1, Vector2 P2, Vector2 Q2, Vector2 R2)
    {
        if (Orient2D(R2, P2, Q1) >= 0.0f)
        {
            if (Orient2D(R2, Q2, Q1) <= 0.0f)
            {
                if (Orient2D(P1, P2, Q1) > 0.0f)
                {
                    if (Orient2D(P1, Q2, Q1) <= 0.0f)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (Orient2D(P1, P2, R1) >= 0.0f)
                    {
                        if (Orient2D(Q1, R1, P2) >= 0.0f)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (Orient2D(P1, Q2, Q1) <= 0.0f)
                {
                    if (Orient2D(R2, Q2, R1) <= 0.0f)
                    {
                        if (Orient2D(Q1, R1, Q2) >= 0.0f)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            if (Orient2D(R2, P2, R1) >= 0.0f)
            {
                if (Orient2D(Q1, R1, R2) >= 0.0f)
                {
                    if (Orient2D(P1, P2, R1) >= 0.0f)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (Orient2D(Q1, R1, Q2) >= 0.0f)
                    {
                        if (Orient2D(R2, R1, Q2) >= 0.0f)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }

    public static bool IntersectionTestEdge(Vector2 P1, Vector2 Q1, Vector2 R1, Vector2 P2, Vector2 Q2, Vector2 R2)
    {
        if (Orient2D(R2, P2, Q1) >= 0.0f)
        {
            if (Orient2D(P1, P2, Q1) >= 0.0f)
            {
                if (Orient2D(P1, Q1, R2) >= 0.0f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Orient2D(Q1, R1, P2) >= 0.0f)
                {
                    if (Orient2D(R1, P1, P2) >= 0.0f)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            if (Orient2D(R2, P2, R1) >= 0.0f)
            {
                if (Orient2D(P1, P2, R1) >= 0.0f)
                {
                    if (Orient2D(P1, R1, R2) >= 0.0f)
                    {
                        return true;
                    }
                    else
                    {
                        if (Orient2D(Q1, R1, R2) >= 0.0f)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public static bool CcwTriTriIntersection2D(Vector2 p1, Vector2 q1, Vector2 r1, Vector2 p2, Vector2 q2, Vector2 r2)
    {
        if (Orient2D(p2, q2, p1) >= 0.0f)
        {
            if (Orient2D(q2, r2, p1) >= 0.0f)
            {
                if (Orient2D(r2, p2, p1) >= 0.0f)
                {
                    return true;
                }
                else
                {
                    return IntersectionTestEdge(p1, q1, r1, p2, q2, r2);
                }
            }
            else
            {
                if (Orient2D(r2, p2, p1) >= 0.0f)
                {
                    return IntersectionTestEdge(p1, q1, r1, r2, p2, q2);
                }
                else
                {
                    return IntersectionTestVertex(p1, q1, r1, p2, q2, r2);
                }
            }
        }
        else
        {
            if (Orient2D(q2, r2, p1) >= 0.0f)
            {
                if (Orient2D(r2, p2, p1) >= 0.0f)
                {
                    return IntersectionTestEdge(p1, q1, r1, q2, r2, p2);
                }
                else
                {
                    return IntersectionTestVertex(p1, q1, r1, q2, r2, p2);
                }
            }
            else
            {
                return IntersectionTestVertex(p1, q1, r1, r2, p2, q2);
            }
        }
    }

    public static bool CheckMinMax(Vector3 p1, Vector3 q1, Vector3 r1, Vector3 p2, Vector3 q2, Vector3 r2)
    {
        Vector3 v1min, v1max, v2min, v2max;
        v1min = Vector3.Min(p1, Vector3.Min(q1, r1));
        v1max = Vector3.Max(p1, Vector3.Max(q1, r1));
        v2min = Vector3.Min(p2, Vector3.Min(q2, r2));
        v2max = Vector3.Max(p2, Vector3.Max(q2, r2));

        if (v1max.x < v2min.x || v1min.x > v2max.x ||
            v1max.y < v2min.y || v1min.y > v2max.y ||
            v1max.z < v2min.z || v1min.z > v2max.z)
            return false;
        else
            return true;
    }

    public static bool TriangleTriangleOverlap(Vector3 p1, Vector3 q1, Vector3 r1, Vector3 p2, Vector3 q2, Vector3 r2)
    {
        if (!CheckMinMax(p1, q1, r1, p2, q2, r2))
            return false;

        Vector3 e1 = q1 - p1;
        Vector3 e2 = r1 - q1;
        Vector3 e3 = p1 - r1;
        Vector3 n1 = Vector3.Cross(e1, e2);
        Vector3 f1 = p2 - p1;
        if (Vector3.Dot(n1, f1) > 0.0f)
            return false;

        Vector3 e4 = q2 - p2;
        Vector3 e5 = r2 - q2;
        Vector3 e6 = p2 - r2;
        Vector3 n2 = Vector3.Cross(e4, e5);
        Vector3 f2 = p1 - p2;
        if (Vector3.Dot(n2, f2) > 0.0f)
            return false;

        if (CcwTriTriIntersection2D(new Vector2(p1.x, p1.y), new Vector2(q1.x, q1.y), new Vector2(r1.x, r1.y),
                                    new Vector2(p2.x, p2.y), new Vector2(q2.x, q2.y), new Vector2(r2.x, r2.y)))
        {
            return true;
        }
        else
        {
            if (CcwTriTriIntersection2D(new Vector2(p1.x, p1.y), new Vector2(q1.x, q1.y), new Vector2(r1.x, r1.y),
                                        new Vector2(p2.x, p2.y), new Vector2(q2.x, q2.y), new Vector2(r2.x, r2.y)))
            {
                return true;
            }
            else
            {
                if (CcwTriTriIntersection2D(new Vector2(p1.x, p1.y), new Vector2(q1.x, q1.y), new Vector2(r1.x, r1.y),
                                            new Vector2(p2.x, p2.y), new Vector2(q2.x, q2.y), new Vector2(r2.x, r2.y)))
                {
                    return true;
                }
                else
                {
                    Vector3 N1 = Vector3.Cross(e1, e2);
                    float d1 = -Vector3.Dot(N1, p1);
                    float du0 = Vector3.Dot(N1, p2) + d1;
                    float du1 = Vector3.Dot(N1, q2) + d1;
                    float du2 = Vector3.Dot(N1, r2) + d1;

                    if (du0 * du1 > 0.0f && du0 * du2 > 0.0f)
                        return false;

                    Vector3 N2 = Vector3.Cross(e4, e5);
                    float d2 = -Vector3.Dot(N2, p2);
                    float dv0 = Vector3.Dot(N2, p1) + d2;
                    float dv1 = Vector3.Dot(N2, q1) + d2;
                    float dv2 = Vector3.Dot(N2, r1) + d2;

                    if (dv0 * dv1 > 0.0f && dv0 * dv2 > 0.0f)
                        return false;

                    return true;
                }
            }
        }
    }
}
