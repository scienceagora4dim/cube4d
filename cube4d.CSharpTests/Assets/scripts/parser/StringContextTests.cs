using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsers.Tests
{
    [TestClass()]
    public class StringContextTests
    {
        /// <summary>
        /// 先頭文字テスト
        /// </summary>
        [TestMethod()]
        public void CurrentTest()
        {
            Assert.AreEqual(new StringContext("t").Current, 't');
        }

        /// <summary>
        /// 次の文字テスト
        /// </summary>
        [TestMethod()]
        public void MoveNextTest()
        {
            StringContext context = new StringContext("te");
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Current, 't');

            context.MoveNext();
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Current, 'e');

            context.MoveNext();
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 位置テスト
        /// </summary>
        [TestMethod()]
        public void PositionTest()
        {
            StringContext context = new StringContext("te");
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Position, 0);

            context.MoveNext();
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Position, 1);

            context.MoveNext();
            Assert.AreEqual(context.Position, 2);
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// バックトラックのテスト
        /// </summary>
        [TestMethod()]
        public void SaveAndBacktrackTest()
        {
            StringContext context = new StringContext("tes");
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Current, 't');

            context.Save();

            context.MoveNext();
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Current, 'e');

            context.MoveNext();
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Current, 's');

            context.Save();
            context.MoveNext();
            Assert.IsTrue(context.IsEmpty);
            context.Shift();
            Assert.IsTrue(context.IsEmpty);

            context.Backtrack();
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Current, 't');

            context.MoveNext();
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Current, 'e');

            context.MoveNext();
            Assert.IsFalse(context.IsEmpty);
            Assert.AreEqual(context.Current, 's');

            context.MoveNext();
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 空かどうか判定するテスト
        /// </summary>
        [TestMethod()]
        public void EmptyTest()
        {
            Assert.IsTrue(new StringContext("").IsEmpty);
            Assert.IsFalse(new StringContext("test").IsEmpty);
        }
        
        /// <summary>
        /// ノード生成のテスト
        /// </summary>
        [TestMethod()]
        public void NodeGeneratorTest()
        {
            StringContext context = new StringContext("test");

            // 親ノード開始
            context.Save(100);

            // 次の文字から子ノード開始
            context.MoveNext();
            context.Save(200);
            context.MoveNext();

            // 子ノード終了
            context.Shift();

            // 親ノード終了
            context.MoveNext();
            context.Shift();

            Assert.AreEqual(context.Position, 3);
            Assert.AreEqual(context.Nodes.Count(), 1);

            Node node = context.Nodes.First();
            Assert.AreEqual(node.Tag, 100);
            Assert.AreEqual(node.Begin, 0);
            Assert.AreEqual(node.End, 3);
            Assert.AreEqual(node.Children.Count(), 1);

            Node child = node.Children.First();
            Assert.AreEqual(child.Tag, 200);
            Assert.AreEqual(child.Begin, 1);
            Assert.AreEqual(child.End, 2);
            Assert.AreEqual(child.Children.Count(), 0);
        }
    }
}