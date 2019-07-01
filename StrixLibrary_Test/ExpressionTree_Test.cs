#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-15 오후 6:56:26
 *	개요 : 
 *	출처 : http://codinghelmet.com/exercises/expression-evaluator
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

/// <summary>
/// 
/// </summary>
public class ExpressionTree_Test
{
    [Test]
    public void Test_Calculator()
    {
        ExpressionTreeCalculator pTree = new ExpressionTreeCalculator(); // 1자리 숫자만, 소수점 처리가 안되는 계산기입니다.
        CalculatorTest(pTree,
            "1",
             1);

        CalculatorTest(pTree,
            "(((1 * (2 + 3)) + (1 * 3)) + 5 * 5) + (5)",
            ((((1 * (2 + 3)) + (1 * 3)) + 5 * 5) + (5)));

        CalculatorTest(pTree,
            "1 * 2 * 3 * 4 * 5 + 5",
             1 * 2 * 3 * 4 * 5 + 5);

        CalculatorTest(pTree,
            "((1 * 2) * (3 * 4)) * (5 + 5)",
             ((1 * 2) * (3 * 4)) * (5 + 5));

        CalculatorTest(pTree,
            "((2 * 3) / 2 + 2) * 8 + (2 * (1 + 1 / 2 * 1 - 1))",
             ((2 * 3) / 2 + 2) * 8 + (2 * (1 + 1 / 2 * 1 - 1)));
    }

    private void CalculatorTest(ExpressionTreeCalculator pTree, string strLine, int iValue)
    {
        Assert.AreEqual(iValue, pTree.EvaluateExpression(strLine.Replace(" ", "").ToCharArray()));
    }


    [Test]
    public void Test_Calculator_Bool()
    {
        ExpressionTreeCalculator_bool pTree = new ExpressionTreeCalculator_bool(); // 0 == false, 1 == true인 bool 계산기입니다.
        Calculator_Test_Bool(pTree,
            "1",
             true);

        Calculator_Test_Bool(pTree,
            "1 & 0",
             true & false);

        Calculator_Test_Bool(pTree,
            "(0 | 1) & (1)",
             (false | true) & (true));

        Calculator_Test_Bool(pTree,
            "((0 | 1) & (1)) & 1 ^ (1 | 0)",
             ((false | true) & (true)) &  true ^ (true | false));
    }

    private void Calculator_Test_Bool(ExpressionTreeCalculator_bool pTree, string strLine, bool bValue)
    {
        Assert.AreEqual(bValue, pTree.EvaluateExpression(strLine.Replace(" ", "").ToCharArray()));
    }

    // ========================================================================================


    public class ExpressionTreeCalculator : ExpressionTreeBase<char, int, char>
    {
        protected override bool Check_IsOperator(char pElement)
        {
            return Convert_Operator(pElement) == pElement;
        }

        protected override bool Check_IsValue(char pElement)
        {
            return pElement >= '0' && pElement <= '9';
        }

        protected override char Convert_Operator(char pElement)
        {
            switch (pElement)
            {
                case '+': return pElement;
                case '-': return pElement;
                case '*': return pElement;
                case '/': return pElement;
                case '(': return pElement;
                case ')': return pElement;
            }

            return ' ';
        }

        protected override int Convert_Value(char pElement)
        {
            return int.Parse(pElement.ToString());
        }

        protected override char IExpressionElement_GetParenthesis_Close()
        {
            return ')';
        }

        protected override char IExpressionElement_GetParenthesis_Open()
        {
            return '(';
        }

        protected override bool Check_IsPriority_Operator(char pOperator)
        {
            return pOperator == '*' || pOperator == '/';
        }

        protected override int OnCalculateOperation(int leftOperand, int rightOperand, char op)
        {
            switch (op)
            {
                case '+': return leftOperand + rightOperand;
                case '-': return leftOperand - rightOperand;
                case '*': return leftOperand * rightOperand;
                case '/': return  leftOperand / rightOperand;
            }

            return 0;
        }
    }

    // ========================================================================================

    public class ExpressionTreeCalculator_bool : ExpressionTreeBase<char, bool, char>
    {
        protected override bool Check_IsOperator(char pElement)
        {
            return Convert_Operator(pElement) == pElement;
        }

        protected override bool Check_IsValue(char pElement)
        {
            return pElement == '0' || pElement == '1';
        }

        protected override char Convert_Operator(char pElement)
        {
            switch (pElement)
            {
                case '|': return pElement;
                case '&': return pElement;
                case '^': return pElement;
            }

            return ' ';
        }

        protected override bool Convert_Value(char pElement)
        {
            switch (pElement)
            {
                case '0': return false;
                case '1': return true;
            }

            return false;
        }

        protected override char IExpressionElement_GetParenthesis_Close()
        {
            return ')';
        }

        protected override char IExpressionElement_GetParenthesis_Open()
        {
            return '(';
        }

        protected override bool Check_IsPriority_Operator(char pOperator)
        {
            return false;
        }

        protected override bool OnCalculateOperation(bool pValue_Left, bool pValue_Right, char pOperator)
        {
            switch (pOperator)
            {
                case '|': return pValue_Left | pValue_Right;
                case '&': return pValue_Left & pValue_Right;
                case '^': return pValue_Left ^ pValue_Right;
            }

            return false;
        }
    }

}