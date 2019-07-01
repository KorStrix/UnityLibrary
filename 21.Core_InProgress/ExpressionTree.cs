#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-15 오후 6:55:39
 *	개요 : 
 *	출처 :http://codinghelmet.com/exercises/expression-evaluator
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 후위연산을 스택으로 구현한 식트리
/// </summary>
public abstract class ExpressionTreeBase<EXPRESSION_ELEMENT, VALUE, OPERATOR>
{
    Stack<VALUE> stack_Value = new Stack<VALUE>();
    Stack<OPERATOR> stack_Operator = new Stack<OPERATOR>();

    public VALUE EvaluateExpression(IEnumerable<EXPRESSION_ELEMENT> arrExpression)
    {
        stack_Value.Clear();
        stack_Operator.Clear();
        stack_Operator.Push(IExpressionElement_GetParenthesis_Open()); // Implicit opening parenthesis

        foreach(var pCurrentCommand in arrExpression)
        {
            if (Check_IsOperator(pCurrentCommand))
            {
                OPERATOR pOperator = Convert_Operator(pCurrentCommand);
                if (pOperator.Equals(IExpressionElement_GetParenthesis_Close()))
                    ProcessClosingParenthesis(stack_Value, stack_Operator);
                else
                    ProcessInputOperator(Convert_Operator(pCurrentCommand), stack_Value, stack_Operator);
            }
            else if (Check_IsValue(pCurrentCommand))
            {
                stack_Value.Push(Convert_Value(pCurrentCommand));
            }
        }
        ProcessClosingParenthesis(stack_Value, stack_Operator);

        return stack_Value.Pop(); // Result remains on values stacks
    }

    // ========================================================================================

    abstract protected OPERATOR IExpressionElement_GetParenthesis_Open();
    abstract protected OPERATOR IExpressionElement_GetParenthesis_Close();
    abstract protected VALUE OnCalculateOperation(VALUE pValue_Left, VALUE pValue_Right, OPERATOR pOperator);

    abstract protected bool Check_IsValue(EXPRESSION_ELEMENT pElement);
    abstract protected bool Check_IsOperator(EXPRESSION_ELEMENT pElement);
    abstract protected bool Check_IsPriority_Operator(OPERATOR pOperator);

    abstract protected VALUE Convert_Value(EXPRESSION_ELEMENT pElement);
    abstract protected OPERATOR Convert_Operator(EXPRESSION_ELEMENT pElement);

    // ========================================================================================

    void ProcessClosingParenthesis(Stack<VALUE> stack_Value, Stack<OPERATOR> stack_Operator)
    {
        while (stack_Operator.Peek().Equals(IExpressionElement_GetParenthesis_Open()) == false)
            ExecuteOperation(stack_Value, stack_Operator);

        stack_Operator.Pop(); // Remove the opening parenthesis
    }

    void ProcessInputOperator(OPERATOR pOperator, Stack<VALUE> stack_Value, Stack<OPERATOR> stack_Operator)
    {
        while (stack_Operator.Count > 0 &&
              (pOperator.Equals(IExpressionElement_GetParenthesis_Close()) ||
              (pOperator.Equals(IExpressionElement_GetParenthesis_Open()) == false && Check_IsPriority_Operator(stack_Operator.Peek()))))
            ExecuteOperation(stack_Value, stack_Operator);

        stack_Operator.Push(pOperator);
    }

    void ExecuteOperation(Stack<VALUE> stack_Value, Stack<OPERATOR> stack_Operator)
    {
        VALUE pValue_Right = stack_Value.Pop();
        VALUE pValue_Left = stack_Value.Pop();
        OPERATOR pOperator = stack_Operator.Pop();

        stack_Value.Push(OnCalculateOperation(pValue_Left, pValue_Right, pOperator));
    }
}