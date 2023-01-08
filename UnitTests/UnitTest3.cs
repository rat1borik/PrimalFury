using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using PrimalFury;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using PrimalFury.Utils.MathTools;
using System.ComponentModel.Design;

namespace PrimalFury.Tests {
    [TestClass()]
    public class MatrixReverseTest {
        public static bool SequenceEquals<T>(T[,] a, T[,] b) => a.Rank == b.Rank
            && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());

        float[,] a = new float[,]{
                { 5,10 },
                { 0,-20 }
        };
        float[,] res = new float[,]{
                { 0.2f,0.1f },
                { 0,-0.05f }
        };
        [TestMethod()]

        public void Reverse2x2() {

            var res2 = a.Reverse();
            Assert.IsTrue(SequenceEquals(res2, res));
        }

    }
}