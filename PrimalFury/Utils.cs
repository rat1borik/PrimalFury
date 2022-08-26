using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace PrimalFury {

    namespace Utils {
        public static class Vectors {
            public static bool Contains(Vector2i container, (Vector2i, Vector2i) item) {
                if (container == null || item.Item1 == null || item.Item2 == null) {
                    throw new ArgumentNullException("Null is not valid value");
                }
                if (Contains(container, item.Item1) && Contains(container, item.Item2)) return true;
                return false;
            }
            public static bool Contains(Vector2i container, Vector2i item) {
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
            public static (Vector2i, Vector2i) Clip(Vector2i container, (Vector2i, Vector2i) item) {
                    Vector2i pt1;
                    Vector2i pt2;

                    if (Contains(container, item.Item1)) {
                        pt1 = item.Item1;
                        pt2 = (Vector2i)Intersections(GetContainerLines(container), item)[0];
                    } else if (Contains(container, item.Item2)) {
                        pt2 = (Vector2i)Intersections(GetContainerLines(container), item)[0];
                        pt1 = item.Item2;
                    } else {
                        var colls = Intersections(GetContainerLines(container), item);
                        if (colls.Count == 2) {
                            pt1 =(Vector2i)colls[0];
                            pt2 =(Vector2i)colls[1];
                        } else {
                            throw new ArgumentException("Cannot clip when both point not in container");
                        }
                    }

                    return (pt1, pt2);
            }
            public static List<(Vector2i, Vector2i)> GetContainerLines(Vector2i container) {
                return new List<(Vector2i, Vector2i)>() {(new Vector2i(0, 0), new Vector2i(0, container.Y)),
                                                                            (new Vector2i(0, container.Y), new Vector2i(container.X, container.Y)),
                                                                            (new Vector2i(container.X, container.Y), new Vector2i(container.X, 0)),
                                                                            (new Vector2i(container.X, 0), new Vector2i(0, 0)) };
            }
            public static int Greatest(params int[] nums) {
                return nums.Max();
            }
            public static int Lowest(params int[] nums) {
                return nums.Min();
            }

            public static bool SignDiff(int i1, int i2) {
                return Math.Sign(i1) != Math.Sign(i2);
            }

            public static bool Intersects((Vector2i, Vector2i) l1 , (Vector2i, Vector2i) l2, bool layIsIntersect = false) {

                var vec1 = ToVector(l1.Item1, l1.Item2);
                var vecCheck1 = ToVector(l1.Item1, l2.Item1);
                var vecCheck2 = ToVector(l1.Item1, l2.Item2);

                int check1 = Cross(vec1, vecCheck1);
                int check2 = Cross(vec1, vecCheck2);

                if (check1 == 0 || check2 == 0) {
                    if (layIsIntersect) {
                        return true;
                    }
                    return false;
                }
                
                return SignDiff(check1, check2);
            }

            public static (Vector2f,bool) Intersection((Vector2i, Vector2i) l1, (Vector2i, Vector2i) l2, bool layIsIntersect = false) {
                var vec1 = ToVector(l1.Item1, l1.Item2);
                var vecCheck1 = ToVector(l1.Item1, l2.Item1);
                var vecCheck2 = ToVector(l1.Item1, l2.Item2);

                int check1 = Cross(vec1, vecCheck1);
                int check2 = Cross(vec1, vecCheck2);

                if (check1 == 0 || check2 == 0 || SignDiff(check1,check2)) {
                    var crossingX = l2.Item1.X + (l2.Item2.X - l2.Item1.X) * Math.Abs(check1) / Math.Abs(check2 - check1);
                    var crossingY = l2.Item1.Y + (l2.Item2.Y - l2.Item1.Y) * Math.Abs(check1) / Math.Abs(check2 - check1);
                    return (new Vector2f(crossingX, crossingY),true);
                }

                return (new Vector2f(),false);
            }

            public static List<Vector2f> Intersections(List<(Vector2i, Vector2i)> list, (Vector2i, Vector2i) line) {
                var result = new List<Vector2f>();
                foreach (var item in list) {
                    var coll = Intersection(item, line);
                    if (coll.Item2 && !result.Exists((el)=>el == coll.Item1)) {
                        result.Add(coll.Item1);
                    }
                }
                return result;
            }
            public static float Length(Vector2i vec) {
                return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
            }
            
            public static Vector2i ToVector(Vector2i v1, Vector2i v2) {
                return v2 - v1;
            }

            public static int Cross(Vector2i v1, Vector2i v2) {
                return v1.X * v2.Y - v1.Y * v2.X;
            }
            public static int Dot(Vector2i v1, Vector2i v2) {
                return v1.X * v2.X + v1.Y * v2.Y;
            }
        }
    }
}
