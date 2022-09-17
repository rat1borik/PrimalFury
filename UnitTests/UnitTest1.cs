using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using PrimalFury;
using System.Collections.Generic;
using System.Linq;

namespace PrimalFury.Tests {
    [TestClass()]
    public class BSPTreeTest {
        
        private const string Expected1 = "123";
        [TestMethod()]
        public void SimpleOneNodeTest() {
            var node1 = new BSPNode<int>(123);
            string travRes = "";

            foreach (int leaf in node1.Traverse((x, y) => { return true; }))
                travRes += leaf.ToString();

            Assert.AreEqual(Expected1, travRes);
        }

        private const string Expected2 = "1231264";
        [TestMethod()]
        public void TwoNodeTest() {
            var nodelev1left = new BSPNode<int>(123);
            var nodelev1right = new BSPNode<int>(64);
            var nodelev2 = new BSPNode<int>(12, nodelev1left, nodelev1right);

            string travRes = "";

            foreach (int leaf in nodelev2.Traverse((x, y) => x > y))
                travRes += leaf.ToString();

            Assert.AreEqual(Expected2, travRes);
        }

        private const string Expected3 = "grasshopper rose bumblebee butterfly";
        [TestMethod()]
        public void HardTest() {
            var nodelev1left = new BSPNode<string>("rose");
            var nodelev1right = new BSPNode<string>("butterfly");
            var nodelev2left = new BSPNode<string>("bumblebee", nodelev1left, nodelev1right);
            var nodelev3 = new BSPNode<string>("grasshopper", nodelev2left, null);


            var travRes = new List<string>();

            foreach (string leaf in nodelev3.Traverse((left, right) => left?.Length < right?.Length)) {
                travRes.Add(leaf.ToString());
            }

            Assert.AreEqual(Expected3, string.Join(" ", travRes));
        }
    }
}