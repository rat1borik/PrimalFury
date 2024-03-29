﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace PrimalFury {

    namespace Utils {
        namespace MathTools {
            
            // structs
            public struct Intersection {
                public Vector2f PointOfIntersection;
                public bool HasIntersection; // if false signals that no intersection
                public bool IsLaysOnSameVector; // if true signals that lines lays one over one
                public Vector2f Get() {
                    return PointOfIntersection;
                }

                public static Intersection SameVector(bool hasInters = false) {
                    return new Intersection {
                        PointOfIntersection = new Vector2f(),
                        HasIntersection = hasInters,
                        IsLaysOnSameVector = true
                    };
                }

                public static Intersection NoIntersection() {
                    return new Intersection {
                        PointOfIntersection = new Vector2f(),
                        HasIntersection = false,
                        IsLaysOnSameVector = false
                    };
                }

                public static Intersection NewIntersection(Vector2f intrs) {
                    return new Intersection {
                        PointOfIntersection = intrs,
                        HasIntersection = true,
                        IsLaysOnSameVector = false
                    };
                }
                //public bool Equals(Intersection i) {
                //    return this.PointOfIntersection == i.PointOfIntersection
                //        && this.HasIntersection == i.HasIntersection
                //        && this.IsLaysOnSameVector == i.IsLaysOnSameVector;
                //}

                //public static bool operator == (Intersection i1, Intersection i2) {
                //    return i1.PointOfIntersection == i2.PointOfIntersection
                //        && i1.HasIntersection == i2.HasIntersection
                //        && i1.IsLaysOnSameVector == i2.IsLaysOnSameVector;
                //}
                //public static bool operator != (Intersection i1, Intersection i2) {
                //    return i1.PointOfIntersection != i2.PointOfIntersection
                //        || i1.HasIntersection != i2.HasIntersection
                //        || i1.IsLaysOnSameVector != i2.IsLaysOnSameVector;
                //}
            }

            public struct Line {

                // ax+by+c=0
                public float A { get; set; }
                public float B { get; set; }
                public float C { get; set; }

                public Line(Vector2f pt1, Vector2f pt2) {
                    A = pt1.Y - pt2.Y;
                    B = pt2.X - pt1.X;
                    C = pt2.Y * pt1.X - pt1.Y * pt2.X;
                }

                public Line((Vector2f ,Vector2f) cut) {
                    A = cut.Item1.Y - cut.Item2.Y;
                    B = cut.Item2.X - cut.Item1.X;
                    C = cut.Item2.Y * cut.Item1.X - cut.Item1.Y * cut.Item2.X;
                }

                public Intersection GetIntersection(Line l) {

                    var denom = this.A * l.B - l.A * this.B;

                    if (denom == 0) {
                        if (this.A / l.A == this.B / l.B && this.B / l.B == this.C / l.C)
                            return Intersection.SameVector();
                        return Intersection.NoIntersection();
                    }

                    return Intersection.NewIntersection(new Vector2f((float)Math.Round((-this.C * l.B + this.B * l.C) / denom, 4),
                                                           (float)Math.Round((-this.A * l.C + l.A * this.C) / denom, 4)));
                }

                public bool Contains((Vector2f, Vector2f) cut) {
                    return GetIntersection(new Line(cut)).IsLaysOnSameVector;
                }

                public bool Contains(Vector2f pt) {
                    
                    return Math.Round(this.A * pt.X + this.B * pt.Y + this.C, 4) == 0;
                }

                public bool Intersects(Line l) {
                    return GetIntersection(l).HasIntersection;
                }
            }

            // static stuff
            public static class Numbers {
                public static int Greatest(this int i1, params int[] nums) {
                    return nums.Append(i1).Max();
                }

                public static int Lowest(this int i1, params int[] nums) {
                    return nums.Append(i1).Min();
                }

                public static float Greatest(this float i1, params float[] nums) {
                    return nums.Append(i1).Max();
                }

                public static float Lowest(this float i1, params float[] nums) {
                    return nums.Append(i1).Min();
                }

                public static bool SignDiff(this int i1, int i2) {
                    return Math.Sign(i1) != Math.Sign(i2);
                }

                public static bool SignDiff(this float i1, float i2) {
                    return Math.Sign(i1) != Math.Sign(i2);
                }
            }

            public static class Intersectables {

                public static bool Contains(this (Vector2f, Vector2f) container, (Vector2f, Vector2f) cut2) {
                    return new Line(container).GetIntersection(new Line(cut2)).IsLaysOnSameVector
                        && RectContains(container, cut2);
                }

                public static bool Contains(this (Vector2f, Vector2f) container, Vector2f pt) {
                    var p1 = new Line(container).Contains(pt);
                    return p1 && RectContains(container, pt);      
                }
                public static bool RectContains(this (Vector2f, Vector2f) container, Vector2f pt) {
                    if (container.ToVector().X == 0) {
                        var p4 = (container.Item1.Y.Lowest(container.Item2.Y) - pt.Y) <= 0;
                        var p5 = (container.Item1.Y.Greatest(container.Item2.Y) - pt.Y) >= 0;
                        return p4 && p5;
                    } else {
                        var p2 = (container.Item1.X.Lowest(container.Item2.X) - pt.X) <= 0;
                        var p3 = (container.Item1.X.Greatest(container.Item2.X) - pt.X) >= 0;
                        return p2 && p3;
                    }
                }
                public static bool RectContains(this (Vector2f, Vector2f) container, (Vector2f, Vector2f) cut2) {
                    if (container.ToVector().X == 0) {
                        var p4 = (container.Item1.Y.Lowest(container.Item2.Y) - cut2.Item1.Y) <= 0;
                        var p5 = (container.Item1.Y.Greatest(container.Item2.Y) - cut2.Item2.Y) >= 0;
                        return p4 && p5;
                    } else {
                        var p2 = (container.Item1.X.Lowest(container.Item2.X) - cut2.Item1.X) <= 0;
                        var p3 = (container.Item1.X.Greatest(container.Item2.X) - cut2.Item2.X) >= 0;
                        return p2 && p3;
                    }
                }

                public static bool Intersects(this (Vector2f, Vector2f) cut1, (Vector2f, Vector2f) cut2, bool is1vec2cut = false) {
                    Intersection i = new Line(cut1).GetIntersection(new Line(cut2));
                    return i.HasIntersection && (cut1.Contains(i.PointOfIntersection)|| is1vec2cut) && cut2.Contains(i.PointOfIntersection);
                }
                public static Intersection GetIntersection(this (Vector2f, Vector2f) cut1, (Vector2f, Vector2f) cut2, bool is1vec2cut = false) {
                    Intersection i = new Line(cut1).GetIntersection(new Line(cut2));
                    if (i.HasIntersection) {
                        var p1 = is1vec2cut ? true : cut1.RectContains(i.PointOfIntersection);

                        var p2 = cut2.RectContains(i.PointOfIntersection);
                        if (p1 && p2)
                            return i;
                    }
                    return Intersection.NoIntersection();
                }

                public static bool IntersectsVectors(this (Vector2f,Vector2f) line1, (Vector2f, Vector2f) line2) {
                    return new Line(line1).GetIntersection(new Line(line2)).HasIntersection;
                }
                public static Intersection GetIntersectionVectors(this (Vector2f, Vector2f) cut1, (Vector2f, Vector2f) cut2) {
                    return new Line(cut1).GetIntersection(new Line(cut2));
                }

                public static List<Vector2f> Intersections(this List<(Vector2f, Vector2f)> list, (Vector2f, Vector2f) line) {
                    var result = new List<Vector2f>();
                    foreach (var item in list) {
                        var coll = GetIntersection(item, line);
                        if (coll.HasIntersection && !result.Exists((el) => el == coll.Get())) {
                            result.Add(coll.Get());
                        }
                    }
                    return result;
                }
            }

            public static class Containers {
                public static bool Contains(this Vector2f container, (Vector2f, Vector2f) item) {
                    if (container == null || item.Item1 == null || item.Item2 == null) {
                        throw new ArgumentNullException("Null is not valid value");
                    }
                    if (Contains(container, item.Item1) && Contains(container, item.Item2)) return true;
                    return false;
                }
                public static bool Contains(this Vector2f container, Vector2f item) {
                    if (container == null || item == null) {
                        throw new ArgumentNullException("Null is not valid value");
                    }

                    if ((item.X <= container.X
                        && item.X >= 0)
                        && (item.Y <= container.Y
                        && item.Y >= 0
                        )) return true;

                    return false;
                }
                public static List<(Vector2f, Vector2f)> GetContainerLines(this Vector2f container) {
                    return new List<(Vector2f, Vector2f)>() {(new Vector2f(0, 0), new Vector2f(0, container.Y)),
                                                                                (new Vector2f(0, container.Y), new Vector2f(container.X, container.Y)),
                                                                                (new Vector2f(container.X, container.Y), new Vector2f(container.X, 0)),
                                                                                (new Vector2f(container.X, 0), new Vector2f(0, 0)) };
                }

                public static List<Vector2f> GetContainerPoints(this Vector2f container) {
                    return new List<Vector2f>() { new Vector2f(0, 0)
                                                , new Vector2f(0, container.Y)
                                                , new Vector2f(container.X, container.Y)
                                                , new Vector2f(container.X, 0)};
                }

                public static List<(Vector2f, Vector2f)> GetContainerLines(this Vector2f container, Vector2f begCoord) {
                    return new List<(Vector2f, Vector2f)>() {(begCoord, new Vector2f(begCoord.X, container.Y + begCoord.Y)),
                                                                                (new Vector2f(begCoord.X, container.Y + begCoord.Y), new Vector2f(container.X + begCoord.X, container.Y + begCoord.Y)),
                                                                                (new Vector2f(container.X + begCoord.X, container.Y + begCoord.Y), new Vector2f(container.X + begCoord.X, begCoord.Y)),
                                                                                (new Vector2f(container.X + begCoord.X, begCoord.Y), begCoord) };
                }

                public static (Vector2f, Vector2f) Clip(this Vector2f container, (Vector2f, Vector2f) item) {
                    if (!Contains(container, item)) {
                        Vector2f pt1;
                        Vector2f pt2;

                        var inters = GetContainerLines(container).Intersections(item);

                        if (Contains(container, item.Item1)) {
                            pt1 = item.Item1;
                            if (inters.Count > 0) pt2 = inters[0];
                            else pt2 = item.Item2;
                        } else if (Contains(container, item.Item2)) {
                            if (inters.Count > 0) pt2 = inters[0];
                            else pt2 = item.Item1;
                            pt1 = item.Item2;
                        } else {
                            var colls = GetContainerLines(container).Intersections(item);
                            if (colls.Count == 2) {
                                pt1 = (Vector2f)colls[0];
                                pt2 = (Vector2f)colls[1];
                            } else {
                                return item;
                                //throw new ArgumentException("Cannot clip when both point not in container");
                            }
                        }

                        return (pt1, pt2);
                    } else {
                        return item;
                    }
                }
            }
            public static class Radians {
                public static float ToRadians(this float gr) {
                    return (float)(gr * Math.PI / 180);
                }
            }
            public static class Vectors {

                public enum Side {
                    None,
                    Left = 1,
                    Right = -1,
                    OnVector = 0,
                    Intersects = 2
                }
                // legacy
                //public static bool Intersects((Vector2f, Vector2f) l1, (Vector2f, Vector2f) l2, bool layIsIntersect = false) {

                //    // must check in both order

                //    var vec1 = ToVector(l1.Item1, l1.Item2);
                //    var vecCheck1 = ToVector(l1.Item1, l2.Item1);
                //    var vecCheck2 = ToVector(l1.Item1, l2.Item2);

                //    int check1 = Cross(vec1, vecCheck1);
                //    int check2 = Cross(vec1, vecCheck2);

                //    if (!(check1 == 0 || check2 == 0 || check1.SignDiff(check2))) {
                //        return false;
                //    }

                //    vec1 = ToVector(l2.Item1, l2.Item2);
                //    vecCheck1 = ToVector(l2.Item1, l1.Item1);
                //    vecCheck2 = ToVector(l2.Item1, l1.Item2);

                //    check1 = Cross(vec1, vecCheck1);
                //    check2 = Cross(vec1, vecCheck2);

                //    if (!(check1 == 0 || check2 == 0 || check1.SignDiff(check2))) {
                //        return false;
                //    }

                //    if (check1 == 0 || check2 == 0) {
                //        if (check1 == 0 && check2 == 0) {
                //            return false;
                //        }
                //        return layIsIntersect;
                //    }
                //    return check1.SignDiff(check2);
                //}

                //public static Intersection GetIntersection((Vector2f, Vector2f) l1, (Vector2f, Vector2f) l2, bool layIsIntersect = false) {
                //    bool isLayOnLine = false;

                //    // must check in both order

                //    var vec1 = ToVector(l1.Item1, l1.Item2);
                //    var vecCheck1 = ToVector(l1.Item1, l2.Item1);
                //    var vecCheck2 = ToVector(l1.Item1, l2.Item2);

                //    int check1 = Cross(vec1, vecCheck1);
                //    int check2 = Cross(vec1, vecCheck2);

                //    if (!(check1 == 0 || check2 == 0 || check1.SignDiff(check2))) {
                //        return Intersection.NoIntersection();
                //    }

                //    vec1 = ToVector(l2.Item1, l2.Item2);
                //    vecCheck1 = ToVector(l2.Item1, l1.Item1);
                //    vecCheck2 = ToVector(l2.Item1, l1.Item2);

                //    check1 = Cross(vec1, vecCheck1);
                //    check2 = Cross(vec1, vecCheck2);

                //    if (!(check1 == 0 || check2 == 0 || check1.SignDiff(check2))) {
                //        return Intersection.NoIntersection();
                //    }

                //    if (check1 == 0 || check2 == 0) {
                //        isLayOnLine = true;
                //        if (check1 == 0 && check2 == 0) {
                //            return Intersection.SameVector();
                //        }
                //    }

                //    var crossingX = l1.Item1.X + (l1.Item2.X - l1.Item1.X) * Math.Abs(check1) / Math.Abs(check2 - check1);
                //    var crossingY = l1.Item1.Y + (l1.Item2.Y - l1.Item1.Y) * Math.Abs(check1) / Math.Abs(check2 - check1);

                //    if (isLayOnLine&& !layIsIntersect) {
                //        return Intersection.NoIntersection();

                //    }
                //    return new Intersection {
                //        PointOfIntersection = new Vector2f(crossingX, crossingY),
                //        HasIntersection = true,
                //        IsLaysOnSameVector = false
                //    };
                //}

                public static float Length(this Vector2f vec) {
                    return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                }

                public static Vector2f ToVector(this Vector2f v1, Vector2f v2) {
                    return v2 - v1;
                }
                public static Vector2f ToVector(this (Vector2f, Vector2f) v1) {
                    return v1.Item2 - v1.Item1;
                }

                public static float Cross(this Vector2f v1, Vector2f v2) {
                    return v1.X * v2.Y - v1.Y * v2.X;
                }

                public static float Dot(this Vector2f v1, Vector2f v2) {
                    return v1.X * v2.X + v1.Y * v2.Y;
                }

                public static float Dot(this Vector3f v1, Vector3f v2) {
                    return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
                }

                public static Vector2f Rotate(this Vector2f v1, float deg) {
                    deg = deg.ToRadians();
                    return new Vector2f(v1.X * (float)Math.Cos(deg) - v1.Y * (float)Math.Sin(deg)
                                        , v1.X * (float)Math.Sin(deg) + v1.Y * (float)Math.Cos(deg));
                }

                // Rotates counterclockwise 
                public static (Vector2f, Vector2f) Rotate(this (Vector2f, Vector2f) v1, float deg) {
                    deg = deg.ToRadians();
                    var vec = v1.ToVector();
                    return new ValueTuple<Vector2f, Vector2f>(v1.Item1, v1.Item1 + new Vector2f(vec.X * (float)Math.Cos(deg) - vec.Y * (float)Math.Sin(deg)
                                        , vec.X * (float)Math.Sin(deg) + vec.Y * (float)Math.Cos(deg)));
                }

                // -1 - right
                //  0 - on same vector
                //  1 - left
                public static Side PtFace(this (Vector2f, Vector2f) line, Vector2f pt) {
                    return (Side)Math.Sign(line.ToVector().Cross(pt - line.Item1));
                }

                public static Side CutFace(this (Vector2f, Vector2f) line, (Vector2f, Vector2f) line2) {
                    if (Math.Sign(line.ToVector().Cross(line2.Item1 - line.Item1)) != Math.Sign(line.ToVector().Cross(line2.Item2 - line.Item1))) return Side.Intersects;
                    return (Side)Math.Sign(line.ToVector().Cross(line2.Item1 - line.Item1));
                }

                public static Vector2f Mul(this Vector2f src, float[,] matrix) {
                    if (matrix.Length != 4 && matrix.Rank !=2) {
                        throw new ArgumentException("Wrong matrix");
                    }

                    return new Vector2f(src.Dot(new Vector2f(matrix[0,0], matrix[0,1])), src.Dot(new Vector2f(matrix[1, 0], matrix[1, 1])));
                }

                public static Vector2f ReverseMul(this Vector2f src, float[,] matrix) {
                    if (matrix.Length != 4 && matrix.Rank !=2) {
                        throw new ArgumentException("Wrong matrix");
                    }

                    return new Vector2f(src.Dot(new Vector2f(matrix[0,0], matrix[1,0])), src.Dot(new Vector2f(matrix[0, 1], matrix[1, 1])));
                }

                public static Vector3f Mul(this Vector3f src, float[,] matrix) {
                    if (matrix.Length != 9 && matrix.Rank != 2) {
                        throw new ArgumentException("Wrong matrix");
                    }
                    return new Vector3f(
                        src.Dot(new Vector3f(matrix[0, 0], matrix[0, 1], matrix[0, 2])),
                        src.Dot(new Vector3f(matrix[1, 0], matrix[1, 1], matrix[1, 2])),
                        src.Dot(new Vector3f(matrix[2, 0], matrix[2, 1], matrix[2, 2])));
                }

                public static Vector2f GetVecByBasis(this Vector2f src, (Vector2f, Vector2f) basis) {
                    return src.Mul(new float[2, 2] {
                        {basis.Item1.X, basis.Item1.Y},
                        {basis.Item2.X, basis.Item2.Y},
                    });
                }

                public static Vector2f GetVecByOrtoBasis(this Vector2f src, Vector2f basisY)
                {
                    return GetVecByBasis(src, (basisY, basisY.Rotate(-90)));
                }

                public static Vector2f RestoreVecByBasis(this Vector2f src, (Vector2f, Vector2f) basis)
                {
                    return src.ReverseMul(new float[2, 2] {
                        {basis.Item1.X, basis.Item1.Y},
                        {basis.Item2.X, basis.Item2.Y},
                    }.Reverse());
                }

                public static Vector3f GetVecByBasis(this Vector3f src, (Vector2f, Vector2f) basis) {
                    return src.Mul(new float[3, 3] {
                        {basis.Item1.X, basis.Item1.Y, 0},
                        {basis.Item2.X, basis.Item2.Y ,0},
                        {0, 0, 1}
                    });
                }

                public static float Length2D(this Vector3f vec) {
                    return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);

                }
                public static float Length(this Vector3f vec) {
                    return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
                }

                public static float Cos2V(this Vector2f v1, Vector2f v2) {
                    return v1.Dot(v2) / (v1.Length() * v2.Length());
                }

                public static (Vector2f, float) Normalize(this Vector2f v) {
                    //if (v.Length() == 0) throw new ArgumentException("Vector length must not be 0");
                    return (new Vector2f(v.Length() == 0 ? 0 : v.X / v.Length(), v.Length() == 0 ? 0 : v.Y / v.Length()), v.Length());
                }

                public static Vector2f GetSausageDistance(this Vector2f pt, (Vector2f, Vector2f) vec) {
                    var basis = (vec.ToVector().Normalize().Item1, vec.ToVector().Normalize().Item1.Rotate(90));
                    var basedPt = (pt-vec.Item1).GetVecByBasis(basis);
                    if (basedPt.X < 0) {
                        return (vec.Item1 - pt).RestoreVecByBasis(basis);
                    } else if (basedPt.X > vec.ToVector().Length()) {
                        return (vec.Item2 - pt).RestoreVecByBasis(basis);
                    }

                    return new Vector2f(0, -basedPt.Y).RestoreVecByBasis(basis);
                }
            }

            public static class Matrixes {
                public static float Determinant(this float[,] m) {
                    if (m.GetLength(0) != m.GetLength(1)) {
                        throw new ArgumentException("Non square matrix");
                    }

                    var size = m.GetLength(0);
                    float acc;
                    
                    if (size == 1) return m[0, 0];

                    if (size == 2) return (m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0]);

                    acc = 0;
                    for (int i = 0; i < size; i++)
                    {
                        acc = acc + m[0, i] * AlgExt(m, 0, i);
                    }
                    return acc;
                }
                public static float AlgExt(this float[,] m, int row, int col) {
                    if (m.GetLength(0) != m.GetLength(1)) {
                        throw new ArgumentException("Non square matrix");
                    }

                    var size = m.GetLength(0);

                    float[,] temp = new float[size - 1, size - 1];
                    for (int i = 0; i < size - 1; i++) { 
                        for (int j = 0; j < size - 1; j++) {
                            temp[i, j] = m[i >= row ? i + 1 : i, j >= col ? j + 1 : j];
                        }
                    }

                    return ((row + col) % 2 == 0 ? 1 : -1) * temp.Determinant();
                }

                public static float[,] Reverse(this float[,] m){
                    if (m.GetLength(0) != m.GetLength(1)){
                        throw new ArgumentException("Non square matrix");
                    }

                    var size = m.GetLength(0);
                    float[,] res = new float[m.GetLength(0), m.GetLength(0)];

                    var dt = m.Determinant();

                    for (int i = 0; i < size; i++)
                        for (int j = 0; j < size; j++)
                            // Transponed matrix
                            res[i, j] = AlgExt(m, j, i) / dt;

                    return res;
                }
            }
        }
    }
}