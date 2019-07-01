using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
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
}
