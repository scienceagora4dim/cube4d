using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsers.Tests
{
    /// <summary>
    /// ParserBuilderのテスト
    /// </summary>
    [TestClass()]
    public class ParserBuilderTests
    {
        /// <summary>
        /// 何も生成しない場合のテスト
        /// </summary>
        [TestMethod()]
        public void BuildNullTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();

            // 必ず失敗する
            Assert.IsFalse(builder.Build().Parse(new StringContext("test")));
            Assert.IsFalse(builder.Build().Parse(new StringContext("")));
        }

        /// <summary>
        /// FailParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildFailTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Fail().Build();

            // 必ず失敗する
            Assert.IsFalse(parser.Parse(new StringContext("test")));
            Assert.IsFalse(parser.Parse(new StringContext("")));
        }

        /// <summary>
        /// EmptyParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildEmptyTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Empty().Build();

            // 空の場合に成功する
            Assert.IsFalse(parser.Parse(new StringContext("test")));
            Assert.IsTrue(parser.Parse(new StringContext("")));
        }

        /// <summary>
        /// AnyParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildAnyTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Any().Build();

            // 1文字存在する場合に成功する
            Assert.IsTrue(parser.Parse(new StringContext("a")));
            Assert.IsTrue(parser.Parse(new StringContext("bc")));
            Assert.IsTrue(parser.Parse(new StringContext(" c")));
            Assert.IsFalse(parser.Parse(new StringContext("")));
        }

        /// <summary>
        /// CharParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildCharTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Ch('t').Build();

            // 指定文字の場合に成功する
            Assert.IsFalse(parser.Parse(new StringContext("eest")));
            Assert.IsTrue(parser.Parse(new StringContext("test")));
        }

        /// <summary>
        /// StringParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildStringTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Str("test").Build();

            // 指定文字列の場合に成功する
            Assert.IsFalse(parser.Parse(new StringContext("eest")));
            Assert.IsTrue(parser.Parse(new StringContext("test")));
        }

        /// <summary>
        /// RangeParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildRangeTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Range('0', '9').Build();

            // 指定文字範囲の場合に成功する
            Assert.IsFalse(parser.Parse(new StringContext("test")));
            Assert.IsTrue(parser.Parse(new StringContext("0est")));
            Assert.IsTrue(parser.Parse(new StringContext("9est")));
        }

        /// <summary>
        /// SetParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildSetTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Set('a', 'b', 'c').Build();

            // 指定文字集合の場合に成功する
            Assert.IsFalse(parser.Parse(new StringContext("test")));
            Assert.IsTrue(parser.Parse(new StringContext("aest")));
            Assert.IsTrue(parser.Parse(new StringContext("best")));
            Assert.IsTrue(parser.Parse(new StringContext("cest")));
        }

        /// <summary>
        /// ExistParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildExistTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Exist().Ch('t').Build();

            // 後続ParserがTrueの場合に成功する
            Context<char> context = new StringContext("eest");
            Assert.IsFalse(parser.Parse(context));
            Assert.AreEqual(0, context.Position);

            context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(0, context.Position);
        }

        /// <summary>
        /// NotExistParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildNotExistTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.NotExist().Ch('t').Build();

            // 後続ParserがTrueの場合に成功する
            Context<char> context = new StringContext("eest");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(0, context.Position);

            context = new StringContext("test");
            Assert.IsFalse(parser.Parse(context));
            Assert.AreEqual(0, context.Position);
        }

        /// <summary>
        /// OptionalParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildOptionalTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Optional().Ch('t').Build();

            // 後続ParserがTrueでもFalseでも成功する
            Context<char> context = new StringContext("eest");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(0, context.Position);

            context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);
        }

        /// <summary>
        /// ZeroOrMoreParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildZeroOrMorelTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.ZeroOrMore().Ch('t').Build();

            // 後続ParserがTrueでもFalseでも成功する
            Context<char> context = new StringContext("eest");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(0, context.Position);

            context = new StringContext("tttest");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
        }

        /// <summary>
        /// OneOrMoreParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildOneOrMorelTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.OneOrMore().Ch('t').Build();

            // 後続ParserがFalseでは失敗する
            Context<char> context = new StringContext("eest");
            Assert.IsFalse(parser.Parse(context));
            Assert.AreEqual(0, context.Position);

            context = new StringContext("tttest");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
        }

        /// <summary>
        /// SequenceParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildSequenceTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Begin().Ch('t').Ch('e').Build();

            // 後続ParserがTrueの場合に成功する
            Context<char> context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(2, context.Position);

            context = new StringContext("tast");
            Assert.IsFalse(parser.Parse(context));
            Assert.AreEqual(0, context.Position);
        }

        /// <summary>
        /// ChoiceParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildChoiceTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Choice().Ch('t').Ch('e').Build();

            // 後続ParserがTrueの場合に成功する
            Context<char> context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            context = new StringContext("east");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            context = new StringContext("aast");
            Assert.IsFalse(parser.Parse(context));
            Assert.AreEqual(0, context.Position);
        }

        /// <summary>
        /// NodeParserのテスト
        /// </summary>
        [TestMethod()]
        public void BuildNodeTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = builder.Node(100).Ch('t').Build();

            // 後続ParserがTrueの場合に成功する
            Context<char> context = new StringContext("eest");
            Assert.IsFalse(parser.Parse(context));
            Assert.AreEqual(0, context.Position);
            Assert.AreEqual(0, context.Nodes.Count());

            context = new StringContext("test");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);
            Assert.AreEqual(1, context.Nodes.Count());
            Assert.AreEqual(new Node(100, 0, 1), context.Nodes.First());
        }
    }
}