using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parsers;
using cube4d.hobj;
using UnityEngine;

namespace cube4d.hobj
{
    /// <summary>
    /// ユーティリティメソッドのクラス
    /// </summary>
    public static class HigherObjects
    {
        /// <summary>
        /// ソースコードからHigherObjectを読み込む。
        /// </summary>
        /// <param name="source">ソースコード文字列</param>
        /// <returns>HigherObject辞書</returns>
        public static IDictionary<string, HigherObject> ReadFromSource(string source)
        {
            Parser<char> parser = new ParserBuilder<char>().Objects().Build();
            Context<char> context = new StringContext(source);
            if (!parser.Parse(context))
            {
                throw new FormatException("invalid higher object source.");
            }
            return new HigherObjectBuilder().Build(source, context.Nodes.First());
        }
        
        /// <summary>
        /// HigherObjectからMeshを生成する。
        /// </summary>
        /// <param name="hobj">HigherObject</param>
        /// <returns>HigherObjectから生成したMesh</returns>
        public static Mesh MakeMesh(HigherObject hobj)
        {
            Mesh mesh = new Mesh();

            // 名称設定
            mesh.name = hobj.Name;

            // 頂点配列の設定
            mesh.vertices = hobj.Vertices.Select(v => new Vector3((float)v.x, (float)v.y, (float)v.z)).ToArray();

            // W軸はUVに格納する
            mesh.uv = hobj.Vertices.Select(v => new Vector2((float)v.w, 0.0f)).ToArray();

            // 面の設定
            mesh.triangles = hobj.Facets.SelectMany(f => new int[] { f.a, f.b, f.c }).ToArray();

            // 法線の計算
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
