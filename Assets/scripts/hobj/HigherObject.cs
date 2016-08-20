using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace cube4d.hobj
{
    /// <summary>
    /// 4次元オブジェクトクラス
    /// </summary>
    public class HigherObject
    {
        /// <summary>
        /// 4次元頂点
        /// </summary>
        public struct Vertex
        {
            /// <summary>
            /// 座標を指定して生成する
            /// </summary>
            /// <param name="x">X座標</param>
            /// <param name="y">Y座標</param>
            /// <param name="z">Z座標</param>
            /// <param name="w">W座標</param>
            public Vertex(double x, double y, double z, double w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            /// <summary>
            /// X座標
            /// </summary>
            public double x;

            /// <summary>
            /// Y座標
            /// </summary>
            public double y;

            /// <summary>
            /// Z座標
            /// </summary>
            public double z;

            /// <summary>
            /// W座標
            /// </summary>
            public double w;
        }

        /// <summary>
        /// 面
        /// </summary>
        public struct Facet
        {
            /// <summary>
            /// 頂点インデックスを指定して生成する
            /// </summary>
            /// <param name="a">頂点インデックス</param>
            /// <param name="b">頂点インデックス</param>
            /// <param name="c">頂点インデックス</param>
            public Facet(int a, int b, int c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            /// <summary>
            /// 頂点インデックス
            /// </summary>
            public int a;

            /// <summary>
            /// 頂点インデックス
            /// </summary>
            public int b;

            /// <summary>
            /// 頂点インデックス
            /// </summary>
            public int c;
        }

        // 頂点配列
        private IList<Vertex> vertices_ = new List<Vertex>();

        // Facet配列
        private IList<Facet> facets_ = new List<Facet>();

        /// <summary>
        /// オブジェクト名
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 頂点の追加
        /// </summary>
        /// <param name="v">頂点</param>
        public void Add(Vertex v)
        {
            vertices_.Add(v);
        }
        
        /// <summary>
        /// Facetの追加
        /// </summary>
        /// <param name="facet">Facet</param>
        public void Add(Facet facet)
        {
            Debug.Assert(0 <= facet.a && facet.a < vertices_.Count);
            Debug.Assert(0 <= facet.b && facet.b < vertices_.Count);
            Debug.Assert(0 <= facet.c && facet.c < vertices_.Count);

            facets_.Add(facet);
        }

        /// <summary>
        /// 頂点の取得
        /// </summary>
        public IEnumerable<Vertex> Vertices
        {
            get
            {
                return vertices_;
            }
        }

        /// <summary>
        /// Facetの取得
        /// </summary>
        public IEnumerable<Facet> Facets
        {
            get
            {
                return facets_;
            }
        }
    }
}
