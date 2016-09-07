using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Parsers
{
    /// <summary>
    /// ノード
    /// </summary>
    public class Node
    {
        public const int NONE = 0;
        private static Node[] EMPTY_CHILDREN = new Node[] { };

        private Node[] children_;

        /// <summary>
        /// タグ値・開始位置・終了位置を指定して生成する
        /// </summary>
        /// <param name="tag">タグ値</param>
        /// <param name="begin">開始位置</param>
        /// <param name="end">終了位置</param>
        /// <param name="children">子ノード。nullは不可</param>
        public Node(int tag, int begin, int end, Node[] children)
        {
            if (children == null)
            {
                throw new ArgumentNullException("children is not nullable");
            }

            this.Tag = tag;
            this.Begin = begin;
            this.End = end;
            this.children_ = children;
        }

        /// <summary>
        /// タグ値・開始位置・終了位置を指定して生成する
        /// </summary>
        /// <param name="tag">タグ値</param>
        /// <param name="begin">開始位置</param>
        /// <param name="end">終了位置</param>
        public Node(int tag, int begin, int end) : this(tag, begin, end, EMPTY_CHILDREN) {}

        /// <summary>
        /// タグ値
        /// </summary>
        public int Tag
        {
            get;
            private set;
        }

        /// <summary>
        /// 始点
        /// </summary>
        public int Begin
        {
            get;
            private set;
        }

        /// <summary>
        /// 終点
        /// </summary>
        public int End
        {
            get;
            private set;
        }

        /// <summary>
        /// ノードの長さ
        /// </summary>
        public int Length
        {
            get
            {
                return End - Begin;
            }
        }

        /// <summary>
        /// 子Nodeを返す。
        /// </summary>
        public IEnumerable<Node> Children
        {
            get
            {
                return children_;
            }
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            Node other = obj as Node;
            if(other == null)
            {
                return false;
            }

            return Tag == other.Tag
                && Begin == other.Begin
                && End == other.End
                && children_.SequenceEqual(other.children_);
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode() + Begin.GetHashCode() + End.GetHashCode() + children_.GetHashCode();
        }

        public override string ToString()
        {
            return "" + Tag + ":[" + Begin + "," + End + ")";
        }
    }

    /// <summary>
    /// 解析結果のノードを訪問するインターフェイス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface NodeVisitor<T>
    {
        /// <summary>
        /// 解析結果のノードを訪問する。
        /// </summary>
        /// <param name="node">訪問対象のノード</param>
        void Visit(Node node);
    }

    /// <summary>
    /// 解析状態の型
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public interface Context<T>
    {
        /// <summary>
        /// 現在の先頭文字を取得する
        /// </summary>
        T Current { get; }

        /// <summary>
        /// 現在位置を返す
        /// </summary>
        int Position { get; }

        /// <summary>
        /// 次の文字に移動する
        /// </summary>
        void MoveNext();

        /// <summary>
        /// 現在位置を記録する
        /// </summary>
        void Save();

        /// <summary>
        /// ノードの開始位置として記録する
        /// </summary>
        void Save(int tag);

        /// <summary>
        /// 記録した位置に戻る
        /// </summary>
        void Backtrack();

        /// <summary>
        /// 直近の記録内容を捨てる
        /// </summary>
        void Shift();

        /// <summary>
        /// 空になったかどうか
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 生成されたNodeを取得する。
        /// </summary>
        IEnumerable<Node> Nodes { get; }
    }

    /// <summary>
    /// ランダムアクセス可能な配列用コンテキスト基本実装クラス。
    /// </summary>
    /// <typeparam name="T">配列の要素の型</typeparam>
    public abstract class ArrayContext<T> : Context<T>
    {
        // 現在位置
        private int position_ = 0;

        // 現在のノード配列
        private List<Node> nodes_ = new List<Node>();

        /// <summary>
        /// 記録位置の構造体
        /// </summary>
        private struct SavePoint
        {
            public int Position;
            public int Tag;
            public int NodePosition;

            /// <summary>
            /// 値をして生成する。
            /// </summary>
            /// <param name="position">現在位置</param>
            /// <param name="tag">開始ノードのタグ</param>
            /// <param name="nodePosition">ノード配列上の現在位置</param>
            public SavePoint(int position, int tag, int nodePosition) : this()
            {
                this.Position = position;
                this.Tag = tag;
                this.NodePosition = nodePosition;
            }
        }

        // バックトラック用スタック
        private Stack<SavePoint> stack_ = new Stack<SavePoint>();

        public T Current
        {
            get
            {
                return GetAt(position_);
            }
        }

        public int Position
        {
            get
            {
                return position_;
            }
        }

        public void Backtrack()
        {
            SavePoint savePoint = stack_.Pop();
            position_ = savePoint.Position;
            nodes_.RemoveRange(savePoint.NodePosition, nodes_.Count - savePoint.NodePosition);
        }

        public void MoveNext()
        {
            if (IsEmpty)
            {
                throw new IndexOutOfRangeException("source is empty");
            }
            ++position_;
        }

        public void Save()
        {
            stack_.Push(new SavePoint(position_, Node.NONE, nodes_.Count));
        }

        public void Save(int tag)
        {
            stack_.Push(new SavePoint(position_, tag, nodes_.Count));
        }

        public void Shift()
        {
            SavePoint savePoint = stack_.Pop();
            if(savePoint.Tag != Node.NONE)
            {
                // 子Nodeを集める。
                Node[] children = new Node[nodes_.Count - savePoint.NodePosition];
                nodes_.CopyTo(savePoint.NodePosition, children, 0, children.Length);

                // 集めた子Nodeを破棄する
                nodes_.RemoveRange(savePoint.NodePosition, children.Length);

                // 新Nodeを生成して末尾に追加する。
                nodes_.Add(new Node(savePoint.Tag, savePoint.Position, position_, children));
            }
        }

        public bool IsEmpty
        {
            get
            {
                return position_ >= GetLength();
            }
        }

        public IEnumerable<Node> Nodes
        {
            get
            {
                return nodes_;
            }
        }

        /// <summary>
        /// 指定位置の要素を取得する。
        /// </summary>
        /// <param name="position">取得する要素の位置</param>
        /// <returns>指定位置の要素</returns>
        protected abstract T GetAt(int position);

        /// <summary>
        /// 配列の長さを取得する。
        /// </summary>
        /// <returns>配列の長さ</returns>
        protected abstract int GetLength();
    }

    /// <summary>
    /// 文字列の解析状態クラス
    /// 2GB未満限定
    /// </summary>
    public class StringContext : ArrayContext<char>
    {
        // 文字列ソース
        private string source_;

        /// <summary>
        /// ソースコード文字列を指定して生成する
        /// </summary>
        /// <param name="source">ソースコード文字列</param>
        public StringContext(string source)
        {
            this.source_ = source;
        }

        protected override char GetAt(int position)
        {
            return source_[position];
        }

        protected override int GetLength()
        {
            return source_.Length;
        }
    }
}
