
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsers
{
    /// <summary>
    /// 解析器
    /// </summary>
    /// <typeparam name="T">ソースの文字の型</typeparam>
    public interface Parser<T>
    {
        /// <summary>
        /// ソースに対して解析を行い、結果を返す。
        /// 解析成功時、trueが返り、ソースは解析後の位置に進められる。
        /// 解析失敗時、falseが返り、ソースの状態は変わらない。
        /// </summary>
        /// <param name="source">解析対象のソース</param>
        /// <returns>解析成功したかどうか</returns>
        bool Parse(Context<T> source);
    }

    /// <summary>
    /// 子Parserを持つ親Parserの基本クラス
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public abstract class SingleChildParser<T> : Parser<T>
    {
        // 子Parser
        private Parser<T> child_;

        /// <summary>
        /// 子Parserを指定して生成する。
        /// </summary>
        /// <param name="child">子Parser</param>
        public SingleChildParser(Parser<T> child)
        {
            this.child_ = child;
        }

        /// <summary>
        /// 子Parser
        /// </summary>
        protected Parser<T> Child
        {
            get
            {
                return child_;
            }
        }

        public abstract bool Parse(Context<T> source);
    }


    /// <summary>
    /// 子Parserを複数持つ親Parserの基本クラス
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public abstract class ChildrenParser<T> : Parser<T>
    {
        // 子Parser
        private Parser<T>[] children_;

        /// <summary>
        /// 子Parserを指定して生成する。
        /// </summary>
        /// <param name="children">子Parser</param>
        public ChildrenParser(Parser<T>[] children)
        {
            this.children_ = (Parser<T>[])children.Clone();
        }

        protected IEnumerable<Parser<T>> Children
        {
            get
            {
                return children_;
            }
        }

        public abstract bool Parse(Context<T> source);
    }

    /// <summary>
    /// 必ず失敗するParser
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public class FailParser<T> : Parser<T>
    {
        public bool Parse(Context<T> source)
        {
            return false;
        }
    }

    /// <summary>
    /// ソースが空の場合にマッチするParser
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public class EmptyParser<T> : Parser<T>
    {
        public bool Parse(Context<T> source)
        {
            return source.IsEmpty;
        }
    }

    /// <summary>
    /// 任意の1文字にマッチするParser
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public class AnyParser<T> : Parser<T>
    {
        public bool Parse(Context<T> source)
        {
            if (source.IsEmpty)
            {
                return false;
            }
            source.MoveNext();
            return true;
        }
    }

    /// <summary>
    /// 1文字にマッチするParser
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public class CharParser<T> : Parser<T>
    {
        // マッチする文字
        private T ch_;

        /// <summary>
        /// マッチする文字を指定して生成する
        /// </summary>
        /// <param name="ch">マッチする文字</param>
        public CharParser(T ch)
        {
            this.ch_ = ch;
        }

        public bool Parse(Context<T> source)
        {
            if (!source.IsEmpty && source.Current.Equals(ch_))
            {
                source.MoveNext();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// stringにマッチするParser
    /// </summary>
    public class StringParser<T> : Parser<T>
    {
        // マッチする文字列
        private string str_;

        /// <summary>
        /// マッチする文字列を指定して生成する
        /// </summary>
        /// <param name="s">マッチする文字列</param>
        public StringParser(string s)
        {
            this.str_ = s;
        }

        public bool Parse(Context<T> source)
        {
            source.Save();
            try
            {
                foreach (char c in str_)
                {
                    // ソースが空か、マッチしなかったら解析失敗
                    if (source.IsEmpty || !source.Current.Equals(c))
                    {
                        source.Backtrack();
                        return false;
                    }

                    // 次の文字へ
                    source.MoveNext();
                }

                // 解析成功
                source.Shift();
                return true;
            }
            catch (Exception e)
            {
                // 例外発生時はソースを元に戻す
                source.Backtrack();
                throw e;
            }
        }
    }

    /// <summary>
    /// 文字範囲にマッチするParser
    /// </summary>
    public class RangeParser<T> : Parser<T>
    {
        // 文字範囲の始点
        private T begin_;

        // 文字範囲の終点
        private T end_;

        // 比較子
        private IComparer<T> comparer_;

        /// <summary>
        /// [begin, end]の範囲の文字にマッチする解析器を生成する
        /// </summary>
        /// <param name="begin">範囲の始点</param>
        /// <param name="end">範囲の終点</param>
        /// <param name="comparer">比較用オブジェクト</param>
        public RangeParser(T begin, T end, IComparer<T> comparer)
        {
            this.begin_ = begin;
            this.end_ = end;
            this.comparer_ = comparer;
        }

        /// <summary>
        /// [begin, end]の範囲の文字にマッチする解析器を生成する
        /// </summary>
        /// <param name="begin">範囲の始点</param>
        /// <param name="end">範囲の終点</param>
        public RangeParser(T begin, T end) : this(begin, end, Comparer<T>.Default) { }

        public bool Parse(Context<T> source)
        {
            if (source.IsEmpty)
            {
                return false;
            }

            // 現在の先頭文字が範囲外であればfalse
            T c = source.Current;
            if (comparer_.Compare(c, begin_) < 0 || comparer_.Compare(end_, c) < 0)
            {
                return false;
            }

            // 解析成功
            source.MoveNext();
            return true;
        }
    }

    /// <summary>
    /// 文字集合にマッチするParser
    /// </summary>
    public class SetParser<T> : Parser<T>
    {
        // マッチする文字集合
        private T[] charSet_;

        /// <summary>
        /// マッチする文字集合を指定して生成する
        /// </summary>
        /// <param name="charSet">マッチする文字集合</param>
        public SetParser(params T[] charSet)
        {
            charSet_ = (T[]) charSet.Clone();
        }

        public bool Parse(Context<T> source)
        {
            if(source.IsEmpty)
            {
                return false;
            }

            // 文字集合のいずれか1文字と一致すればtrue
            foreach(T c in charSet_)
            {
                if(source.Current.Equals(c))
                {
                    source.MoveNext();
                    return true;
                }
            }

            // 文字集合のどの文字ともマッチしなかった
            return false;
        }
    }

    /// <summary>
    /// 存在確認Parser
    /// ソースの状態は変更しない。
    /// </summary>
    public class ExistParser<T> : SingleChildParser<T>
    {
        /// <summary>
        /// 存在確認を行うParserを指定して生成する
        /// </summary>
        /// <param name="child">存在確認を行うParser</param>
        public ExistParser(Parser<T> child) : base(child) { }

        public override bool Parse(Context<T> source)
        {
            source.Save();
            try
            {
                return Child.Parse(source);
            }
            finally
            {
                source.Backtrack();
            }
        }
    }

    /// <summary>
    /// 非存在確認Parser
    /// ソースの状態は変更しない。
    /// </summary>
    public class NotExistParser<T> : SingleChildParser<T>
    {
        /// <summary>
        /// 非存在確認を行うParserを指定して生成する
        /// </summary>
        /// <param name="child">非存在確認を行うParser</param>
        public NotExistParser(Parser<T> child) : base(child) { }

        public override bool Parse(Context<T> source)
        {
            source.Save();
            try
            {
                return !Child.Parse(source);
            }
            finally
            {
                source.Backtrack();
            }
        }
    }

    /// <summary>
    /// オプションParser
    /// </summary>
    public class OptionalParser<T> : SingleChildParser<T>
    {
        /// <summary>
        /// 子Parserを指定して生成する
        /// </summary>
        /// <param name="child">子Parser</param>
        public OptionalParser(Parser<T> child) : base(child) { }

        public override bool Parse(Context<T> source)
        {
            Child.Parse(source);
            return true;
        }
    }

    /// <summary>
    /// 0回以上Parser
    /// </summary>
    public class ZeroOrMoreParser<T> : SingleChildParser<T>
    {
        /// <summary>
        /// 子Parserを指定して生成する
        /// </summary>
        /// <param name="child">子Parser</param>
        public ZeroOrMoreParser(Parser<T> child) : base(child) { }

        public override bool Parse(Context<T> source)
        {
            while(Child.Parse(source))
                ; // do nothing
            return true;
        }
    }

    /// <summary>
    /// 1回以上Parser
    /// </summary>
    public class OneOrMoreParser<T> : SingleChildParser<T>
    {
        /// <summary>
        /// 子Parserを指定して生成する
        /// </summary>
        /// <param name="child">子Parser</param>
        public OneOrMoreParser(Parser<T> child) : base(child) { }

        public override bool Parse(Context<T> source)
        {
            bool result = false;
            while (Child.Parse(source))
            {
                // 1回以上成功したら成功を返す
                result = true;
            }
            return result;
        }
    }

    /// <summary>
    /// Parserの連接にマッチするParser
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public class SequenceParser<T> : ChildrenParser<T>
    {
        /// <summary>
        /// 連接するParserの配列を指定して生成する。
        /// </summary>
        /// <param name="parsers">連接するParserの配列</param>
        public SequenceParser(params Parser<T>[] parsers) : base(parsers) {}

        public override bool Parse(Context<T> source)
        {
            source.Save();
            try
            {
                foreach (Parser<T> p in Children)
                {
                    // 解析失敗したParserがあれば元に戻す
                    if(!p.Parse(source))
                    {
                        source.Backtrack();
                        return false;
                    }
                }

                // すべて解析成功
                source.Shift();
                return true;
            }
            catch(Exception e)
            {
                // エラー時は元に戻す
                source.Backtrack();
                throw e;
            }
        }
    }

    /// <summary>
    /// Parserの選択にマッチするParser
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public class ChoiceParser<T> : ChildrenParser<T>
    {
        /// <summary>
        /// 選択するParserの配列を指定して生成する。
        /// </summary>
        /// <param name="parsers">選択するParserの配列</param>
        public ChoiceParser(params Parser<T>[] children) : base(children) {}

        public override bool Parse(Context<T> source)
        {
            foreach (Parser<T> p in Children)
            {
                // 解析成功したParserがあれば終了
                if (p.Parse(source))
                {
                    return true;
                }
            }

            // すべて解析失敗
            return false;
        }
    }

    /// <summary>
    /// Nodeを生成するParser
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public class NodeParser<T> : SingleChildParser<T>
    {
        // ノードの種類を示すタグ値
        private int tag_;

        /// <summary>
        /// Nodeの範囲を示すParserとタグ値を指定して生成する。
        /// </summary>
        /// <param name="tag">タグ値</param>
        /// <param name="child">Nodeの範囲を示すParser</param>
        public NodeParser(int tag, Parser<T> child) : base(child)
        {
            this.tag_ = tag;
        }

        public override bool Parse(Context<T> source)
        {
            source.Save(tag_);
            try
            {
                if (Child.Parse(source))
                {
                    // 解析成功時、シフトでNode生成
                    source.Shift();
                    return true;
                }

                // 解析失敗
                source.Backtrack();
                return false;
            }
            catch (Exception e)
            {
                // エラー時は元に戻す
                source.Backtrack();
                throw e;
            }
        }
    }

    /// <summary>
    /// Parser生成クラス
    /// </summary>
    /// <typeparam name="T">文字の型</typeparam>
    public class ParserBuilder<T>
    {
        // 生成中のParser
        private List<Parser<T>> parsers_ = new List<Parser<T>>();

        // FailParser
        private FailParser<T> failParser_ = new FailParser<T>();

        // EmptyParser
        private EmptyParser<T> emptyParser_ = new EmptyParser<T>();

        // 連接開始位置のスタック
        private Stack<int> sequencePositions_ = new Stack<int>();

        // 親Parser生成関数のスタック
        private Stack<Func<IEnumerable<Parser<T>>, Parser<T>>> parentBuilders_
            = new Stack<Func<IEnumerable<Parser<T>>, Parser<T>>>();

        /// <summary>
        /// FailParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Fail()
        {
            parsers_.Add(failParser_);
            return this;
        }

        /// <summary>
        /// EmptyParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Empty()
        {
            parsers_.Add(emptyParser_);
            return this;
        }

        /// <summary>
        /// AnyParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Any()
        {
            parsers_.Add(new AnyParser<T>());
            return this;
        }

        /// <summary>
        /// CharParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Ch(T ch)
        {
            parsers_.Add(new CharParser<T>(ch));
            return this;
        }

        /// <summary>
        /// StringParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Str(string s)
        {
            parsers_.Add(new StringParser<T>(s));
            return this;
        }

        /// <summary>
        /// RangeParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Range(T begin, T end)
        {
            parsers_.Add(new RangeParser<T>(begin, end));
            return this;
        }

        /// <summary>
        /// SetParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Set(params T[] set)
        {
            parsers_.Add(new SetParser<T>(set));
            return this;
        }

        /// <summary>
        /// ExistParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Exist()
        {
            PushParent(children => new ExistParser<T>(new SequenceParser<T>(children.ToArray())));
            return this;
        }

        /// <summary>
        /// NotExistParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> NotExist()
        {
            PushParent(children => new NotExistParser<T>(new SequenceParser<T>(children.ToArray())));
            return this;
        }

        /// <summary>
        /// OptionalParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Optional()
        {
            PushParent(children => new OptionalParser<T>(new SequenceParser<T>(children.ToArray())));
            return this;
        }

        /// <summary>
        /// ZeroOrMoreParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> ZeroOrMore()
        {
            PushParent(children => new ZeroOrMoreParser<T>(new SequenceParser<T>(children.ToArray())));
            return this;
        }

        /// <summary>
        /// OneOrMoreParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> OneOrMore()
        {
            PushParent(children => new OneOrMoreParser<T>(new SequenceParser<T>(children.ToArray())));
            return this;
        }

        /// <summary>
        /// SequenceParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Begin()
        {
            PushParent(children => new SequenceParser<T>(children.ToArray()));
            return this;
        }

        /// <summary>
        /// ChoiceParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Choice()
        {
            PushParent(children => new ChoiceParser<T>(children.ToArray()));
            return this;
        }

        /// <summary>
        /// NodeParserの追加
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> Node(int tag)
        {
            PushParent(children => new NodeParser<T>(tag, new SequenceParser<T>(children.ToArray())));
            return this;
        }

        // 親Parserの開始
        private void PushParent(Func<IEnumerable<Parser<T>>, Parser<T>> parentBuilder)
        {
            parentBuilders_.Push(parentBuilder);
            try
            {
                sequencePositions_.Push(parsers_.Count);
            }
            catch(Exception e)
            {
                // 途中でエラー発生時はスタックを元に戻す
                parentBuilders_.Pop();
                throw e;
            }
        }

        /// <summary>
        /// 親Parserを閉じる
        /// </summary>
        /// <returns>Parser追加後のBuilder</returns>
        public ParserBuilder<T> End()
        {
            // 親Parser生成関数・開始位置を取得する
            Func<IEnumerable<Parser<T>>, Parser<T>> parentBuilder = parentBuilders_.Peek();
            int position = sequencePositions_.Peek();

            // 親Parserを生成する
            Parser<T> parentParser = parentBuilder(parsers_.Skip(position));
            parsers_.RemoveRange(position, parsers_.Count - position);
            parsers_.Add(parentParser);

            // 追加成功後、スタックを破棄する
            parentBuilders_.Pop();
            sequencePositions_.Pop();

            return this;
        }

        /// <summary>
        /// Parser生成
        /// </summary>
        /// <returns>生成したParser</returns>
        public Parser<T> Build()
        {
            if(parsers_.Count == 0)
            {
                // Parser未生成の場合はFailParser
                return failParser_;
            }
            else
            {
                // 親Parserをすべて閉じる
                while (parentBuilders_.Count > 0)
                {
                    End();
                }

                // 生成されたParserを繋いだParserを返す
                return new SequenceParser<T>(parsers_.ToArray());
            }
        }
    } 
}
