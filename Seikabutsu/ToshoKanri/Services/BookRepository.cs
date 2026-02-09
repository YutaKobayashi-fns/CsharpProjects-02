using System.Data;
using System.IO.Pipelines;
using ToshoKanri;
using ToshoKanri.Models;

namespace ToshoKanri.Services;

public class BookRepository
{
    /* 定数宣言 */
    // 処理結果
    public const int D_REPO_PROCESS_OK = 0;    // 正常終了
    public const int D_REPO_PROCESS_NG = 1;    // 異常終了
    public const int D_REPO_PROCESS_BREAK = 999;    // 中断

    // ユーザがコンソール上で指定した書籍情報を、OrderedDictionary型のコレクションより削除するメソッド
    // 処理概要：コンソール上で指定した番号の書籍情報をコレクションより削除し、外部出力としてテキストに削除処理後の書籍一覧情報を出力します。
    public void RemoveBook(OrderedDictionary<string, BookInfo> bookDataDict, int infoCountMin, int infoCountMax)
    {
        // インスタンス作成
        BookInfoDisplay infoDisp = new BookInfoDisplay();
        BookFileWriter writers = new BookFileWriter();

        string? result;     // ユーザ入力値
        int dictCount = 0;  // コレクション保有情報数
        string delKey = ""; // 書籍情報削除対象キー情報

        dictCount = bookDataDict.Count;

        // コレクションは情報を保有しているか？
        if (dictCount > 0)
        {
            bool compFlg = false;

            while (compFlg == false)
            {
                infoDisp.DispAllBooks(bookDataDict, infoCountMax);  // 書籍情報一覧表示

                Console.Write("");
                Console.WriteLine("※削除処理を中断する場合は[ Cancel ]を入力してください。");
                Console.Write("削除したい書籍情報の番号（ [ ** / 10 ] の  ** に表示される番号）を入力してください。 : ");
                result = Console.ReadLine();

                // nullチェック
                if (result != null)
                {
                    // ユーザ入力値は整数値か？
                    if (int.TryParse(result, out int idx))
                    {
                        // 入力値は有効な整数値か？
                        if ((idx <= dictCount) && (idx >= infoCountMin))
                        {
                            delKey = bookDataDict.ElementAt(idx - 1).Key;

                            infoDisp.BookInfoShow(bookDataDict[delKey]);    // 書籍情報表示

                            Console.WriteLine("");
                            Console.WriteLine("※削除処理を中断する場合は[ Cancel ]を入力してください。");
                            Console.Write("表示された書籍情報を削除しますか？ ( y / n ) : ");
                            result = Console.ReadLine();

                            // nullチェック
                            if (result != null)
                            {
                                string selectResult = "";
                                selectResult = result;

                                // ユーザ入力値は"y(yes)か？"
                                if (selectResult.Replace(" ", "").ToLower() == "y")
                                {
                                    bookDataDict.Remove(delKey);    // 書籍情報削除 
                                    infoDisp.DispAllBooks(bookDataDict, infoCountMax);  // 書籍情報一覧表示

                                    try
                                    {
                                        writers.WriteDictionaryToText(bookDataDict, Program.W_TEXT_BOOKS, BookInfo.D_WRITELIST_COUNT_MAX);  // 書籍情報テキスト書き出し
                                    }
                                    catch (IOException ioEx)
                                    {
                                        throw new IOException("RemoveBook:IOException", ioEx);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("RemoveBook:Exception", ex);
                                    }

                                    compFlg = true;

                                    Console.WriteLine("該当情報を削除しました。");
                                    Console.WriteLine("");
                                    Console.Write("Press Enter Key...");
                                    Console.ReadLine();
                                }
                                // ユーザ入力値は"n(no)か？"
                                else if (selectResult.Replace(" ", "").ToLower() == "n")
                                {
                                    continue;
                                }
                                // ユーザ入力値は"Cancelか？"
                                else if (selectResult.Replace(" ", "") == "Cancel")
                                {
                                    Console.Clear();
                                    Console.WriteLine("");
                                    Console.WriteLine($"[ {selectResult} ]が入力されました。");
                                    Console.WriteLine("削除処理を中断します。");
                                    Console.WriteLine("");
                                    Console.Write("Press Enter Key...");
                                    Console.ReadLine();
                                    break;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("[ y ] または [ n ] 以外の文字が入力されています。");
                                    Console.WriteLine("再入力してください。");
                                    Console.WriteLine("");
                                    Console.Write("Press Enter Key...");
                                    Console.ReadLine();
                                }
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("");
                            Console.WriteLine($"無効な番号が入力されました。(入力値[ {result} ])");
                            Console.WriteLine("入力値を確認してください。");
                            Console.WriteLine("");
                            Console.Write("Press Enter Key...");
                            Console.ReadLine();
                        }
                    }
                    else if (result.Replace(" ", "") == "Cancel")
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine($"[ {result} ]が入力されました。");
                        Console.WriteLine("処理を中断します。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine($"無効なデータが入力されました。(入力値[ {result} ])");
                        Console.WriteLine("入力値を確認してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("null文字が入力されています。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }

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

        Console.Clear();
        Console.WriteLine("削除処理を終了します。");
        Console.WriteLine("");
        Console.Write("Press Enter Key...");
        Console.ReadLine();
        Console.Clear();
    }

    // ユーザがコンソール上で入力した書籍情報を、OrderedDictionary型のコレクションに新規追加するメソッド
    // 処理概要：追加書籍情報の作成処理を呼び出し、外部出力としてテキストに作成した書籍情報を追加した書籍一覧情報を出力します。
    public void AddBook(OrderedDictionary<string, BookInfo> bookDataDict, BookInfo addBook)
    {
        string? result;         // ユーザ入力値
        bool compFlg = false;   // 処理完了フラグ
        int endFlg = D_REPO_PROCESS_OK; // 処理結果判断フラグ

        // インスタンス作成
        BookInfoDisplay infoDisp = new BookInfoDisplay();
        BookFileWriter writers = new BookFileWriter();

        while (compFlg == false)
        {
            endFlg = MakeAddBookInfo(bookDataDict, addBook);    // 追加書籍情報作成

            // 処理結果は正常終了か？
            if (endFlg == D_REPO_PROCESS_OK)
            {
                while (compFlg == false)
                {
                    Console.WriteLine("");
                    Console.WriteLine("入力した情報を表示します。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                    Console.Clear();

                    infoDisp.BookInfoShow(addBook); // 書籍情報表示

                    Console.WriteLine("この情報で登録してもよろしいですか？");
                    Console.Write("( y / n ) : ");
                    result = Console.ReadLine();

                    // nullチェック
                    if (result != null)
                    {
                        string selectResult = "";
                        selectResult = result;

                        // ユーザ入力値は"y(yes)か？" 
                        if (selectResult.Replace(" ", "").ToLower() == "y")
                        {
                            int dictCount = 0;  // コレクション保有情報数

                            dictCount = bookDataDict.Count();

                            // コレクションの保有情報数はリスト最大保有件数未満か？
                            if (dictCount < BookInfo.D_READLIST_COUNT_MAX)
                            {
                                bookDataDict.Add(addBook.isbn, addBook);

                                infoDisp.DispAllBooks(bookDataDict, BookInfo.D_READLIST_COUNT_MAX); // 書籍情報一覧表示

                                try
                                {
                                    writers.WriteDictionaryToText(bookDataDict, Program.W_TEXT_BOOKS, BookInfo.D_WRITELIST_COUNT_MAX);  // 書籍情報テキスト書き出し
                                }
                                catch (IOException ioEx)
                                {
                                    throw new IOException("AddBook:IOException", ioEx);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("AddBook:Exception", ex);
                                }

                                compFlg = true;

                                Console.WriteLine("");
                                Console.WriteLine("書籍情報の登録が完了しました。");
                                Console.WriteLine("");
                                Console.Write("Press Enter Key...");
                                Console.ReadLine();
                            }
                            else
                            {
                                endFlg = D_REPO_PROCESS_BREAK;

                                Console.Clear();
                                Console.WriteLine("");
                                Console.WriteLine("登録済の書籍情報が最大件数に達しているため、登録処理を中断します。");
                                Console.WriteLine("書籍情報を削除してから、再度登録処理を実行してください。");
                                Console.WriteLine("");
                                Console.Write("Press Enter Key...");
                                Console.ReadLine();

                                break;
                            }
                        }
                        // ユーザ入力値は"n(no)か？"
                        else if (selectResult.Replace(" ", "").ToLower() == "n")
                        {
                            while (endFlg == D_REPO_PROCESS_OK)
                            {
                                Console.Clear();
                                Console.WriteLine("登録処理を中断しますか？");
                                Console.Write("( y / n ) : ");
                                result = Console.ReadLine();

                                // nullチェック
                                if (result != null)
                                {
                                    selectResult = "";
                                    selectResult = result;

                                    // ユーザ入力値は"y(yes)か？"
                                    if (selectResult.Replace(" ", "").ToLower() == "y")
                                    {
                                        endFlg = D_REPO_PROCESS_BREAK;
                                    }
                                    // ユーザ入力値は"n(no)か？"
                                    else if (selectResult.Replace(" ", "").ToLower() == "n")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.Clear();
                                        Console.WriteLine("");
                                        Console.WriteLine("[ y ] または [ n ] 以外の文字が入力されています。");
                                        Console.WriteLine("再入力してください。");
                                        Console.WriteLine("");
                                        Console.Write("Press Enter Key...");
                                        Console.ReadLine();
                                    }
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("");
                                    Console.WriteLine("null文字が入力されています。");
                                    Console.WriteLine("再入力してください。");
                                    Console.WriteLine("");
                                    Console.Write("Press Enter Key...");
                                    Console.ReadLine();
                                }
                            }

                            // 処理結果が中断か？
                            if (endFlg == D_REPO_PROCESS_BREAK)
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("");
                            Console.WriteLine("[ y ] または [ n ] 以外の文字が入力されています。");
                            Console.WriteLine("再入力してください。");
                            Console.WriteLine("");
                            Console.Write("Press Enter Key...");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine("null文字が入力されています。");
                        Console.WriteLine("再入力してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                }
            }
            // 処理結果が異常終了か？
            else if (endFlg == D_REPO_PROCESS_NG)
            {
                while (endFlg == D_REPO_PROCESS_NG)
                {
                    Console.WriteLine("登録処理中に異常が発生しています。");
                    Console.Write("再度、登録処理を行いますか？（ y / n ）: ");
                    result = Console.ReadLine();

                    // nullチェック
                    if (result != null)
                    {
                        string selectResult = "";
                        selectResult = result;

                        // ユーザ入力値は"y(yes)か？"
                        if (selectResult.Replace(" ", "").ToLower() == "y")
                        {
                            endFlg = D_REPO_PROCESS_OK;
                        }
                        // ユーザ入力値は"no(no)か？"
                        else if (selectResult.Replace(" ", "").ToLower() == "n")
                        {
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("");
                            Console.WriteLine("[ y ] または [ n ] 以外の文字が入力されています。");
                            Console.WriteLine("再入力してください。");
                            Console.WriteLine("");
                            Console.Write("Press Enter Key...");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine("null文字が入力されています。");
                        Console.WriteLine("再入力してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                }
            }

            // 処理結果は中断、または以上終了か？
            if ((endFlg == D_REPO_PROCESS_NG) || (endFlg == D_REPO_PROCESS_BREAK))
            {
                string message = "";

                // 処理結果は異常か？
                if (endFlg == D_REPO_PROCESS_NG)
                {
                    message = "書籍情報の作成処理が異常終了した";
                }
                else
                {
                    message = "登録処理の中断が選択された";
                }

                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine($"{message}ため、登録処理を中断します。");
                Console.WriteLine("書籍情報の登録を最初からやり直してください。");
                Console.WriteLine("");
                Console.Write("Press Enter Key...");
                Console.ReadLine();
                break;
            }
        }

    }

    // OrderedDictionary型のコレクションに新規追加する書籍情報を、作成するための各種メソッドを呼び出すメソッド
    // 各種書籍情報に対応したMakeメソッドを呼び出し、コンソールで入力した情報を基に書籍情報を作成します。
    public int MakeAddBookInfo(OrderedDictionary<string, BookInfo> bookDataDict, BookInfo addBook)
    {
        int endFlg = D_REPO_PROCESS_OK; // 処理結果判定フラグ

        endFlg = MakeBookTitle(addBook);    // 書籍情報タイトル情報作成

        // タイトル情報作成処理が正常終了、かつタイトル情報はnull以外か？
        if ((endFlg == D_REPO_PROCESS_OK) && (addBook.title != null))
        {
            endFlg = MakeBookIsbn(bookDataDict, addBook);   // 書籍情報ISBNコード情報作成

            // ISBNコード情報作成処理が正常終了、かつISBNコード情報はnull以外か？
            if ((endFlg == D_REPO_PROCESS_OK) && (addBook.isbn != null))
            {
                endFlg = MakeBookPublisher(addBook);    // 書籍情報著者情報作成

                // 著者情報作成処理が正常終了、かつ著者情報はnull以外か？
                if ((endFlg == D_REPO_PROCESS_OK) && (addBook.publisher != null))
                {
                    endFlg = MakeBookKind(addBook); // 書籍情報製本形態情報作成

                    // 製本形態情報作成処理が正常終了、かつ製本形態情報はnull以外か？
                    if ((endFlg == D_REPO_PROCESS_OK) && (addBook.kind != null))
                    {
                        endFlg = MakeBookGenre(addBook);    // 書籍情報ジャンル情報作成

                        // ジャンル情報作成処理が正常終了、かつジャンル情報はnull以外か？
                        if ((endFlg == D_REPO_PROCESS_OK) && (addBook.genre != null))
                        {
                            endFlg = MakeBookPrice(addBook);    // 書籍情報価格情報作成

                            // 価格情報作成処理が正常終了、かつ価格情報はnull以外か？
                            if ((endFlg == D_REPO_PROCESS_OK) && (addBook.price != null))
                            {
                                // 製本形態情報は"電子書籍"以外か？
                                if (addBook.kind != BookInfo.S_BOOK_KIND_DIGITAL)
                                {
                                    endFlg = MakeBookJan(addBook);  // 書籍情報JANコード情報作成

                                    // JANコード情報作成処理が異常終了したか？
                                    if (endFlg == D_REPO_PROCESS_NG)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("");
                                        Console.WriteLine("JANコードの登録処理で異常が発生しました。");
                                        Console.ReadLine();
                                    }
                                }
                                else
                                {
                                    addBook.jan = "";
                                    Console.Clear();
                                    Console.WriteLine("");
                                    Console.WriteLine("登録対象の書籍が電子書籍のため、JANコードにはブランクが設定されます。");
                                    Console.ReadLine();
                                }
                            }
                            // 価格情報作成処理が異常終了したか？
                            else if (endFlg == D_REPO_PROCESS_NG)
                            {
                                Console.Clear();
                                Console.WriteLine("");
                                Console.WriteLine("価格の登録処理で異常が発生しました。");
                                Console.ReadLine();
                            }
                        }
                        // ジャンル情報作成処理が異常終了したか？
                        else if (endFlg == D_REPO_PROCESS_NG)
                        {
                            Console.Clear();
                            Console.WriteLine("");
                            Console.WriteLine("ジャンルの登録処理で異常が発生しました。");
                            Console.ReadLine();
                        }
                    }
                    // 製本形態情報作成処理が異常終了したか？
                    else if (endFlg == D_REPO_PROCESS_NG)
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine("製本形態の登録処理で異常が発生しました。");
                        Console.ReadLine();
                    }
                }
                // 著者情報作成処理が異常終了したか？
                else if (endFlg == D_REPO_PROCESS_NG)
                {
                    Console.Clear();
                    Console.WriteLine("");
                    Console.WriteLine("著者の登録処理で異常が発生しました。");
                    Console.ReadLine();
                }

            }
            // ISBNコード情報作成処理が異常終了したか？
            else if (endFlg == D_REPO_PROCESS_NG)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("ISBNコードの登録処理で異常が発生しました。");
                Console.ReadLine();
            }
        }
        // タイトル情報作成処理が異常終了したか？
        else if (endFlg == D_REPO_PROCESS_NG)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("タイトルの登録処理で異常が発生しました。");
            Console.ReadLine();
        }

        // 各種情報作成処理が正常終了したか？
        if (endFlg == D_REPO_PROCESS_OK)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("追加する書籍情報の作成が完了しました。");
            //Console.ReadLine();
        }
        // 各種情報作成処理が異常終了したか？
        else if (endFlg == D_REPO_PROCESS_NG)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("書籍情報の作成中に異常が発生しました。");
            Console.ReadLine();
        }

        return endFlg;
    }

    // 書籍情報で保有する情報のうち、タイトルの情報を作成するメソッド
    // 処理概要：コンソールで入力した情報を基に、書籍情報で保有するタイトル情報を作成します。
    public int MakeBookTitle(BookInfo addBook)
    {
        string? result; // ユーザ入力値
        bool titleEnd = false;  // タイトル情報設定処理終了判定
        int endFlg = D_REPO_PROCESS_NG; // 処理結果判定フラグ

        while (titleEnd == false)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("※登録処理を中断する場合は[ Cancel ]を入力してください。");
            Console.Write("登録したい書籍のタイトルを入力してください : ");

            result = Console.ReadLine();

            // nullチェック
            if (result != null)
            {
                // "Cancel"以外の有効値が入力されたか？
                if ((result != "") && (result.Replace(" ", "") != "") && (result.StartsWith(" ") == false) && (result != "Cancel"))
                {
                    // タイトル情報設定
                    string addTitle = result;
                    addBook.title = addTitle;
                    titleEnd = true;
                    endFlg = D_REPO_PROCESS_OK; // 処理正常終了
                }
                // "Cancel"が入力されたか？
                else if (result == "Cancel")
                {
                    titleEnd = true;
                    endFlg = D_REPO_PROCESS_BREAK;  // 処理正常中断
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("先頭文字が空白以外、かつ空白以外で最低1文字以上の文字列入力が必要です。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("null文字が入力されています。");
                Console.WriteLine("再入力してください。");
                Console.WriteLine("");
                Console.Write("Press Enter Key...");
                Console.ReadLine();
            }
        }

        return endFlg;
    }

    // 書籍情報で保有する情報のうち、ISBNコードの情報を作成するメソッド
    // 処理概要：コンソールで入力した情報を基に、書籍情報で保有するISBNコード情報を作成します。
    public int MakeBookIsbn(OrderedDictionary<string, BookInfo> bookDataDict, BookInfo addBook)
    {
        string? result; // ユーザ入力値 
        bool isbnEnd = false;   // ISBNコード情報設定処理終了判定
        bool duplicateFlg = false;  // 重複判定フラグ
        int endFlg = D_REPO_PROCESS_NG; // 処理結果判定フラグ

        while (isbnEnd == false)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("※登録処理を中断する場合は[ Cancel ]を入力してください。");
            Console.Write("登録したい書籍のISBNコード(半角整数13桁)を入力してください : ");

            result = Console.ReadLine();

            // nullチェック
            if (result != null)
            {
                // "Cancel"以外の有効値が入力されたか？
                if ((result != "") && (result.Replace(" ", "") != "") && (result.StartsWith(" ") == false) && (result != "Cancel"))
                {
                    string addIsbn = result;

                    // 追加するISBNコードの桁数は13桁か？
                    if (addIsbn.Length == BookInfo.D_ISBN_MAX)
                    {
                        // 入力値は整数値か？
                        if (long.TryParse(addIsbn, out long isbn) == true)
                        {
                            duplicateFlg = false;

                            // キー情報の重複確認
                            foreach (string key in bookDataDict.Keys)
                            {
                                // 追加するキー情報は登録済か？
                                if (key == addIsbn)
                                {
                                    duplicateFlg = true;
                                    break;
                                }
                            }

                            // キー情報の重複はなしか？
                            if (duplicateFlg == false)
                            {
                                // ISBNコード情報設定
                                addBook.isbn = addIsbn;
                                isbnEnd = true;
                                endFlg = D_REPO_PROCESS_OK;
                            }
                            else
                            {
                                Console.WriteLine("");
                                Console.WriteLine($"入力されたISBNコードは既に登録されています。（ISBN : {addIsbn}）");
                                Console.WriteLine($"入力中のISBNコード、または登録済の書籍情報を確認してください。");
                                Console.WriteLine("");
                                Console.Write("Press Enter Key...");
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"ISBNコードは半角整数 {BookInfo.D_ISBN_MAX} 桁のコードのみ有効です。");
                            Console.WriteLine("再入力してください。");
                            Console.WriteLine("");
                            Console.Write("Press Enter Key...");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine($"ISBNコードは {BookInfo.D_ISBN_MAX} 桁の半角整数コードのみ有効です。");
                        Console.WriteLine("再入力してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                }
                // "Cancel"が入力されたか？
                else if (result == "Cancel")
                {
                    isbnEnd = true;
                    endFlg = D_REPO_PROCESS_BREAK;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine($"先頭文字が空白以外、かつ最低 {BookInfo.D_ISBN_MAX} 桁の半角整数によるコード入力が必要です。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("null文字が入力されています。");
                Console.WriteLine("再入力してください。");
                Console.WriteLine("");
                Console.Write("Press Enter Key...");
                Console.ReadLine();
            }
        }

        return endFlg;
    }

    // 書籍情報で保有する情報のうち、著者の情報を作成するメソッド
    // 処理概要：コンソールで入力した情報を基に、書籍情報で保有する著者情報を作成します。
    public int MakeBookPublisher(BookInfo addBook)
    {
        string? result; // ユーザ入力値
        bool publisherEnd = false;  // 著者情報設定処理終了判定
        int endFlg = D_REPO_PROCESS_NG; // 処理結果判定フラグ

        while (publisherEnd == false)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("※登録処理を中断する場合は[ Cancel ]を入力してください。");
            Console.Write("登録したい書籍の著者を入力してください : ");

            result = Console.ReadLine();

            // nullチェック
            if (result != null)
            {
                // "Cancel"以外の有効値が入力されたか？
                if ((result != "") && (result.Replace(" ", "") != "") && (result.StartsWith(" ") == false) && (result != "Cancel"))
                {
                    // 著者情報設定
                    string addPublisher = result;
                    addBook.publisher = addPublisher;
                    publisherEnd = true;
                    endFlg = D_REPO_PROCESS_OK;
                }
                // "Cancel"が入力されたか？
                else if (result == "Cancel")
                {
                    publisherEnd = true;
                    endFlg = D_REPO_PROCESS_BREAK;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("先頭文字が空白以外、かつ最低1文字以上の文字列入力が必要です。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("null文字が入力されています。");
                Console.WriteLine("再入力してください。");
                Console.WriteLine("");
                Console.Write("Press Enter Key...");
                Console.ReadLine();
            }
        }

        return endFlg;
    }

    // 書籍情報で保有する情報のうち、製本形態の情報を作成するメソッド
    // 処理概要：コンソールで入力した情報を基に、書籍情報で保有する製本形態情報を作成します。
    public int MakeBookKind(BookInfo addBook)
    {
        string? result; // ユーザ入力値
        bool kindEnd = false;   // 製本形態情報設定処理終了判定
        int endFlg = D_REPO_PROCESS_NG; // 処理結果判定フラグ

        while (kindEnd == false)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("※登録処理を中断する場合は[ Cancel ]を入力してください。");
            Console.WriteLine("[1]ソフトカバー [2]ハードカバー [3]電子書籍 [4]その他");
            Console.Write($"登録したい書籍の製本形態[ {BookInfo.D_BOOK_KIND_SOFT} ~ {BookInfo.D_BOOK_KIND_OTHER} ]を入力してください : ");

            result = Console.ReadLine();

            // nullチェック
            if (result != null)
            {
                // "Cancel"以外の有効値が入力されたか？
                if ((result != "") && (result.Replace(" ", "") != "") && (result.StartsWith(" ") == false) && (result != "Cancel"))
                {
                    string addKind = result;

                    // 入力値は整数値か？
                    if (int.TryParse(addKind, out int kind) == true)
                    {
                        // 製本形態の振り分け
                        switch (kind)
                        {
                            case BookInfo.D_BOOK_KIND_SOFT: // ソフトカバー
                                // 製本形態情報設定
                                addBook.kind = BookInfo.S_BOOK_KIND_SOFT;
                                kindEnd = true;
                                endFlg = D_REPO_PROCESS_OK;
                                break;

                            case BookInfo.D_BOOK_KIND_HARD: // ハードカバー
                                // 製本形態情報設定
                                addBook.kind = BookInfo.S_BOOK_KIND_HARD;
                                kindEnd = true;
                                endFlg = D_REPO_PROCESS_OK;
                                break;

                            case BookInfo.D_BOOK_KIND_DIGITAL:  // 電子書籍
                                // 製本形態情報設定
                                addBook.kind = BookInfo.S_BOOK_KIND_DIGITAL;
                                kindEnd = true;
                                endFlg = D_REPO_PROCESS_OK;
                                break;

                            case BookInfo.D_BOOK_KIND_OTHER:    // その他
                                // 製本形態情報設定
                                addBook.kind = BookInfo.S_BOOK_KIND_OTHER;
                                kindEnd = true;
                                endFlg = D_REPO_PROCESS_OK;
                                break;

                            default:
                                Console.WriteLine("");
                                Console.WriteLine($"製本形態は半角整数 [ {BookInfo.D_BOOK_KIND_SOFT} ~ {BookInfo.D_BOOK_KIND_OTHER} ] のみ有効です。");
                                Console.WriteLine("再入力してください。");
                                Console.WriteLine("");
                                Console.Write("Press Enter Key...");
                                Console.ReadLine();
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine($"製本形態は半角整数 [ {BookInfo.D_BOOK_KIND_SOFT} ~ {BookInfo.D_BOOK_KIND_OTHER} ] で指定してください。");
                        Console.WriteLine("再入力してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                }
                // "Cancel"が入力されたか？
                else if (result == "Cancel")
                {
                    kindEnd = true;
                    endFlg = D_REPO_PROCESS_BREAK;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("先頭文字が空白以外、かつ空白以外で最低1文字以上の文字列入力が必要です。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("null文字が入力されています。");
                Console.WriteLine("再入力してください。");
                Console.WriteLine("");
                Console.Write("Press Enter Key...");
                Console.ReadLine();
            }
        }

        return endFlg;
    }

    // 書籍情報で保有する情報のうち、ジャンルの情報を作成するメソッド
    // 処理概要：コンソールで入力した情報を基に、書籍情報で保有するジャンル情報を作成します。
    public int MakeBookGenre(BookInfo addBook)
    {
        string? result; // ユーザ入力値
        bool genreEnd = false;  // ジャンル情報設定処理終了判定
        int endFlg = D_REPO_PROCESS_NG; // 処理結果判定フラグ

        while (genreEnd == false)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("※登録処理を中断する場合は[ Cancel ]を入力してください。");
            Console.Write("登録したい書籍のジャンルを入力してください : ");

            result = Console.ReadLine();

            // nullチェック
            if (result != null)
            {
                // "Cancel"以外の有効値が入力されたか？
                if ((result != "") && (result.Replace(" ", "") != "") && (result.StartsWith(" ") == false) && (result != "Cancel"))
                {
                    // ジャンル情報設定
                    string addGenre = result;
                    addBook.genre = addGenre;
                    genreEnd = true;
                    endFlg = D_REPO_PROCESS_OK;
                }
                // "Cancel"が入力されたか？
                else if (result == "Cancel")
                {
                    genreEnd = true;
                    endFlg = D_REPO_PROCESS_BREAK;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("先頭文字が空白以外、かつ空白以外で最低1文字以上の文字列入力が必要です。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("null文字が入力されています。");
                Console.WriteLine("再入力してください。");
                Console.WriteLine("");
                Console.Write("Press Enter Key...");
                Console.ReadLine();
            }
        }

        return endFlg;
    }

    // 書籍情報で保有する情報のうち、価格の情報を作成するメソッド
    // 処理概要：コンソールで入力した情報を基に、書籍情報で保有する価格情報を作成します。
    public int MakeBookPrice(BookInfo addBook)
    {
        string? result; // ユーザ入力値
        bool priceEnd = false;  // 価格情報設定処理終了判定
        int endFlg = D_REPO_PROCESS_NG; // 処理結果判定フラグ

        while (priceEnd == false)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("※登録処理を中断する場合は[ Cancel ]を入力してください。");
            Console.Write($"登録したい書籍の価格(半角整数：最低 {BookInfo.D_PRICE_MIN} 円以上)を入力してください : ");

            result = Console.ReadLine();

            // nullチェック
            if (result != null)
            {
                // "Cancel"以外の有効値が入力されたか？
                if ((result != "") && (result.Replace(" ", "") != "") && (result.StartsWith(" ") == false) && (result != "Cancel"))
                {
                    string addPrice = result;

                    // 入力値は整数値か？
                    if (long.TryParse(addPrice, out long price) == true)
                    {
                        // 入力値に最低価格が設定されているか？
                        if (price >= BookInfo.D_PRICE_MIN)
                        {
                            // 価格情報設定 
                            addBook.price = addPrice;
                            priceEnd = true;
                            endFlg = D_REPO_PROCESS_OK;
                        }
                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"書籍価格には最低 {BookInfo.D_PRICE_MIN} 円以上を指定してください。");
                            Console.WriteLine("再入力してください。");
                            Console.WriteLine("");
                            Console.Write("Press Enter Key...");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine($"書籍価格は半角整数の入力のみ有効です。");
                        Console.WriteLine("再入力してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                }
                // "Cancel"が入力されたか？
                else if (result == "Cancel")
                {
                    priceEnd = true;
                    endFlg = D_REPO_PROCESS_BREAK;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine($"先頭文字が空白以外、かつ最低 {BookInfo.D_ISBN_MAX} 桁の半角整数によるコード入力が必要です。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("null文字が入力されています。");
                Console.WriteLine("再入力してください。");
                Console.WriteLine("");
                Console.Write("Press Enter Key...");
                Console.ReadLine();
            }
        }

        return endFlg;
    }

    // 書籍情報で保有する情報のうち、JANコードの情報を作成するメソッド
    // 処理概要：コンソールで入力した情報を基に、書籍情報で保有するJANコード情報を作成します。
    public int MakeBookJan(BookInfo addBook)
    {
        string? result; // ユーザ入力値
        bool janEnd = false;    // JANコード情報設定処理終了判定
        int endFlg = D_REPO_PROCESS_NG; // 処理結果判定フラグ 

        while (janEnd == false)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("※登録処理を中断する場合は[ Cancel ]を入力してください。");
            Console.Write("登録したい書籍のJANコード(半角整数13桁)を入力してください : ");

            result = Console.ReadLine();

            // nullチェック
            if (result != null)
            {
                // "Cancel"以外の有効値が入力されたか？
                if ((result != "") && (result.Replace(" ", "") != "") && (result.StartsWith(" ") == false) && (result != "Cancel"))
                {
                    string addJan = result;

                    // 追加するJANコードの桁数は13桁か？
                    if (addJan.Length == BookInfo.D_JAN_LONG_MAX)
                    {
                        // 入力値は整数値か？
                        if (long.TryParse(addJan, out long isbn) == true)
                        {
                            // JANコード情報設定
                            addBook.jan = addJan;
                            janEnd = true;
                            endFlg = D_REPO_PROCESS_OK;
                        }
                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"JANコードは半角整数 {BookInfo.D_JAN_LONG_MAX} 桁のコードのみ有効です。");
                            Console.WriteLine("再入力してください。");
                            Console.WriteLine("");
                            Console.Write("Press Enter Key...");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine($"JANコードは {BookInfo.D_JAN_LONG_MAX} 桁の半角整数コードのみ有効です。");
                        Console.WriteLine("再入力してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                }
                // "Cancel"が入力されたか？
                else if (result == "Cancel")
                {
                    janEnd = true;
                    endFlg = D_REPO_PROCESS_BREAK;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine($"先頭文字が空白以外、かつ最低 {BookInfo.D_JAN_LONG_MAX} 桁の半角整数によるコード入力が必要です。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("null文字が入力されています。");
                Console.WriteLine("再入力してください。");
                Console.WriteLine("");
                Console.Write("Press Enter Key...");
                Console.ReadLine();
            }
        }

        return endFlg;
    }

    // ユーザがコンソール上で指定した、OrderedDictionary型のコレクションで保有する書籍情報を更新するメソッド
    // 処理概要：コンソール上で指定した番号の書籍情報をコレクションより参照し、書籍情報の更新処理を実行。外部出力としてテキストに更新処理後の書籍一覧情報を出力します。
    public void UpdateBookInfo(OrderedDictionary<string, BookInfo> bookDataDict)
    {
        // インスタンス作成
        BookInfoDisplay infoDisp = new BookInfoDisplay();
        BookFileWriter writers = new BookFileWriter();

        string? result; // ユーザ入力値
        int dictCount = 0;  // コレクション数
        string updateKey = "";  // 更新対象キー情報

        dictCount = bookDataDict.Count;

        // コレクションは情報を保有しているか？
        if (dictCount > 0)
        {
            bool updateEnd = false;

            while (updateEnd == false)
            {
                infoDisp.DispAllBooks(bookDataDict, BookInfo.D_READLIST_COUNT_MAX); // 書籍情報一覧表示

                Console.Write("");
                Console.WriteLine("※更新処理を中断する場合は[ Cancel ]を入力してください。");
                Console.Write("更新したい書籍情報の番号（ [ ** / 10 ] の  ** に表示される番号）を入力してください。 : ");
                result = Console.ReadLine();

                // nullチェック
                if (result != null)
                {
                    // ユーザ入力値は整数値か？
                    if (int.TryParse(result, out int idx))
                    {
                        // 入力値は有効な整数値か？
                        if ((idx <= dictCount) && (idx >= BookInfo.D_READLIST_COUNT_MIN))
                        {
                            updateKey = bookDataDict.ElementAt(idx - 1).Key;

                            string selectItem = "";
                            int errorChk = D_REPO_PROCESS_OK;
                            bool updateChk = false;

                            BookInfo updateInfo = new BookInfo();
                            CopyBookInfo(bookDataDict[updateKey], updateInfo);  // 更新前書籍情報コピー

                            while (updateEnd == false)
                            {
                                Console.Clear();
                                Console.WriteLine("");
                                infoDisp.BookInfoShow(updateInfo);  // 書籍情報表示

                                Console.WriteLine("更新したい書籍の項目を選択してください。");
                                Console.WriteLine("[1]タイトル [2]ISBNコード [3]著者 [4]製本形態 [5]ジャンル [6]価格 [7]JANコード [e]更新終了");
                                Console.Write("input No : ");

                                result = Console.ReadLine();

                                // nullチェック
                                if (result != null)
                                {
                                    selectItem = result;

                                    // 選択アイテムは？
                                    switch (selectItem)
                                    {
                                        case "1":   // タイトル情報
                                            errorChk = MakeBookTitle(updateInfo);
                                            break;
                                        case "2":   // ISBNコード情報
                                            errorChk = MakeBookIsbn(bookDataDict, updateInfo);
                                            break;
                                        case "3":   // 著者情報
                                            errorChk = MakeBookPublisher(updateInfo);
                                            break;
                                        case "4":   // 製本形態情報
                                            errorChk = MakeBookKind(updateInfo);
                                            break;
                                        case "5":   // ジャンル情報
                                            errorChk = MakeBookGenre(updateInfo);
                                            break;
                                        case "6":   // 価格情報
                                            errorChk = MakeBookPrice(updateInfo);
                                            break;
                                        case "7":   // JANコード情報
                                            errorChk = MakeBookJan(updateInfo);
                                            break;
                                        case "e":   // 更新処理終了
                                            updateEnd = true;
                                            updateChk = CheckBookInfoUpdate(bookDataDict[updateKey], updateInfo);
                                            break;

                                        default:
                                            Console.Clear();
                                            Console.WriteLine("");
                                            Console.WriteLine($"無効な入力です。(入力値[ {result} ])");
                                            Console.WriteLine("入力値を確認してください。");
                                            Console.WriteLine("");
                                            Console.Write("Press Enter Key...");
                                            Console.ReadLine();
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("");
                                    Console.WriteLine("null文字が入力されています。");
                                    Console.WriteLine("再入力してください。");
                                    Console.WriteLine("");
                                    Console.Write("Press Enter Key...");
                                    Console.ReadLine();
                                }

                            }

                            // 書籍情報は更新ありか？
                            if (updateChk == true)
                            {
                                CopyBookInfo(updateInfo, bookDataDict[updateKey]);  // 更新後書籍情報格納

                                infoDisp.DispAllBooks(bookDataDict, BookInfo.D_READLIST_COUNT_MAX); // 書籍情報一覧表示

                                try
                                {
                                    writers.WriteDictionaryToText(bookDataDict, Program.W_TEXT_BOOKS, BookInfo.D_WRITELIST_COUNT_MAX);  // 書籍情報テキスト書き出し
                                }
                                catch (IOException ioEx)
                                {
                                    throw new IOException("UpdateBookInfo:IOException", ioEx);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("UpdateBookInfo:Exception", ex);
                                }

                                Console.WriteLine("");
                                Console.WriteLine("該当情報を更新しました。");
                                Console.WriteLine("");
                                Console.Write("Press Enter Key...");
                                Console.ReadLine();
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("");
                                Console.WriteLine("該当情報に変更はありませんでした。");
                                Console.WriteLine("");
                                Console.Write("Press Enter Key...");
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("");
                            Console.WriteLine($"無効な番号が入力されました。(入力値[ {result} ])");
                            Console.WriteLine("入力値を確認してください。");
                            Console.WriteLine("");
                            Console.Write("Press Enter Key...");
                            Console.ReadLine();
                        }
                    }
                    // ユーザ入力値は"Cancelか？"
                    else if (result.Replace(" ", "") == "Cancel")
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine($"[ {result} ]が入力されました。");
                        Console.WriteLine("処理を中断します。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine($"無効なデータが入力されました。(入力値[ {result} ])");
                        Console.WriteLine("入力値を確認してください。");
                        Console.WriteLine("");
                        Console.Write("Press Enter Key...");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("");
                    Console.WriteLine("null文字が入力されています。");
                    Console.WriteLine("再入力してください。");
                    Console.WriteLine("");
                    Console.Write("Press Enter Key...");
                    Console.ReadLine();
                }
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

        Console.Clear();
        Console.WriteLine("");
        Console.WriteLine("更新処理を終了します。");
        Console.WriteLine("");
        Console.Write("Press Enter Key...");
        Console.ReadLine();
        Console.Clear();
    }

    // 書籍情報のコピーを行うメソッド
    // 処理概要：引数に指定された書籍情報を基に、コピー元からコピー先へ各種書籍情報を設定します。
    public void CopyBookInfo(BookInfo baseInfo, BookInfo destInfo)
    {
        // コピー情報
        string destTitle = "";      // タイトル情報
        string destIsbn = "";       // ISBNコード
        string destPublisher = "";  // 著者
        string destKind = "";       // 製本形態
        string destGenre = "";      // ジャンル
        string destPrice = "";      // 価格
        string destJan = "";        // JANコード

        // コピー元より情報取得
        destTitle = baseInfo.title; // タイトル情報
        destIsbn = baseInfo.isbn;   // ISBNコード
        destPublisher = baseInfo.publisher; // 著者
        destKind = baseInfo.kind;   // 製本形態
        destGenre = baseInfo.genre; // ジャンル
        destPrice = baseInfo.price; // 価格
        destJan = baseInfo.jan;     // JANコード

        // コピー先に情報格納
        destInfo.title = destTitle; // タイトル情報
        destInfo.isbn = destIsbn;   // ISBNコード
        destInfo.publisher = destPublisher; // 著者
        destInfo.kind = destKind;   // 製本形態
        destInfo.genre = destGenre; // ジャンル
        destInfo.price = destPrice; // 価格
        destInfo.jan = destJan;     // JANコード
    }

    // 書籍情報の更新有無をチェックするメソッド
    // 処理概要：引数に指定された書籍情報を基に、更新前と更新後の各種書籍情報を比較し更新の有無をチェックします。
    public bool CheckBookInfoUpdate(BookInfo baseInfo, BookInfo chkInfo)
    {
        bool result = true;

        // タイトル情報は一致するか？
        if (baseInfo.title == chkInfo.title)
        {
            // ISBNコード情報は一致するか？
            if (baseInfo.isbn == chkInfo.isbn)
            {
                // 著者情報は一致するか？
                if (baseInfo.publisher == chkInfo.publisher)
                {
                    // 製本形態情報は一致するか？
                    if (baseInfo.kind == chkInfo.kind)
                    {
                        // ジャンル情報は一致するか？
                        if (baseInfo.genre == chkInfo.genre)
                        {
                            // 価格情報は一致するか？
                            if (baseInfo.price == chkInfo.price)
                            {
                                // JANコード情報は一致するか？
                                if (baseInfo.jan == chkInfo.jan)
                                {
                                    result = false; // 更新なし
                                }
                            }
                        }
                    }
                }
            }

        }

        return result;
    }
}