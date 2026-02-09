namespace ToshoKanri.Models;

public class BookInfo   // 書籍情報
{
    // 定数(数値)
    public const int D_READLIST_COUNT_MIN = 1;  // 書籍データのリスト最小保有件数
    public const int D_READLIST_COUNT_MAX = 10; // 書籍データのリスト最大保有件数
    public const int D_WRITELIST_COUNT_MAX = 10; // 書籍データの書き込み最大件数
    public const int D_BOOK_KIND_SOFT = 1;      // 書籍の製本形態(ソフトカバー)
    public const int D_BOOK_KIND_HARD = 2;      // 書籍の製本形態(ソフトカバー)
    public const int D_BOOK_KIND_DIGITAL = 3;   // 書籍の製本形態(ソフトカバー)
    public const int D_BOOK_KIND_OTHER = 4;     // 書籍の製本形態(ソフトカバー)
    public const int D_READTEXT_LINE_MAX = 100; // 書籍データのテキスト最大行数
    public const int D_ISBN_MAX = 13;           // ISBNコード最大桁数
    public const int D_PRICE_MIN = 1;           // 最低価格
    public const int D_JAN_LONG_MAX = 13;       // JANコード(標準)最大桁数
    public const int D_BOOK_ELEMENT_MAX = 7;    // 書籍最大要素数(0base)
    public const int D_READ_INDEX_ISBN = 0;    // テキスト読み込み時Index(ISBNコード)
    public const int D_READ_INDEX_TITLE = 1;    // テキスト読み込み時Index(タイトル)
    public const int D_READ_INDEX_PUBLISHER = 2; // テキスト読み込み時Index(著者)
    public const int D_READ_INDEX_KIND = 3;     // テキスト読み込み時Index(製本形態)
    public const int D_READ_INDEX_GENRE = 4;    // テキスト読み込み時Index(ジャンル)
    public const int D_READ_INDEX_PRICE = 5;    // テキスト読み込み時Index(価格)
    public const int D_READ_INDEX_JAN = 6;      // テキスト読み込み時Index(JANコード)
    
    // 定数(文字列)
    public const string S_BOOK_KIND_SOFT = "ソフトカバー";  // 製本形態文字列(ソフトカバー)
    public const string S_BOOK_KIND_HARD = "ハードカバー";  // 製本形態文字列(ハードカバー)
    public const string S_BOOK_KIND_DIGITAL = "電子書籍";   // 製本形態文字列(電子書籍)
    public const string S_BOOK_KIND_OTHER = "その他";       // 製本形態文字列(その他)

    // 定数(bool)
    public const bool B_ADD_DATA_WITH = true;       // データ追加あり
    public const bool B_ADD_DATA_WITHOUT = false;   // データ追加なし

    // 書籍情報
    public string title { get; set; }       // タイトル
    public string isbn { get; set; }        // ISBNコード(13桁)
    public string publisher { get; set; }   // 著者 
    public string kind { get; set; }        // 製本形態
    public string genre { get; set; }       // ジャンル
    public string price { get; set; }       // 価格
    public string jan { get; set; }         // JANコード(13桁)
}


