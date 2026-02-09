
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO.Pipelines;
using ToshoKanri.Models;
using ToshoKanri.Services;

class Program
{
    /* 定数宣言 */
    // 定数(数値)
    public const int D_MENU_NO_MIN = 1;    // 機能番号最小
    public const int D_MENU_NO_MAX = 5;    // 機能番号最大
     // 定数(文字列)
    public const string R_TEXT_BOOKS = "syoseki-read.txt";  // 読み込みテキスト名称
    public const string W_TEXT_BOOKS = "syoseki-write.txt"; // 書き込みテキスト名称

    // コレクション宣言
    static List<string> readTextList = new();
    static OrderedDictionary<string, BookInfo> bookDataDict = new();

    // インスタンス作成
    static BookFileReader reader = new BookFileReader();
    static BookFileWriter writers = new BookFileWriter();
    static BookInfoDisplay infoDisp = new BookInfoDisplay();
    static BookRepository repo = new BookRepository();

    // メイン処理
    static void Main()
    {
        // 変数宣言
        bool processContinue = true;

        try
        {
            bookDataDict = reader.ReadListToDictionary(R_TEXT_BOOKS, BookInfo.D_READTEXT_LINE_MAX); // 書籍情報読み込み処理

            do
            {
                Console.Clear();

                // メニュー表示
                showMenu(); 
                processContinue = selectFunction();

            } while (processContinue == true);
        }
        catch (Exception)
        {
            Console.WriteLine("");
            Console.WriteLine("実行中の処理でエラーが発生しました。");
            Console.WriteLine("出力されたエラー内容を確認してください。");
            Console.WriteLine("");
            Console.Write("Press Enter Key...");
            Console.ReadLine();
            Console.Clear();
        }
    }

    // 起動時のメニューを表示するメソッド
    // 処理概要：アプリ起動時にコンソールでメニュー画面を表示します。
    static void showMenu()
    {
        Console.WriteLine("====== 書籍管理アプリ ======");
        Console.WriteLine($"\n利用する機能番号 [ {D_MENU_NO_MIN} ~ {D_MENU_NO_MAX} ] を指定してください。");
        Console.WriteLine("");
        Console.WriteLine("\t1 : 書籍一覧表示");
        Console.WriteLine("\t2 : 書籍登録");
        Console.WriteLine("\t3 : 書籍削除");
        Console.WriteLine("\t4 : 書籍情報更新");
        Console.WriteLine("\t5 : 終了");
        Console.WriteLine("");
    }

    // ユーザがコンソールで入力した情報を基に、各種該当機能を呼び出すメソッド
    // 処理概要：コンソールで入力された機能番号を基に、各機能番号に応じた関連処理を呼び出す処理を行います。
    static bool selectFunction()
    {
        string? result; // ユーザ入力値
        bool processContinue = true;    // 処理続行判定

        Console.Write("Input No : ");
        result = Console.ReadLine();

        
        if (result != null)
        {
            switch (result)
            {
                case "1":   // 書籍一覧表示
                    infoDisp.DispAllBooks(bookDataDict, BookInfo.D_READLIST_COUNT_MAX);

                    Console.WriteLine("");
                    Console.WriteLine("書籍一覧の表示が完了しました。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                    Console.Clear();

                    break;

                case "2":
                    // 登録済の書籍はリスト最大件数未満か？
                    if (bookDataDict.Count < BookInfo.D_READLIST_COUNT_MAX)
                    {
                        BookInfo addBook = new BookInfo();

                        try
                        {
                            repo.AddBook(bookDataDict, addBook);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("予期せぬエラーが発生しました。");
                            Console.WriteLine("エラーログを出力します。");
                            Console.WriteLine("[ErrorLog Start]====================================================");
                            Console.WriteLine(ex);
                            Console.WriteLine("[ErrorLog End...]===================================================");
                            Console.WriteLine("");
                        }

                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine("登録済の書籍数が最大件数に達しています。");
                        Console.WriteLine("登録済の書籍情報を削除してから、再度登録操作を実行してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }

                    break;

                case "3":
                    // 書籍情報は登録済か？
                    if (bookDataDict.Count >= BookInfo.D_READLIST_COUNT_MIN)
                    {
                        try
                        {
                            repo.RemoveBook(bookDataDict, BookInfo.D_READLIST_COUNT_MIN, BookInfo.D_READLIST_COUNT_MAX);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("予期せぬエラーが発生しました。");
                            Console.WriteLine("エラーログを出力します。");
                            Console.WriteLine("[ErrorLog Start]====================================================");
                            Console.WriteLine(ex);
                            Console.WriteLine("[ErrorLog End...]===================================================");
                            Console.WriteLine("");
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine("削除可能な書籍情報の登録がありません。");
                        Console.WriteLine("書籍情報の確認を行ってください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }

                    break;

                case "4":
                    // 書籍情報は登録済か？
                    if (bookDataDict.Count >= BookInfo.D_READLIST_COUNT_MIN)
                    {
                        try
                        {
                            repo.UpdateBookInfo(bookDataDict);
                            writers.WriteDictionaryToText(bookDataDict, W_TEXT_BOOKS, BookInfo.D_WRITELIST_COUNT_MAX);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("予期せぬエラーが発生しました。");
                            Console.WriteLine("エラーログを出力します。");
                            Console.WriteLine("[ErrorLog Start]====================================================");
                            Console.WriteLine(ex);
                            Console.WriteLine("[ErrorLog End...]===================================================");
                            Console.WriteLine("");
                        }

                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine("更新可能な書籍情報の登録がありません。");
                        Console.WriteLine("書籍情報の確認を行ってください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                    break;

                case "5":
                    Console.Clear();
                    Console.WriteLine("");
                    Console.WriteLine("アプリケーションを終了します。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                    Console.Clear();

                    processContinue = false;
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("有効な機能番号が入力されませんでした。");
                    Console.WriteLine($"[ {D_MENU_NO_MIN} ~ {D_MENU_NO_MAX} ] の機能番号を指定してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                    break;
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("null文字が入力されました。");
            Console.WriteLine($"[ {D_MENU_NO_MIN} ~ {D_MENU_NO_MAX} ] の機能番号を指定してください。");
            Console.WriteLine("");
            Console.Write("Press Enter Key...");
            Console.ReadLine();
        }

        return processContinue;
    }
}