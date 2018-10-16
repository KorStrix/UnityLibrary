#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-02 오후 3:52:16
 *	기능 : 
 *	
 *	도움이 된 코드 링크
 *	http://plog2012.blogspot.kr/2012/11/c.html
 *	http://ehclub.co.kr/2484?category=658554
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using NUnit.Framework;
using UnityEngine.TestTools;

#pragma warning disable CS0660 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.Equals(object o)를 재정의하지 않습니다.
#pragma warning disable CS0661 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.GetHashCode()를 재정의하지 않습니다.

public class SHangulString
{
    StringBuilder _pStrBuilder = new StringBuilder();

    public LinkedList<SHangulChar> _listHangulChar { get; private set; }

    public SHangulChar _chLastHangul { get { return _listHangulChar.Last.Value; } }

    public int Count
    {
        get { return _listHangulChar.Count; }
    }

    public SHangulString()
    {
        _listHangulChar = new LinkedList<SHangulChar>();
    }

    public SHangulString(IEnumerable<SHangulChar> pIterHangul)
    {
        _listHangulChar = new LinkedList<SHangulChar>(pIterHangul);
    }

    public SHangulString(string strText)
    {
        _listHangulChar = new LinkedList<SHangulChar>();
        for (int i = 0; i < strText.Length; i++)
            _listHangulChar.AddLast(SHangulChar.ConvertToHangul(strText[i]));
    }


    public SHangulString DoInsert(char ch한글)
    {
        // Case 1 - 빈 문자열이면 무조건 추가입력.
        // Case 2 - 입력 실패의 경우, 
        //  Case 2.1 - 입력한 글자가 자음일 때 뒤에 추가로 입력.
        //  Case 2.2 - 입력한 글자가 모음일 때 전의 글자의 종성이 있으면 종성을 빼오고 모음 입력.
        //  Case 2.3 - 입력한 글자가 모음일 때 전의 글자의 종성이 없으면 뒤에 추가로 입력.
        // Case 3 - 입력에 성공한 경우, 마지막 글자를 추가한 글자로 갱신


        // Case 1 - 빈 문자열이면 무조건 추가입력.
        if (_listHangulChar.Count == 0)
        {
            _listHangulChar.AddLast(SHangulChar.Empty.DoInsert(ch한글));
            return this;
        }

        SHangulChar chNewHangul = _chLastHangul.DoInsert(ch한글);
        if (chNewHangul.p_bHas한글아닌글자) // 한글이 아닌 경우에도 추가입력
        {
            _listHangulChar.AddLast(SHangulChar.Empty.DoInsert(ch한글));
            return this;
        }

        bool bIsFail_InsertHangul = chNewHangul == _chLastHangul;
        if (bIsFail_InsertHangul)
        {
            if (SHangulChar.CheckIs자음(ch한글)) // Case 2.1 - 입력한 글자가 자음일 때 뒤에 추가로 입력.
            {
                _listHangulChar.AddLast(SHangulChar.Empty.DoInsert(ch한글));
            }
            else

            // Case 2.2 - 입력한 글자가 모음일 때 전의 글자의 종성이 있으면 종성을 빼오고 모음 입력.
            if (chNewHangul.p_bHas종성)
            {
                char ch직전글자의종성 = chNewHangul.p_ch종성;
                char ch직전글자의종성_왼쪽, ch직전글자의종성_오른쪽;
                if(SHangulChar.CheckIs이중종성(ch직전글자의종성, out ch직전글자의종성_왼쪽, out ch직전글자의종성_오른쪽))
                    ch직전글자의종성 = ch직전글자의종성_오른쪽;
                
                _listHangulChar.Last.Value = chNewHangul.DoRemove();
                _listHangulChar.AddLast(SHangulChar.ConvertToHangul(ch직전글자의종성).DoInsert(ch한글)); 
            }
            else // Case 2.3 - 입력한 글자가 모음일 때 전의 글자의 종성이 없으면 뒤에 추가로 입력.
                _listHangulChar.AddLast(SHangulChar.Empty.DoInsert(ch한글));
        }
        else // Case 3 - 입력에 성공한 경우, 마지막 글자를 추가한 글자로 갱신
            _listHangulChar.Last.Value = chNewHangul;

        return this;
    }

    public SHangulString DoRemove()
    {
        if (_listHangulChar.Count == 0)
            return this;

        SHangulChar chNewHangul = _chLastHangul.DoRemove();
        if (chNewHangul == SHangulChar.Empty)
            _listHangulChar.RemoveLast();
        else
            _listHangulChar.Last.Value = chNewHangul;

        return this;
    }

    public override string ToString()
    {
        _pStrBuilder.Length = 0;
        foreach (SHangulChar pHangulChar in _listHangulChar)
            _pStrBuilder.Append(pHangulChar.ToString());

        return _pStrBuilder.ToString();
    }


}

public struct SHangulChar
{
    /* const & readonly declaration             */

    // 초성, 중성, 종성 테이블.
    static private readonly string const_str초성Table = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
    static private readonly string const_str중성Table = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
    static private readonly string const_str종성Table = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

    static private readonly Dictionary<char, char> const_map이중모음분리용Table = new Dictionary<char, char>()
    {
        { 'ㅘ', 'ㅗ' }, { 'ㅙ', 'ㅗ' }, { 'ㅚ', 'ㅗ'}, { 'ㅝ', 'ㅜ'}, { 'ㅞ', 'ㅜ' }, { 'ㅟ', 'ㅜ' }, { 'ㅢ', 'ㅡ'},
    };

    static private readonly Dictionary<KeyValuePair<char, char>, char> const_map이중모음조합용Table = new Dictionary<KeyValuePair<char, char>, char>()
    {
        { new KeyValuePair<char, char>('ㅗ', 'ㅏ'), 'ㅘ' }, { new KeyValuePair<char, char>('ㅗ', 'ㅐ'), 'ㅙ' }, { new KeyValuePair<char, char>('ㅗ', 'ㅣ'), 'ㅚ' },
        { new KeyValuePair<char, char>('ㅜ', 'ㅓ'), 'ㅝ' }, { new KeyValuePair<char, char>('ㅜ', 'ㅔ'), 'ㅞ' }, { new KeyValuePair<char, char>('ㅜ', 'ㅣ'), 'ㅟ' },
        { new KeyValuePair<char, char>('ㅡ', 'ㅣ'), 'ㅢ' }
    };

    static private readonly Dictionary<KeyValuePair<char, char>, char> const_map이중종성조합용Table = new Dictionary<KeyValuePair<char, char>, char>()
    {
        { new KeyValuePair<char, char>('ㄱ', 'ㅅ'), 'ㄳ'}, { new KeyValuePair<char, char>('ㄴ', 'ㅈ'), 'ㄵ'}, { new KeyValuePair<char, char>('ㄴ', 'ㅎ'), 'ㄶ'},
        { new KeyValuePair<char, char>('ㄹ', 'ㄱ'), 'ㄺ'}, { new KeyValuePair<char, char>('ㄹ', 'ㅁ'), 'ㄻ'}, { new KeyValuePair<char, char>('ㄹ', 'ㅂ'), 'ㄼ'},
        { new KeyValuePair<char, char>('ㄹ', 'ㅅ'), 'ㄽ'}, { new KeyValuePair<char, char>('ㄹ', 'ㅌ'), 'ㄾ'}, { new KeyValuePair<char, char>('ㄹ', 'ㅍ'), 'ㄿ'},
        { new KeyValuePair<char, char>('ㄹ', 'ㅎ'), 'ㅀ'}, { new KeyValuePair<char, char>('ㅂ', 'ㅅ'), 'ㅄ'}
    };

    static private readonly Dictionary<char, KeyValuePair<char, char>> const_map이중종성분리용Table = new Dictionary<char, KeyValuePair<char, char>>()
    {
        { 'ㄳ', new KeyValuePair<char, char>('ㄱ', 'ㅅ') }, { 'ㄵ', new KeyValuePair<char, char>('ㄴ', 'ㅈ') }, { 'ㄶ', new KeyValuePair<char, char>('ㄴ', 'ㅎ') },
        { 'ㄺ', new KeyValuePair<char, char>('ㄹ', 'ㄱ') }, { 'ㄻ', new KeyValuePair<char, char>('ㄹ', 'ㅁ') }, { 'ㄼ', new KeyValuePair<char, char>('ㄹ', 'ㅂ') }, { 'ㄽ', new KeyValuePair<char, char>('ㄹ', 'ㅅ') },
        { 'ㄾ', new KeyValuePair<char, char>('ㄹ', 'ㅌ') }, { 'ㄿ', new KeyValuePair<char, char>('ㄹ', 'ㅍ') }, { 'ㅀ', new KeyValuePair<char, char>('ㄹ', 'ㅎ') }, { 'ㅄ', new KeyValuePair<char, char>('ㅂ', 'ㅅ') }
    };

    static private readonly int const_i중성테이블길이 = const_str중성Table.Length;
    static private readonly int const_i종성테이블길이 = const_str종성Table.Length;

    static private readonly ushort const_ushUniCode한글Start = 0xAC00;
    static private readonly ushort const_ushUniCode한글Last = 0xD79F;

    static private char const_chEmpty = '\0';

    /* enum & struct declaration                */

    /* public - Field declaration            */

    static public readonly SHangulChar Empty = new SHangulChar();

    public char p_ch초성 { get; private set; }
    public char p_ch중성 { get; private set; }
    public char p_ch종성 { get; private set; }

    public bool p_bHas초성 { get { return char.Equals(p_ch초성, const_chEmpty) == false; } }
    public bool p_bHas중성 { get { return char.Equals(p_ch중성, const_chEmpty) == false; } }
    public bool p_bHas종성 { get { return char.Equals(p_ch종성, const_chEmpty) == false; } }
    public bool p_bHas한글아닌글자 { get { return char.Equals(p_ch한글아닌글자, const_chEmpty) == false; } }

    public bool p_bIsEmpty { get { return this == Empty; } }
    public char p_ch한글아닌글자 { get; private set; }

    /* protected & private - Field declaration         */

    // ========================================================================== //

    public SHangulChar(char ch한글아닌글자)
    {
        p_ch한글아닌글자 = ch한글아닌글자;
        p_ch초성 = const_chEmpty; p_ch중성 = const_chEmpty; p_ch종성 = const_chEmpty;
    }

    public SHangulChar(char ch초성, char ch중성, char ch종성)
    {
        p_ch초성 = ch초성; p_ch중성 = ch중성; p_ch종성 = ch종성; p_ch한글아닌글자 = const_chEmpty;
    }

    static public SHangulChar ConvertToHangul(string str한글자)
    {
        if (string.IsNullOrEmpty(str한글자)) 
            return Empty;

        return ConvertToHangul(str한글자[0]);
    }

    static public SHangulChar ConvertToHangul(char ch한글자)
    {
        // 캐릭터가 글자가 아닐 경우 처리
        if (CheckIs자음(ch한글자))
            return new SHangulChar(ch한글자, const_chEmpty, const_chEmpty);

        if (CheckIsHangul(ch한글자) == false)
            return new SHangulChar(ch한글자);

        ushort shUniCode = GetUniCode(ch한글자);

        // iUniCode에 한글코드에 대한 유니코드 위치를 담고 이를 이용해 인덱스 계산.
        int iUniCode = shUniCode - const_ushUniCode한글Start;
        int i초성Idx = iUniCode / (const_i중성테이블길이 * const_i종성테이블길이);
        iUniCode = iUniCode % (const_i중성테이블길이 * const_i종성테이블길이);

        int i중성Idx = iUniCode / const_i종성테이블길이;
        iUniCode = iUniCode % const_i종성테이블길이;
        if (iUniCode != 0) // 초성, 중성, 종성이 모두 있는 경우
        {
            int i종성Idx = iUniCode;
            return new SHangulChar(const_str초성Table[i초성Idx], const_str중성Table[i중성Idx], const_str종성Table[i종성Idx]);
        }
        else // 초성, 중성까지 있는 경우
            return new SHangulChar(const_str초성Table[i초성Idx], const_str중성Table[i중성Idx], '\0');
    }

    static public bool operator ==(SHangulChar chHangulA, SHangulChar chHangulB)
    {
        return (char.Equals(chHangulA.p_ch초성, chHangulB.p_ch초성) &&
                char.Equals(chHangulA.p_ch중성, chHangulB.p_ch중성) &&
                char.Equals(chHangulA.p_ch종성, chHangulB.p_ch종성) &&
                char.Equals(chHangulA.p_ch한글아닌글자, chHangulB.p_ch한글아닌글자));
    }

    static public bool operator !=(SHangulChar chHangulA, SHangulChar chHangulB)
    {
        return (chHangulA == chHangulB) == false;
    }

    static public bool CheckIs자음(char ch자음)
    {
        return const_str초성Table.IndexOf(ch자음) != -1;
    }

    static public bool CheckIs모음(char ch모음)
    {
        return const_str중성Table.IndexOf(ch모음) != -1;
    }

    static public bool CheckIs이중종성(char ch종성, out char ch단일자음_왼쪽, out char ch단일자음_오른쪽)
    {
        KeyValuePair<char, char> pair단일자음;
        bool bResult = const_map이중종성분리용Table.TryGetValue(ch종성, out pair단일자음);

        ch단일자음_왼쪽 = pair단일자음.Key;
        ch단일자음_오른쪽 = pair단일자음.Value;

        return bResult;
    }

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public SHangulChar DoInsert(string strText)
    {
        char chText;
        if (char.TryParse(strText, out chText) == false)
            return this;

        return DoInsert(chText);
    }

    public SHangulChar DoInsert(char chText)
    {
        if (p_bHas한글아닌글자) return this;

        // 들어오는 문자가
        // Case 1 - 초성이 없을 경우엔 무조건 삽입
        // Case 2 - 중성이 없을 경우엔 모음인지,
        // Case 3 - 중성이 있을 경우엔 이중 모음이 가능하고, 모음이 들어왔으며, 해당 모음의 이중모음 쌍인지        
        // Case 4 - 종성이 없을 경우엔 자음인지
        // Case 5 - 종성이 있을 경우엔 이중 자음이 가능하고, 자음이 들어왔으며, 해당 자음의 이중자음 쌍인지


        bool b들어온문자는_자음 = CheckIs자음(chText);
        bool b들어온문자는_모음 = CheckIs모음(chText);

        // 자음도, 모음도 아닌 경우는 한글이 아닌 글자
        if (b들어온문자는_자음 == false && b들어온문자는_모음 == false)
            return new SHangulChar(chText);

        // Case 1 - 초성이 없을 경우엔 무조건 삽입

        if ( p_bHas초성 == false )
        {
            if(b들어온문자는_자음)
                return new SHangulChar(chText, const_chEmpty, const_chEmpty);
            else if(p_bHas중성 == false)
                return new SHangulChar(const_chEmpty, chText, const_chEmpty);
        }

        // Case 2 - 중성이 없을 경우엔 모음인지
        if(p_bHas중성 == false)
        {
            if (b들어온문자는_모음)
                return new SHangulChar(p_ch초성, chText, const_chEmpty);
            else
                return this;
        }

        // Case 3 - 중성이 있을 경우엔 이중 모음이 가능하고, 모음이 들어왔으며, 해당 모음의 이중모음 쌍인지    
        // + 종성이 없어야 한다.
        char ch이중모음;
        if (b들어온문자는_모음 && p_bHas중성 && p_bHas종성 == false && Try이중모음조합(p_ch중성, chText, out ch이중모음))
            return new SHangulChar(p_ch초성, ch이중모음, const_chEmpty);

        // Case 4 - 종성이 없을 경우엔 자음인지
        if ( b들어온문자는_자음 && p_bHas종성 == false) 
            return new SHangulChar(p_ch초성, p_ch중성, chText);

        // Case 5 - 종성이 있을 경우엔 이중 자음이 가능하고, 자음이 들어왔으며, 해당 자음의 이중자음 쌍인지
        char ch이중종성;
        if (b들어온문자는_자음 && p_bHas종성 && Try이중종성조합(p_ch종성, chText, out ch이중종성))  
            return new SHangulChar(p_ch초성, p_ch중성, ch이중종성);

        return this;
    }

    public SHangulChar DoRemove()
    {
        if (p_bHas한글아닌글자) return Empty;

        // 지우는 순서는 글자 추가의 역순으로..
        // Case 1 - 이중종성인지, 맞다면 단일자음으로
        // Case 2 - 단일종성인 경우 종성을 없애기
        // Case 3 - 이중모음인지, 맞다면 단일모음으로 
        // Case 4 - 단일모음인 경우 모음을 없애기
        // Case 5 - 초성만 있는 경우 초성을 없애기 ( 그 외의 경우 )

        if(p_bHas종성)
        {
            char ch단일자음_왼쪽, ch단일자음_오른쪽;
            if (CheckIs이중종성(p_ch종성, out ch단일자음_왼쪽, out ch단일자음_오른쪽)) 
                return new SHangulChar(p_ch초성, p_ch중성, ch단일자음_왼쪽); // Case 1 - 이중종성인지, 맞다면 단일자음으로
            else
                return new SHangulChar(p_ch초성, p_ch중성, const_chEmpty); // Case 2 - 단일종성인 경우 종성을 없애기
        }

        if(p_bHas중성)
        {
            char ch단일모음;
            if (CheckIs이중모음(p_ch중성, out ch단일모음))
                return new SHangulChar(p_ch초성, ch단일모음, const_chEmpty);      // Case 3 - 이중모음인지, 맞다면 단일모음으로 
            else
                return new SHangulChar(p_ch초성, const_chEmpty, const_chEmpty); // Case 4 - 단일모음인 경우 모음을 없애기
        }

        return Empty; // Case 5 - 초성만 있는 경우 초성을 없애기(그 외의 경우 )
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */
    
    public override string ToString()
    {
        if (p_bHas한글아닌글자)
        {
            string strPrint = p_ch한글아닌글자.ToString();
            if (strPrint == " ")
                return " ";
            else
                return p_ch한글아닌글자.ToString();
        }

        int i초성위치 = const_str초성Table.IndexOf(p_ch초성);
        int i중성위치 = const_str중성Table.IndexOf(p_ch중성);
        int i종성위치 = const_str종성Table.IndexOf(p_ch종성);

        int iUniCode = -1;
        if (i초성위치 == -1)
        {
            if (i중성위치 == -1)
                return "";
            else // 초성은 없는데 중성만 있는 경우가 있다.
                return p_ch중성.ToString();
        }
        else if (i중성위치 == -1)
            return p_ch초성.ToString();
        else if (i종성위치 == -1)
            iUniCode = const_ushUniCode한글Start + (((i초성위치 * const_i중성테이블길이) + i중성위치) * const_i종성테이블길이);
        else
            iUniCode = const_ushUniCode한글Start + (((i초성위치 * const_i중성테이블길이) + i중성위치) * const_i종성테이블길이) + i종성위치;

        return Convert.ToChar(iUniCode).ToString();
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    /// <summary>
    /// //Char을 16비트 부호없는 정수형 형태로 변환 - Unicode
    /// </summary>
    /// <param name="strText"></param>
    /// <returns></returns>
    static private ushort GetUniCode(string strText)
    {
        return GetUniCode(char.Parse(strText));
    }

    static private ushort GetUniCode(char ch)
    {
        return Convert.ToUInt16(ch);
    }

    static private bool CheckIsHangul(char ch한글자)
    {
        ushort shUniCode = GetUniCode(ch한글자);
        return ((const_ushUniCode한글Start <= shUniCode) && (shUniCode <= const_ushUniCode한글Last));
    }

    static private bool CheckIs이중모음(char ch모음, out char ch단일모음)
    {
        return const_map이중모음분리용Table.TryGetValue(ch모음, out ch단일모음);
    }

    static private bool Try이중모음조합(char ch이중모음왼쪽, char ch이중모음오른쪽, out char ch이중모음완성)
    {
        return const_map이중모음조합용Table.TryGetValue(new KeyValuePair<char, char>(ch이중모음왼쪽, ch이중모음오른쪽), out ch이중모음완성);
    }

    static private bool Try이중종성조합(char ch이중종성왼쪽, char ch이중종성오른쪽, out char ch이중종성완성)
    {
        return const_map이중종성조합용Table.TryGetValue(new KeyValuePair<char, char>(ch이중종성왼쪽, ch이중종성오른쪽), out ch이중종성완성);
    }

    #endregion Private

    // ========================================================================== //
}

#region Test
[Category("StrixLibrary")]
public class 한글파서_테스트
{
    [Test]
    public void 한글컨버팅_테스트()
    {
        Assert.AreEqual("ㄱ", SHangulChar.ConvertToHangul("ㄱ").ToString());
        Assert.AreEqual("가", SHangulChar.ConvertToHangul("가").ToString());
        Assert.AreEqual("각", SHangulChar.ConvertToHangul("각").ToString());
        Assert.AreEqual("갂", SHangulChar.ConvertToHangul("갂").ToString());

        Assert.AreEqual("ㄲ", SHangulChar.ConvertToHangul("ㄲ").ToString());
        Assert.AreEqual("꺼", SHangulChar.ConvertToHangul("꺼").ToString());
        Assert.AreEqual("꺽", SHangulChar.ConvertToHangul("꺽").ToString());
        Assert.AreEqual("꺾", SHangulChar.ConvertToHangul("꺾").ToString());

        // 두글자 이상인 경우 첫글자만
        Assert.AreEqual("꺾", SHangulChar.ConvertToHangul("꺾ㄷ").ToString());
        Assert.AreEqual("ㄲ", SHangulChar.ConvertToHangul("ㄲㄲ").ToString());
    }

    [Test]
    public void 한글컨버팅_완성형만_좀더테스트()
    {
        테스트_자소나누고합치기("테"); 테스트_자소나누고합치기("스"); 테스트_자소나누고합치기("트");
        테스트_자소나누고합치기("가"); 테스트_자소나누고합치기("나"); 테스트_자소나누고합치기("다");
        테스트_자소나누고합치기("동"); 테스트_자소나누고합치기("해"); 테스트_자소나누고합치기("물");
        테스트_자소나누고합치기("백"); 테스트_자소나누고합치기("두"); 테스트_자소나누고합치기("산");

        테스트_자소나누고합치기("밟");
        테스트_자소나누고합치기("꿹");
        테스트_자소나누고합치기("꿟");
        테스트_자소나누고합치기("뛟");
    }

    [Test]
    public void 한글작성하기_테스트()
    {
        SHangulChar pHangul = SHangulChar.Empty;

        Assert.AreEqual("ㄱ", pHangul.DoInsert("ㄱ").ToString());
        Assert.AreEqual("가", pHangul.DoInsert("ㄱ").DoInsert("ㅏ").ToString());
        Assert.AreEqual("각", pHangul.DoInsert("ㄱ").DoInsert("ㅏ").DoInsert("ㄱ").ToString());


        Assert.AreEqual("ㅂ", pHangul.DoInsert("ㅂ").ToString());
        Assert.AreEqual("붸", pHangul.DoInsert("ㅂ").DoInsert("ㅜ").DoInsert("ㅔ").ToString());
        Assert.AreEqual("뷀", pHangul.DoInsert("ㅂ").DoInsert("ㅜ").DoInsert("ㅔ").DoInsert("ㄹ").ToString());
        Assert.AreEqual("뷁", pHangul.DoInsert("ㅂ").DoInsert("ㅜ").DoInsert("ㅔ").DoInsert("ㄹ").DoInsert("ㄱ").ToString());


        Assert.AreEqual("ㄲ", pHangul.DoInsert("ㄲ").ToString());
        Assert.AreEqual("꺄", pHangul.DoInsert("ㄲ").DoInsert("ㅑ").ToString());
        Assert.AreEqual("꺆", pHangul.DoInsert("ㄲ").DoInsert("ㅑ").DoInsert("ㄲ").ToString());

        //잘못된 케이스의 경우 마지막에서 변동사항 없음
        Assert.AreEqual("각", pHangul.DoInsert("ㄱ").DoInsert("ㅏ").DoInsert("ㄱ").DoInsert("ㄱ").ToString());
        Assert.AreEqual("뷀", pHangul.DoInsert("ㅂ").DoInsert("ㅜ").DoInsert("ㅔ").DoInsert("ㄹ").DoInsert("ㄷ").ToString());
        Assert.AreEqual("꺄", pHangul.DoInsert("ㄲ").DoInsert("ㅑ").DoInsert("ㄸ").ToString());
    }

    [Test]
    public void 한글지우기_테스트()
    {
        string strText = "궯";
        SHangulChar pHangul = SHangulChar.ConvertToHangul(strText);

        Assert.AreEqual("궬", pHangul.DoRemove().ToString());
        Assert.AreEqual("궤", pHangul.DoRemove().DoRemove().ToString());
        Assert.AreEqual("구", pHangul.DoRemove().DoRemove().DoRemove().ToString());
        Assert.AreEqual("ㄱ", pHangul.DoRemove().DoRemove().DoRemove().DoRemove().ToString());
        Assert.AreEqual("", pHangul.DoRemove().DoRemove().DoRemove().DoRemove().DoRemove().ToString());


        strText = "뛙";
        pHangul = SHangulChar.ConvertToHangul(strText);

        Assert.AreEqual("뛘", pHangul.DoRemove().ToString());
        Assert.AreEqual("뛔", pHangul.DoRemove().DoRemove().ToString());
        Assert.AreEqual("뚜", pHangul.DoRemove().DoRemove().DoRemove().ToString());
        Assert.AreEqual("ㄸ", pHangul.DoRemove().DoRemove().DoRemove().DoRemove().ToString());
        Assert.AreEqual("", pHangul.DoRemove().DoRemove().DoRemove().DoRemove().DoRemove().ToString());


        strText = "가";
        pHangul = SHangulChar.ConvertToHangul(strText);

        Assert.AreEqual("ㄱ", pHangul.DoRemove().ToString());
        Assert.AreEqual("", pHangul.DoRemove().DoRemove().ToString());


        strText = "위";
        pHangul = SHangulChar.ConvertToHangul(strText);

        Assert.AreEqual("우", pHangul.DoRemove().ToString());
        Assert.AreEqual("ㅇ", pHangul.DoRemove().DoRemove().ToString());
        Assert.AreEqual("", pHangul.DoRemove().DoRemove().DoRemove().ToString());
    }

    void 테스트_자소나누고합치기(string strText)
    {
        SHangulChar pHangul = SHangulChar.ConvertToHangul(strText);
        Assert.AreEqual(strText, pHangul.ToString());
    }


    [Test]
    public void 한글스트링_작성하기테스트_다른글자도됩니다()
    {
        SHangulString pHangul = new SHangulString();
        pHangul.DoInsert('ㅇ').DoInsert('ㅏ').DoInsert('ㄴ').
                DoInsert('ㄴ').DoInsert('ㅕ').DoInsert('ㅇ').
                DoInsert('ㅎ').DoInsert('ㅏ').
                DoInsert('ㅅ').DoInsert('ㅔ').
                DoInsert('ㅇ').DoInsert('ㅛ').
                DoInsert(' ').
                DoInsert('ㅌ').DoInsert('ㅔ').
                DoInsert('ㅅ').DoInsert('ㅡ').
                DoInsert('ㅌ').DoInsert('ㅡ').
                DoInsert('ㅇ').DoInsert('ㅣ').DoInsert('ㅂ').
                DoInsert('ㄴ').DoInsert('ㅣ').
                DoInsert('ㄷ').DoInsert('ㅏ').
                DoInsert(' ').
                DoInsert('ㅃ').DoInsert('ㅜ').DoInsert('ㅔ').DoInsert('ㄹ').DoInsert('ㄱ');

        Assert.AreEqual("안녕하세요 테스트입니다 쀍", pHangul.ToString());

        pHangul.DoInsert(' ').
                DoInsert('H').DoInsert('i').
                DoInsert(' ').
                DoInsert('I').DoInsert('t').DoInsert('\'').DoInsert('s').
                DoInsert(' ').
                DoInsert('a').
                DoInsert(' ').
                DoInsert('T').DoInsert('e').DoInsert('s').DoInsert('t');

        Assert.AreEqual("안녕하세요 테스트입니다 쀍 Hi It's a Test", pHangul.ToString());

        pHangul = new SHangulString();
        Assert.AreEqual("헤헤", pHangul.DoInsert('ㅎ').DoInsert('ㅔ').DoInsert('ㅎ').DoInsert('ㅔ').ToString());
    }

    [Test]
    public void 한글스트링_작성하기테스트_이상한경우()
    {
        SHangulString pHangul = new SHangulString();
        pHangul.DoInsert('ㄱ').DoInsert('ㄴ').DoInsert('ㄷ').
                DoInsert('ㅏ').DoInsert('ㅑ').
                DoInsert('ㅗ').DoInsert('ㅐ');

        Assert.AreEqual("ㄱㄴ다ㅑㅙ", pHangul.ToString());

        pHangul.DoRemove(); Assert.AreEqual("ㄱㄴ다ㅑㅗ", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("ㄱㄴ다ㅑ", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("ㄱㄴ다", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("ㄱㄴㄷ", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("ㄱㄴ", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("ㄱ", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("", pHangul.ToString());
    }

    [Test]
    public void 한글스트링_지우기테스트()
    {
        SHangulString pHangul = new SHangulString("쀍 Hi");
        pHangul.DoRemove(); Assert.AreEqual("쀍 H", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("쀍 ", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("쀍", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("쀌", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("쀄", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("뿌", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("ㅃ", pHangul.ToString());
        pHangul.DoRemove(); Assert.AreEqual("", pHangul.ToString());
    }
}

#endregion