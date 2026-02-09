using System.Data;
using Microsoft.VisualBasic;
using ToshoKanri.Models;

namespace ToshoKanri.Services;

public class BookFileWriter
{
    // OrderedDictionary型のコレクションより、登録されている書籍情報を記録したテキストファイルを作成するメソッド
    // 処理概要：コレクションで保有している書籍情報をすべて参照し、テキスト出力用の判別文字（区切り文字、終端文字等）を設定した各種書籍情報の作成を実行。
    //          外部出力として作成した書籍情報をテキストファイルに出力します。
    public void WriteDictionaryToText(OrderedDictionary<string, BookInfo> bookDataDict, string textName, int writeLineMax)
    {
        using (var writer = new StreamWriter(textName))
        {
            foreach (string key in bookDataDict.Keys)
            {
                // テキスト出力用付加文字設定
                string bookIsbn = "[01]";
                string bookTitle = "[02]";
                string bookPublisher = "[03]";
                string bookKind = "[04]";
                string bookGenre = "[05]";
                string bookPrice = "[06]";
                string bookJan = "[07]";
                string bookDataSplit = "[CO]";

                // テキスト出力用書籍情報作成
                bookIsbn += bookDataDict[key].isbn;
                bookTitle += bookDataDict[key].title;
                bookPublisher += bookDataDict[key].publisher;
                bookKind += bookDataDict[key].kind;
                bookGenre += bookDataDict[key].genre;
                bookPrice += bookDataDict[key].price;
                bookJan += bookDataDict[key].jan;

                // 作成情報をテキストへ出力
                writer.WriteLine($"{bookIsbn}");
                writer.WriteLine($"{bookTitle}");
                writer.WriteLine($"{bookPublisher}");
                writer.WriteLine($"{bookKind}");
                writer.WriteLine($"{bookGenre}");
                writer.WriteLine($"{bookPrice}");
                writer.WriteLine($"{bookJan}");
                writer.WriteLine($"{bookDataSplit}");
            }

            // 終端文字付加処理
            string bookDataEnd = "[EN]";
            writer.WriteLine($"{bookDataEnd}");
        }
    }
}

