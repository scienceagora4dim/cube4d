using UnityEngine;
using System.Collections;
using Parsers;

namespace cube4d.hobj
{
    /// <summary>
    /// 4次元オブジェクト解析器生成クラス
    /// </summary>
    public static class HigherObjectParsers
    {
        /// <summary>
        /// Nodeの型の定義
        /// </summary>
        public enum NodeType : int
        {
            Root = 1,
            Object,
            Vertex,
            Facet,
            Name,
            Number,
            Index
        }

        /// <summary>
        /// 改行Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> EndOfLineChars(this ParserBuilder<char> builder)
        {
            return builder
                .Choice()
                    .Ch('\n')
                    .Begin()
                        .Ch('\r')
                        .Optional()
                            .Ch('\n')
                        .End()
                    .End()
                .End();
        }

        /// <summary>
        /// 空白Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> WhiteSpace(this ParserBuilder<char> builder)
        {
            return builder
                .Set(' ', '\t', '\f', '\v');
        }

        /// <summary>
        /// 複数空白Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> WhiteSpaces(this ParserBuilder<char> builder)
        {
            return builder
                .OneOrMore()
                    .WhiteSpace()
                .End();
        }

        /// <summary>
        /// 行末Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> EofOrEol(this ParserBuilder<char> builder)
        {
            return builder
                .Choice()
                    .Empty()
                    .EndOfLineChars()
                .End();
        }

        /// <summary>
        /// コメントParser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> Comment(this ParserBuilder<char> builder)
        {
            return builder
                .Begin()
                    .Ch('#')
                        .ZeroOrMore()
                            .NotExist()
                                .EofOrEol()
                            .End()
                            .Any()
                        .End()
                    .EofOrEol()
                .End();
        }

        /// <summary>
        /// コメント・ファイル終端を含む行末Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> LineEnd(this ParserBuilder<char> builder)
        {
            return builder
                .Choice()
                    .Comment()
                    .EofOrEol()
                .End();
        }

        /// <summary>
        /// 空白Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> Space(this ParserBuilder<char> builder)
        {
            return builder
                .Choice()
                    .Comment()
                    .WhiteSpaces()
                    .EndOfLineChars()
                .End();
        }

        /// <summary>
        /// 識別子Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> Identifier(this ParserBuilder<char> builder)
        {
            return builder
                .Begin()
                    .Choice()
                        .Ch('_')
                        .Range('a', 'z')
                        .Range('A', 'Z')
                    .End()
                    .ZeroOrMore()
                        .Choice()
                            .Ch('_')
                            .Range('a', 'z')
                            .Range('A', 'Z')
                            .Range('0', '9')
                        .End()
                    .End()
                .End();
        }

        /// <summary>
        /// 数値Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> Number(this ParserBuilder<char> builder)
        {
            return builder
                .Node((int)NodeType.Number)
                    .Optional()
                        .Set('+', '-')
                    .End()
                    .OneOrMore()
                        .Range('0', '9')
                    .End()
                    .Optional()
                        .Begin()
                            .Ch('.')
                            .OneOrMore()
                                .Range('0', '9')
                            .End()
                        .End()
                    .End()
                .End();
        }

        /// <summary>
        /// インデックス値Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> Index(this ParserBuilder<char> builder)
        {
            return builder
                .Node((int)NodeType.Index)
                    .Choice()
                        .Begin()
                            .Range('1', '9')
                            .ZeroOrMore()
                                .Range('0', '9')
                            .End()
                        .End()
                        .Ch('0')
                    .End()
                .End();
        }

        /// <summary>
        /// 頂点Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> Vertex(this ParserBuilder<char> builder)
        {
            return builder
                .Node((int)NodeType.Vertex)
                    .Optional()
                        .WhiteSpaces()
                    .End()
                    .Set('v', 'V')
                    .WhiteSpaces()
                    .Number()
                    .WhiteSpaces()
                    .Number()
                    .WhiteSpaces()
                    .Number()
                    .WhiteSpaces()
                    .Number()
                    .Optional()
                        .WhiteSpaces()
                    .End()
                    .LineEnd()
                .End();
        }

        /// <summary>
        /// FacetParser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> Facet(this ParserBuilder<char> builder)
        {
            return builder
                .Node((int)NodeType.Facet)
                    .Optional()
                        .WhiteSpaces()
                    .End()
                    .Set('f', 'F')
                    .WhiteSpaces()
                    .Index()
                    .WhiteSpaces()
                    .Index()
                    .WhiteSpaces()
                    .Index()
                    .Optional()
                        .WhiteSpaces()
                    .End()
                    .LineEnd()
                .End();
        }

        /// <summary>
        /// Object定義HeaderParser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> ObjectHeader(this ParserBuilder<char> builder)
        {
            return builder
                .Begin()
                    .Optional()
                        .WhiteSpaces()
                    .End()
                    .Set('o', 'O')
                    .WhiteSpaces()
                    .Node((int)NodeType.Name)
                        .Identifier()
                    .End()
                    .Optional()
                        .WhiteSpaces()
                    .End()
                    .LineEnd()
                .End();
        }

        /// <summary>
        /// Object定義Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> ObjectDefinition(this ParserBuilder<char> builder)
        {
            return builder
                .Node((int)NodeType.Object)
                    .ZeroOrMore()
                        .Space()
                    .End()
                    .ObjectHeader()
                    .OneOrMore()
                        .ZeroOrMore()
                            .Space()
                        .End()
                        .Vertex()
                    .End()
                    .OneOrMore()
                        .ZeroOrMore()
                            .Space()
                        .End()
                        .Facet()
                    .End()
                .End();
        }

        /// <summary>
        /// Object定義Parser
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public static ParserBuilder<char> Objects(this ParserBuilder<char> builder)
        {
            return builder
                .Node((int)NodeType.Root)
                    .ZeroOrMore()
                        .ObjectDefinition()
                    .End()
                    .ZeroOrMore()
                        .Space()
                    .End()
                    .Empty()
                .End();
        }
    }
}
