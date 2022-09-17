using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PrimalFury {
    public class BSPNode<T> {
        
        // returns true if need to begin traversing from left, false - right
        public delegate bool Comparer(T left, T right);

        BSPNode<T> _left;
        BSPNode<T> _right;
        T _root;

        public BSPNode(T root, BSPNode<T> left, BSPNode<T> right) {
            _left = left;
            _right = right;
            _root = root;
        }

        public BSPNode(T root) {
            _root = root;
        }

        public bool IsLeaf() {
            return _left == null && _right == null;
        }

        public BSPNode<T> Left {
            get { return _left; }
        }

        public BSPNode<T> Right {
            get { return _right; }
        }

        public T Root {
            get { return _root; }
        }
        public IEnumerable<T> Traverse(Comparer c) {
            if (this.IsLeaf()) {
                yield return _root;
                yield break;
            } else {
                if (c.Invoke((T)(_left is null ? default(T) : _left.Root), (T)(_right is null ? default(T) : _right.Root))) {
                    foreach (T res in _left?.Traverse(c) ?? Enumerable.Empty<T>()) {
                        yield return res;
                    }
                    yield return Root;
                    foreach (T res in _right?.Traverse(c) ?? Enumerable.Empty<T>()) {
                        yield return res;
                    }
                    yield break;
                } else {
                    foreach (T res in _right?.Traverse(c) ?? Enumerable.Empty<T>()) {
                        yield return res;
                    }
                    yield return Root;
                    foreach (T res in _left?.Traverse(c) ?? Enumerable.Empty<T>()) {
                        yield return res;
                    }
                    yield break;
                }
            }

        }

        public IEnumerable<(T,int)> TraverseWithDepth(Comparer c, int depth = 0) {
            if (this.IsLeaf()) {
                yield return (Root, depth);
                yield break;
            } else {
                if (c.Invoke((T)(_left is null ? default(T) : _left.Root), (T)(_right is null ? default(T) : _right.Root))) {
                    foreach ((T, int) res in _left?.TraverseWithDepth(c, depth + 1) ?? Enumerable.Empty<(T,int)>()) {
                        yield return res;
                    }
                    yield return (Root, depth);
                    foreach ((T, int) res in _right?.TraverseWithDepth(c, depth + 1) ?? Enumerable.Empty<(T, int)>()) {
                        yield return res;
                    }
                    yield break;
                } else {
                    foreach ((T, int) res in _right?.TraverseWithDepth(c, depth + 1) ?? Enumerable.Empty<(T, int)>()) {
                        yield return res;
                    }
                    yield return (Root, depth);
                    foreach ((T, int) res in _left?.TraverseWithDepth(c, depth + 1) ?? Enumerable.Empty<(T, int)>()) {
                        yield return res;
                    }
                    yield break;
                }
            }
        }

        public string GetView() {

            var res = new List<List<string>>();

            foreach (var (leaf, depth) in this.TraverseWithDepth((x, y) => { return true; })) {
                while (depth >= res.Count) res.Add(new List<string>());
                if (res[depth].Count == 0) res[depth].Add(depth.ToString());
                try { res[depth].Add(leaf.ToString()); }
                catch { return depth.ToString() + " " + res.Count.ToString(); }

            }
            return string.Join("\n", res.ConvertAll(new Converter<List<string>, string>(vl=>string.Join(" ", vl))));
        }
    }
}
