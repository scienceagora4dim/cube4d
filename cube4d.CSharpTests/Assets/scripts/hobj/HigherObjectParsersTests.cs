using Microsoft.VisualStudio.TestTools.UnitTesting;
using cube4d.hobj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parsers;

namespace cube4d.hobj.Tests
{
    /// <summary>
    /// 4次元オブジェクト解析のテスト
    /// </summary>
    [TestClass()]
    public class HigherObjectParsersTests
    {
        /// <summary>
        /// 改行の解析テスト
        /// </summary>
        [TestMethod()]
        public void EndOfLineTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.EndOfLineChars(builder).Build();

            Context<char> context = new StringContext("\r");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            context = new StringContext("\n");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            context = new StringContext("\r\n");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(2, context.Position);

            context = new StringContext("\n\r");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);
        }

        /// <summary>
        /// 空白の解析テスト
        /// </summary>
        [TestMethod()]
        public void WhiteSpaceTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.WhiteSpace(builder).Build();

            Context<char> context = new StringContext(" \t\v\fabc");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(2, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(4, context.Position);
            Assert.IsFalse(parser.Parse(context));
            Assert.AreEqual('a', context.Current);
        }

        /// <summary>
        /// コメントの解析テスト
        /// </summary>
        [TestMethod()]
        public void CommentTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.Comment(builder).Build();

            Context<char> context = new StringContext("#abc\r\n#test");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(6, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 空白の解析テスト
        /// </summary>
        [TestMethod()]
        public void SpaceTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.Space(builder).Build();

            Context<char> context = new StringContext("  #abc\r\n#test  \r\nabc");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(2, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(8, context.Position);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(17, context.Position);
            Assert.IsFalse(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("abc").Parse(context));
            Assert.IsTrue(context.IsEmpty);
        }

        /// <summary>
        /// 識別子の解析テスト
        /// </summary>
        [TestMethod()]
        public void IdentifierTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.Identifier(builder).Build();

            Context<char> context = new StringContext("a");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            context = new StringContext("1a");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("a1");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(2, context.Position);

            context = new StringContext("_1a");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);

            context = new StringContext("a_1");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);

            context = new StringContext("z_9");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);

            context = new StringContext("A_9");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);

            context = new StringContext("Z_9");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);
        }

        /// <summary>
        /// 数値の解析テスト
        /// </summary>
        [TestMethod()]
        public void NumberTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.Number(builder).Build();

            Context<char> context = new StringContext("0");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            context = new StringContext("a");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("10");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(2, context.Position);

            context = new StringContext("+10");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);

            context = new StringContext("-10");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(3, context.Position);

            context = new StringContext("10.1");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(4, context.Position);

            context = new StringContext("10.12");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(5, context.Position);

            context = new StringContext("+10.12");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(6, context.Position);

            context = new StringContext("-10.12");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(6, context.Position);

            context = new StringContext("-.12");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("-a");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("-");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("+");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext(".");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext(".1");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("+ 0");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("- 0");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("1. 0");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);
        }

        /// <summary>
        /// インデックス数値の解析テスト
        /// </summary>
        [TestMethod()]
        public void IndexTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.Index(builder).Build();

            Context<char> context = new StringContext("0");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            context = new StringContext("a");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("10");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(2, context.Position);

            context = new StringContext("99990");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(5, context.Position);

            context = new StringContext("+10");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("-10");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("10.1");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(2, context.Position);

            context = new StringContext("0123");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(1, context.Position);

            context = new StringContext("1230");
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual(4, context.Position);
        }

        /// <summary>
        /// 頂点定義の解析テスト
        /// </summary>
        [TestMethod()]
        public void VertexTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.Vertex(builder).Build();

            Context<char> context = new StringContext("v 1 -1 1 -1");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("v  1  -1   1  -1");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("v  1  -1   1  -1    ");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("v  1  -1   1  -1    #test  ");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("v  1  -1   1  -1    #test  \nabc");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("abc").Parse(context));

            context = new StringContext("    v  1  -1   1  -1    #test  \nabc");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("abc").Parse(context));
        }

        /// <summary>
        /// Facet定義の解析テスト
        /// </summary>
        [TestMethod()]
        public void FacetTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.Facet(builder).Build();

            Context<char> context = new StringContext("f 1 2 34");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("f  8  0     5");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("f  3  2   3      ");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("f   1   123  2456    #test  ");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("f  2    123  2456    #test  \nabc");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("abc").Parse(context));

            context = new StringContext("    f   1   123  2456    #test  \nabc");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("abc").Parse(context));
        }

        /// <summary>
        /// Object定義ヘッダ部分の解析テスト
        /// </summary>
        [TestMethod()]
        public void ObjectHeaderTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.ObjectHeader(builder).Build();

            Context<char> context = new StringContext("o Hypercube");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("    o Hypercube");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("o Hypercube    ");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(context.IsEmpty);

            context = new StringContext("o Hypercube    abc");
            Assert.IsFalse(parser.Parse(context));

            context = new StringContext("o Hypercube    \r\nabc");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("abc").Parse(context));

            context = new StringContext("   o    Hypercube1234    #test\r\nabc");
            Assert.IsTrue(parser.Parse(context));
            Assert.IsTrue(new StringParser<char>("abc").Parse(context));
        }
        
        /// <summary>
        /// Object定義の解析テスト
        /// </summary>
        [TestMethod()]
        public void ObjectDefinitionTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.ObjectDefinition(builder).Build();

            String source = @"
                o Simplex
                v 1 -1 -1 -1
                v 1 -1 -1 -1
                v 1 -1 -1 -1
                
                f 1 2 3
                f 1 2 3
                f 1 2 3
                f 1 2 3";
            Context<char> context = new StringContext(source);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual("", source.Substring(context.Position));
        }

        /// <summary>
        /// 複数のObject定義の解析テスト
        /// </summary>
        [TestMethod()]
        public void ObjectsTest()
        {
            ParserBuilder<char> builder = new ParserBuilder<char>();
            Parser<char> parser = HigherObjectParsers.Objects(builder).Build();

            String source = @"
                # 1番目
                o Simplex1
                v 1 -1 -1 -1 # すごい頂点
                v 1 -1 -1 -1
                v 1 -1 -1 -1
                
                f 1 2 3 # すごいFacet
                f 1 2 3
                f 1 2 3
                f 1 2 3

                # 2番目
                o Simplex2
                v 1 -1 -1 -1
                v 1 -1 -1 -1
                v 1 -1 -1 -1
                
                f 1 2 3
                f 1 2 3
                f 1 2 3
                f 1 2 3


                # 3番目
                o Simplex3
                v 1 -1 -1 -1
                v 1 -1 -1 -1
                v 1 -1 -1 -1
                
                f 1 2 3
                f 1 2 3
                f 1 2 3
                f 1 2 3

                #test
                ";
            Context<char> context = new StringContext(source);
            Assert.IsTrue(parser.Parse(context));
            Assert.AreEqual("", source.Substring(context.Position));
            
            // Rootが生成されていること
            Assert.AreEqual(1, context.Nodes.Count());
            Assert.AreEqual((int)HigherObjectParsers.NodeType.Root, context.Nodes.First().Tag);

            // Objectが生成されていること
            Node root = context.Nodes.First();
            Assert.AreEqual(3, root.Children.Count());

            Node[] objects = root.Children.ToArray();
            foreach (Node child in objects)
            {
                // 子はすべてオブジェクト定義であること
                Assert.AreEqual((int)HigherObjectParsers.NodeType.Object, child.Tag);

                // 最初の子Nodeはオブジェクト名であること
                Assert.AreEqual((int)HigherObjectParsers.NodeType.Name, child.Children.First().Tag);

                // 次の子Nodeは頂点であること
                Assert.AreEqual((int)HigherObjectParsers.NodeType.Vertex, child.Children.Skip(1).First().Tag);
            }

            // 名称取得の確認
            Node name = objects[0].Children.First();
            Assert.AreEqual("Simplex1", source.Substring(name.Begin, name.Length));
            name = objects[1].Children.First();
            Assert.AreEqual("Simplex2", source.Substring(name.Begin, name.Length));
            name = objects[2].Children.First();
            Assert.AreEqual("Simplex3", source.Substring(name.Begin, name.Length));
        }
    }
}