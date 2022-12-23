using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using PrimalFury;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace PrimalFury.Tests {
    [TestClass()]
    public class AnimationTest {
        public static bool SequenceEquals<T>(T[,] a, T[,] b) => a.Rank == b.Rank
            && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());

        [TestMethod()]
        public void AnimGetWithoutStart() {
           var a = new Animation<float[,]>((x) => new float[,] { { 0, 0 }, { 0, 0 } }, 2);

            Assert.ThrowsException<InvalidOperationException>(() => a.Get());
        }

        float[,] res1 = { { 0, 0 }, { 0, 0 } };
        [TestMethod()]
        public void AnimGetNormal() {
            var a = new Animation<float[,]>((x) => res1, 2);
            a.Start();
            Assert.IsTrue(SequenceEquals(a.Get(), res1));
        }

        float[,] res2 = { { 1, 1 }, { 1, 1 } };
        [TestMethod()]
        public void AnimWithElapsed() {
            var a = new Animation<float[,]>((x) => new float[,] { { x, x }, { x, x } }, 1000, false);
            a.Start();
            Thread.Sleep(2000);
            var test = a.Get();
            Assert.IsTrue(SequenceEquals(test, res2));
        }

        float[,] res3 = { { 0, 0 }, { 0, 0 } };
        [TestMethod()]
        public void AnimWithElapsedAndReversable() {
            var a = new Animation<float[,]>((x) => new float[,] { { x, x }, { x, x } }, 1000, false, true);
            a.Start();
            Thread.Sleep(2000);
            var test = a.Get();
            Assert.IsTrue(SequenceEquals(test, res3));
        }

        float[,] res4 = { { 1, 1 }, { 1, 1 } };
        [TestMethod()]
        public void AnimWithElapsedWithLastKnown() {
            var a = new Animation<float[,]>((x) => new float[,] { { x, x }, { x, x } }, 1000, false);
            a.Start();
            Thread.Sleep(1000);
            var test = a.Get();
            Thread.Sleep(2000);
            test = a.Get();
            Assert.IsTrue(SequenceEquals(test, res4));
        }

        float[,] res5 = { { 0, 0 }, { 0, 0 } };
        [TestMethod()]
        public void AnimWithElapsedWithLastKnownAndReversable() {
            var a = new Animation<float[,]>((x) => new float[,] { { x, x }, { x, x } }, 1000, false, true);
            a.Start();
            Thread.Sleep(1000);
            var test = a.Get();
            Thread.Sleep(2000);
            test = a.Get();
            Assert.IsTrue(SequenceEquals(test, res5));
        }

        float[,] res6 = { { 2, 2 }, { 2, 2 } };
        float[,] res7 = { { 3, 3 }, { 3, 3 } };
        [TestMethod()]
        public void AnimValueChanging() {
            var a = new Animation<float[,]>((x) => x < 0.5 ? res6 : res7, 1000);
            a.Start();

            var passed = false;
            var lastknown = a.Get();

            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            while (!passed) {
                Thread.Sleep(50);
                var vl = a.Get();
                passed = !SequenceEquals(vl, lastknown);
                lastknown = vl;
            }

            watcher.Stop();
            if(watcher.ElapsedMilliseconds > 1000) { 
                Assert.Fail();
            }
        }
    }
}