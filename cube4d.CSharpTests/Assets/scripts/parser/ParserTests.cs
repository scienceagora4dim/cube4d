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
    public class ParsersTests
    {
        /// <summary>
        /// 空ソース判定のテスト
        /// </summary>
        [TestMethod()]
        public void EmptyParserTest()
        {
            Assert.IsTrue(new EmptyParser<char>().Parse(new StringContext("")));
            Assert.IsFalse(new EmptyParser<char>().Parse(new StringContext("test")));
        }

        /// <summary>
        /// 任意の1文字マッチのテスト
        /// </summary>
        [TestMethod()]
        public void AnyParserTest()
        {
            StringContext context = new StringContext("test");
            Parser<char> parser = new AnyParser<char>();
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 1文字マッチのテスト
        /// </summary>
        [TestMethod()]
        public void CharParserTest()
        {
            StringContext context = new StringContext("test");
            Assert.IsTrue(new CharParser<char>('t').Parse(context));
            Assert.IsTrue(new CharParser<char>('e').Parse(context));
            Assert.IsTrue(new CharParser<char>('s').Parse(context));
            Assert.IsTrue(new CharParser<char>('t').Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 文字列マッチのテスト
        /// </summary>
        [TestMethod()]
        public void StringParserTest()
        {
            StringContext context = new StringContext("test");
            Assert.IsTrue(new StringParser<char>("te").Parse(context));
            Assert.IsTrue(new StringParser<char>("st").Parse(context));
            Assert.IsTrue(context.IsEmpty);
            
            context = new StringContext("test");
            Assert.IsFalse(new StringParser<char>("tesp").Parse(context));
            Assert.IsTrue(new StringParser<char>("test").Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("test");
            Assert.IsFalse(new StringParser<char>("testa").Parse(context));
            Assert.IsTrue(new StringParser<char>("test").Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("");
            Assert.IsFalse(new StringParser<char>("test").Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 文字範囲マッチのテスト
        /// </summary>
        [TestMethod()]
        public void RangeParserTest()
        {
            StringContext context = new StringContext("test");
            Assert.IsTrue(new RangeParser<char>('a', 'z').Parse(context));
            Assert.IsTrue(new StringParser<char>("est").Parse(context));

            context = new StringContext("test");
            Assert.IsFalse(new RangeParser<char>('A', 'Z').Parse(context));
            Assert.IsTrue(new StringParser<char>("test").Parse(context));

            // 範囲未満・範囲より大のどちらでもfalse
            context = new StringContext("B");
            Assert.IsFalse(new RangeParser<char>('A', 'A').Parse(context));
            Assert.IsFalse(new RangeParser<char>('C', 'C').Parse(context));
            Assert.IsTrue(new RangeParser<char>('B', 'B').Parse(context));

            // 範囲の上限・下限どちらでもtrue
            Assert.IsTrue(new RangeParser<char>('A', 'B').Parse(new StringContext("B")));
            Assert.IsTrue(new RangeParser<char>('B', 'C').Parse(new StringContext("B")));

            context = new StringContext("");
            Assert.IsFalse(new RangeParser<char>('A', 'Z').Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 文字集合マッチのテスト
        /// </summary>
        [TestMethod()]
        public void SetParserTest()
        {
            StringContext context = new StringContext("test");
            Assert.IsTrue(new SetParser<char>('t', 'e', 's').Parse(context));
            Assert.IsTrue(new SetParser<char>('t', 'e', 's').Parse(context));
            Assert.IsTrue(new SetParser<char>('t', 'e', 's').Parse(context));
            Assert.IsTrue(new SetParser<char>('t', 'e', 's').Parse(context));
            Assert.IsFalse(new SetParser<char>('t', 'e', 's').Parse(context));
            Assert.IsTrue(context.IsEmpty);
            
            Assert.IsFalse(new SetParser<char>('a', 'b', 'c').Parse(new StringContext("test")));

            context = new StringContext("");
            Assert.IsFalse(new SetParser<char>('a', 'b', 'c').Parse(context));
            Assert.IsTrue(context.IsEmpty);
            Assert.IsFalse(new SetParser<char>().Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 存在確認Parserのテスト
        /// </summary>
        [TestMethod()]
        public void ExistParserTest()
        {
            StringContext context = new StringContext("test");
            Assert.IsTrue(new ExistParser<char>(new CharParser<char>('t')).Parse(context));
            Assert.IsFalse(new ExistParser<char>(new CharParser<char>('e')).Parse(context));
            Assert.IsTrue(new StringParser<char>("test").Parse(context));
        }

        /// <summary>
        /// 非存在確認Parserのテスト
        /// </summary>
        [TestMethod()]
        public void NotExistParserTest()
        {
            StringContext context = new StringContext("test");
            Assert.IsFalse(new NotExistParser<char>(new CharParser<char>('t')).Parse(context));
            Assert.IsTrue(new NotExistParser<char>(new CharParser<char>('e')).Parse(context));
            Assert.IsTrue(new StringParser<char>("test").Parse(context));
        }

        /// <summary>
        /// オプションParserのテスト
        /// </summary>
        [TestMethod()]
        public void OptionalParserTest()
        {
            Parser<char> parser = new OptionalParser<char>(new CharParser<char>('t'));
            StringContext context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            // 成功するが位置は進まない
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);
        }

        /// <summary>
        /// 0回以上繰り返しParserのテスト
        /// </summary>
        [TestMethod()]
        public void ZeroOrMoreParserTest()
        {
            Parser<char> parser = new ZeroOrMoreParser<char>(new CharParser<char>('t'));
            StringContext context = new StringContext("tttest");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);

            // 成功するが位置は進まない
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
        }

        /// <summary>
        /// 1回以上繰り返しParserのテスト
        /// </summary>
        [TestMethod()]
        public void OneOrMoreParserTest()
        {
            Parser<char> parser = new OneOrMoreParser<char>(new CharParser<char>('t'));
            StringContext context = new StringContext("tttest");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
            Assert.IsFalse(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
        }

        /// <summary>
        /// 連接Parserのテスト
        /// </summary>
        [TestMethod()]
        public void SequenceParserTest()
        {
            // tとeの連接
            Parser<char> parser = new SequenceParser<char>(new CharParser<char>('t'), new CharParser<char>('e'));

            StringContext context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("st").Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("tset");
            Assert.IsFalse(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("tset").Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 選択Parserのテスト
        /// </summary>
        [TestMethod()]
        public void ChoiceParserTest()
        {
            // tとeの選択
            Parser<char> parser = new ChoiceParser<char>(new CharParser<char>('t'), new CharParser<char>('e'));

            StringContext context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(parser.Parse(context));
            Assert.IsFalse(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("st").Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("etst");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(parser.Parse(context));
            Assert.IsFalse(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("st").Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// NodeParserのテスト
        /// </summary>
        [TestMethod()]
        public void NodeParserTest()
        {
            Parser<Char> parser = new NodeParser<char>(100, new SequenceParser<char>(
                    new CharParser<char>('t'),
                    new NodeParser<char>(200, new CharParser<char>('e')),
                    new CharParser<char>('s')
                )
            );

            StringContext context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));

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

        /// <summary>
        /// NodeParserのBacktrack時のテスト
        /// </summary>
        [TestMethod()]
        public void NodeParserBacktrackTest()
        {
            Parser<Char> parser = new ChoiceParser<char>(
                new NodeParser<char>(100, new StringParser<char>("tess")),
                new NodeParser<char>(200, new StringParser<char>("test")));

            StringContext context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));

            Assert.AreEqual(context.Position, 4);
            Assert.AreEqual(context.Nodes.Count(), 1);

            Node node = context.Nodes.First();
            Assert.AreEqual(node.Tag, 200);
            Assert.AreEqual(node.Begin, 0);
            Assert.AreEqual(node.End, 4);
            Assert.AreEqual(node.Children.Count(), 0);
        }
    }
}