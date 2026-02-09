using System.Data;
using ToshoKanri.Models;

namespace ToshoKanri.Services;

public class BookFileReader
{
    // テキストファイルからデータを読み込み、List型のコレクションを作成するメソッド
    // 処理概要：テキストファイルから文字列を1行ずつ読み込み、List型のコレクションへ格納します。
    public List<string> ReadTextToList(string textName, int readLineMax)
    {
        List<string> resultData = new();    // 読み出し情報格納リスト

        try
        {
            using (var reader = new StreamReader(textName))
            {
                string? line;
                int count = 0;

                while ((line = reader.ReadLine()) != null)
                {

                    resultData.Add(line);
                    count++;

                    // テキスト最大行数分読み込んだか？
                    if (count == readLineMax)
                    {
                        break;
                    }
                }
            }
        }
        catch (FileNotFoundException fnfex)
        {
            Console.WriteLine("予期せぬエラーが発生しました。");
            Console.WriteLine("エラーログを出力します。");
            Console.WriteLine("[ErrorLog Start]====================================================");
            Console.WriteLine(fnfex);
            Console.WriteLine("[ErrorLog End...]===================================================");
            Console.WriteLine("");

            throw new FileNotFoundException("ReadTextToList:FileNotFoundException", fnfex);
        }
        catch (Exception ex)
        {
            Console.WriteLine("予期せぬエラーが発生しました。");
            Console.WriteLine("エラーログを出力します。");
            Console.WriteLine("[ErrorLog Start]====================================================");
            Console.WriteLine(ex);
            Console.WriteLine("[ErrorLog End...]===================================================");
            Console.WriteLine("");

            throw new Exception("ReadTextToList:Exception", ex);
        }

        return resultData;
    }

    // テキストファイルから作成したList型コレクションを読み込み、OrderedDictionary型のコレクションを作成するメソッド
    // 処理概要：テキストファイルから作成したList型コレクションを先頭から読み込み、付加情報を基に情報を判別。
    //          書籍情報を作成し、コレクションへ格納します。
    public OrderedDictionary<string, BookInfo> ReadListToDictionary(string textName, int readLineMax)
    {
        List<string> readList = new();  // 読み出し情報格納リスト
        OrderedDictionary<string, BookInfo> resultData = new(); // 読み出し情報格納コレクション(Dictionary型)
        string checkWord = "";  // 付加情報格納変数
        string addKeys = "";    // コレクション格納キー
        bool duplicateChk = false;  // 重複判定用変数
        BookInfo bookData;  // 書籍情報

        try
        {
            readList = ReadTextToList(textName, readLineMax);   // 書籍情報テキスト読み出し情報リスト変換
            int readCount = 0;  // 書籍情報読み込み件数
            bool[] addCheck = new bool[BookInfo.D_BOOK_ELEMENT_MAX];    // 各種情報追加判定用変数
            bool readEnd = false;   // リスト読み込み終了フラグ

            foreach (string read in readList)
            {
                // nullチェック
                if (read != null)
                {
                    checkWord = read.Substring(0, 4);

                    // 付加情報判定
                    switch (checkWord)
                    {
                        case "[01]":    // ISBNコード
                            readCount++;

                            // 読み込んだ書籍情報はリスト最大保有件数以下か？
                            if (readCount <= BookInfo.D_READLIST_COUNT_MAX)
                            {
                                Array.Fill(addCheck, BookInfo.B_ADD_DATA_WITHOUT);

                                addKeys = read.Substring(4);
                                duplicateChk = false;
                                bookData = new BookInfo();

                                // キー情報の重複確認
                                foreach (string result in resultData.Keys)
                                {
                                    // 追加するキー情報は登録済か？
                                    if (result == addKeys)
                                    {
                                        duplicateChk = true;
                                        break;
                                    }
                                }

                                // キー情報の重複はなしか？
                                if (duplicateChk == false)
                                {
                                    resultData.Add(addKeys, bookData);
                                    resultData[addKeys].isbn = addKeys;

                                    addCheck[BookInfo.D_READ_INDEX_ISBN] = BookInfo.B_ADD_DATA_WITH;
                                }
                            }
                            else
                            {
                                readEnd = true;
                            }
                            break;

                        case "[02]":    // タイトル
                            // ISBNコード追加済み、かつタイトル情報が未追加か？
                            if ((addCheck[BookInfo.D_READ_INDEX_ISBN] == BookInfo.B_ADD_DATA_WITH) && (addCheck[BookInfo.D_READ_INDEX_TITLE] == BookInfo.B_ADD_DATA_WITHOUT))
                            {
                                resultData[addKeys].title = read.Substring(4);
                                addCheck[BookInfo.D_READ_INDEX_TITLE] = BookInfo.B_ADD_DATA_WITH;
                            }
                            break;

                        case "[03]":    // 著者
                            // ISBNコード追加済み、かつ著者情報が未追加か？
                            if ((addCheck[BookInfo.D_READ_INDEX_ISBN] == BookInfo.B_ADD_DATA_WITH) && (addCheck[BookInfo.D_READ_INDEX_PUBLISHER] == BookInfo.B_ADD_DATA_WITHOUT))
                            {
                                resultData[addKeys].publisher = read.Substring(4);
                                addCheck[BookInfo.D_READ_INDEX_PUBLISHER] = BookInfo.B_ADD_DATA_WITH;
                            }
                            break;

                        case "[04]":    // 製本形態
                            // ISBNコード追加済み、かつ製本形態情報が未追加か？
                            if ((addCheck[BookInfo.D_READ_INDEX_ISBN] == BookInfo.B_ADD_DATA_WITH) && (addCheck[BookInfo.D_READ_INDEX_KIND] == BookInfo.B_ADD_DATA_WITHOUT))
                            {
                                resultData[addKeys].kind = read.Substring(4);
                                addCheck[BookInfo.D_READ_INDEX_KIND] = BookInfo.B_ADD_DATA_WITH;
                            }
                            break;

                        case "[05]":    // ジャンル
                            // ISBNコード追加済み、かつジャンル情報が未追加か？
                            if ((addCheck[BookInfo.D_READ_INDEX_ISBN] == BookInfo.B_ADD_DATA_WITH) && (addCheck[BookInfo.D_READ_INDEX_GENRE] == BookInfo.B_ADD_DATA_WITHOUT))
                            {
                                resultData[addKeys].genre = read.Substring(4);
                                addCheck[BookInfo.D_READ_INDEX_GENRE] = BookInfo.B_ADD_DATA_WITH;
                            }
                            break;

                        case "[06]":    // 価格
                            // ISBNコード追加済み、かつ価格情報が未追加か？
                            if ((addCheck[BookInfo.D_READ_INDEX_ISBN] == BookInfo.B_ADD_DATA_WITH) && (addCheck[BookInfo.D_READ_INDEX_PRICE] == BookInfo.B_ADD_DATA_WITHOUT))
                            {
                                resultData[addKeys].price = read.Substring(4);
                                addCheck[BookInfo.D_READ_INDEX_PRICE] = BookInfo.B_ADD_DATA_WITH;
                            }
                            break;

                        case "[07]":    // JANコード
                            // ISBNコード追加済み、かつJANコード情報が未追加か？
                            if ((addCheck[BookInfo.D_READ_INDEX_ISBN] == BookInfo.B_ADD_DATA_WITH) && (addCheck[BookInfo.D_READ_INDEX_JAN] == BookInfo.B_ADD_DATA_WITHOUT))
                            {
                                // 製本形態情報が追加済、かつ"電子書籍"か？
                                if ((addCheck[BookInfo.D_READ_INDEX_KIND] == BookInfo.B_ADD_DATA_WITH) && (resultData[addKeys].kind == BookInfo.S_BOOK_KIND_DIGITAL))
                                {
                                    resultData[addKeys].jan = "";
                                }
                                // 製本形態情報が追加済、かつ"電子書籍以外"か？
                                else if ((addCheck[BookInfo.D_READ_INDEX_KIND] == BookInfo.B_ADD_DATA_WITH) && (resultData[addKeys].kind != BookInfo.S_BOOK_KIND_DIGITAL))
                                {
                                    resultData[addKeys].jan = read.Substring(4);
                                }
                                addCheck[BookInfo.D_READ_INDEX_JAN] = BookInfo.B_ADD_DATA_WITH;
                            }
                            break;

                        case "[CO]":    // 読み込みデータ区切り文字
                            Array.Fill(addCheck, BookInfo.B_ADD_DATA_WITHOUT);  // 各種情報追加判定用変数初期化
                            break;

                        case "[EN]":    // 読み込みデータ終端文字
                            readEnd = true;
                            break;

                        default:
                            break;
                    }

                    // リスト読み込み終了か？
                    if (readEnd == true)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

        }
        catch (FileNotFoundException fnfex)
        {
            throw new FileNotFoundException("ReadListToDictionary:FileNotFoundException", fnfex);
        }
        catch (Exception ex)
        {
            throw new Exception("ReadListToDictionary:Exception", ex);
        }

        return resultData;
    }

}

