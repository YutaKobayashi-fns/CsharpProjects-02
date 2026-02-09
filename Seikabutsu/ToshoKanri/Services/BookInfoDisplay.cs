using System.Data;
using ToshoKanri.Models;

namespace ToshoKanri.Services;

public class BookInfoDisplay
{
    // OrderedDictionary型のコレクションで保有している書籍情報を、一覧表示するメソッド
    // 処理概要：コレクションで保有している情報を参照し、コンソール上に読み取り上限数分の書籍情報を一覧形式で表示します。
    public void DispAllBooks(OrderedDictionary<string, BookInfo> bookDictionary, int dispMax)
    {
        int bookCount = 0;  // 表示済書籍情報件数

        Console.Clear();
        Console.WriteLine("");
        Console.WriteLine("==========================書籍一覧==========================");
        foreach (string key in bookDictionary.Keys)
        {
            bookCount++;

            // コレクションの保有情報数はリスト最大保有件数以下か？
            if (bookCount <= BookInfo.D_READLIST_COUNT_MAX)
            {
                Console.WriteLine($"[ {bookCount} / {dispMax} ]");
                BookInfoShow(bookDictionary[key]);  // 書籍情報表示
                Console.WriteLine("============================================================");
            }
        }

        // コレクションは書籍情報を保有しているか？
        if (bookDictionary.Count() < BookInfo.D_READLIST_COUNT_MIN)
        {
            Console.WriteLine("書籍情報の登録はありません。");
            Console.WriteLine("============================================================");
        }

        Console.WriteLine("");
    }

    // 書籍情報を基に、登録されている書籍の情報を表示するメソッド
    // 処理概要：引数に指定された書籍情報を参照し、コンソール上に各種書籍情報を表示します。
    public void BookInfoShow(BookInfo info)
    {
        Console.WriteLine($"タイトル    ：{info.title}");
        Console.WriteLine($"ISBNコード  ：{info.isbn}");
        Console.WriteLine($"著者        ：{info.publisher}");
        Console.WriteLine($"製本形態    ：{info.kind}");
        Console.WriteLine($"ジャンル    ：{info.genre}");
        Console.WriteLine($"価格        ：{info.price}");
        Console.WriteLine($"JANコード   ：{info.jan}");
    }
}
