using cube4d.hobj;
using Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cube4d.hobj
{
    /// <summary>
    /// NodeからHigherObjectを生成する
    /// </summary>
    class HigherObjectBuilder
    {
        /// <summary>
        /// RootNodeから生成する。
        /// </summary>
        /// <param name="source">ソースコード文字列</param>
        /// <param name="rootNode">最上位のNode</param>
        /// <returns>オブジェクト名とHigherObjectのIDictionary</returns>
        public IDictionary<string, HigherObject> Build(string source, Node rootNode)
        {
            Dictionary<string, HigherObject> objects = new Dictionary<string, HigherObject>();

            foreach(Node node in rootNode.Children)
            {
                if(node.Tag == (int)HigherObjectParsers.NodeType.Object)
                {
                    // オブジェクトデータの生成
                    HigherObject obj = BuildObject(source, node);
                    objects.Add(obj.Name, obj);
                }
            }
            return objects;
        }

        /// <summary>
        /// HigherObjectを生成する。
        /// </summary>
        /// <param name="source">ソースコード文字列</param>
        /// <param name="objectNode">HigherObjectのNode</param>
        /// <returns></returns>
        private HigherObject BuildObject(string source, Node objectNode)
        {
            HigherObject obj = new HigherObject();
            foreach(Node node in objectNode.Children)
            {
                switch (node.Tag)
                {
                    case (int)HigherObjectParsers.NodeType.Name:
                        // オブジェクト名の生成
                        obj.Name = source.Substring(node.Begin, node.Length);
                        break;
                    case (int)HigherObjectParsers.NodeType.Vertex:
                        // 頂点データの生成
                        obj.Add(BuildVertex(source, node));
                        break;
                    case (int)HigherObjectParsers.NodeType.Facet:
                        // 面データの生成
                        obj.Add(BuildFacet(source, node));
                        break;
                    default:
                        break;
                }
            }
            return obj;
        }

        /// <summary>
        /// 頂点データの生成
        /// </summary>
        /// <param name="source">ソースコード文字列</param>
        /// <param name="vertexNode">頂点ノード</param>
        /// <returns></returns>
        private HigherObject.Vertex BuildVertex(string source, Node vertexNode)
        {
            HigherObject.Vertex vertex = new HigherObject.Vertex();
            Node[] values = vertexNode.Children.ToArray();
            System.Diagnostics.Debug.Assert(values.Length == 4);
            vertex.x = ParseNumber(source, values[0]);
            vertex.y = ParseNumber(source, values[1]);
            vertex.z = ParseNumber(source, values[2]);
            vertex.w = ParseNumber(source, values[3]);
            return vertex;
        }

        /// <summary>
        /// Facetデータの生成
        /// </summary>
        /// <param name="source">ソースコード文字列</param>
        /// <param name="facetNode">Facetノード</param>
        /// <returns></returns>
        private HigherObject.Facet BuildFacet(string source, Node facetNode)
        {
            HigherObject.Facet facet = new HigherObject.Facet();
            Node[] values = facetNode.Children.ToArray();
            System.Diagnostics.Debug.Assert(values.Length == 3);
            facet.a = ParseIndex(source, values[0]);
            facet.b = ParseIndex(source, values[1]);
            facet.c = ParseIndex(source, values[2]);
            return facet;
        }

        /// <summary>
        /// NumberNodeをdoubleに変換する。
        /// </summary>
        /// <param name="source">ソースコード文字列</param>
        /// <param name="numberNode">NumberNode</param>
        /// <returns></returns>
        private static double ParseNumber(string source, Node numberNode)
        {
            System.Diagnostics.Debug.Assert(numberNode.Tag == (int)HigherObjectParsers.NodeType.Number);
            return double.Parse(source.Substring(numberNode.Begin, numberNode.Length));
        }

        /// <summary>
        /// IndexNodeをintに変換する。
        /// </summary>
        /// <param name="source">ソースコード文字列</param>
        /// <param name="indexNode">IndexNode</param>
        /// <returns></returns>
        private static int ParseIndex(string source, Node indexNode)
        {
            System.Diagnostics.Debug.Assert(indexNode.Tag == (int)HigherObjectParsers.NodeType.Index);
            return int.Parse(source.Substring(indexNode.Begin, indexNode.Length));
        }
    }
}
