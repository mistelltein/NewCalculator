using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;


namespace NewCalculator
{
    public class Stack<T>
    {
        private List<T> items = new List<T>();

        public void Push(T item)
        {
            items.Add(item);
        }

        public T Pop()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }

            T item = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return item;
        }

        public T Peek()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }
            return items[items.Count - 1];
        }

        public int Count
        {
            get { return items.Count; }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to the Calculator App");

            while (true)
            {
                Console.WriteLine("Enter an arithmetic expression (or 'exit' to quit): ");
                string input = Console.ReadLine();

                if (input == "exit")
                {
                    break;
                }
                try
                {
                    if (IsCorrectString(input))
                    {
                        double result = CalculateExpression(input);
                        Console.WriteLine($"Result: {result}");
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid expression.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            Console.WriteLine("Goodbye!");
        }

        static bool IsCorrectString(string str)
        {
            var stack = new Stack<char>();

            foreach (char ch in str)
            {
                if (ch == '(' || ch == '[')
                {
                    stack.Push(ch);
                }
                else if (ch == ')' || ch == ']')
                {
                    if (stack.Count == 0)
                    {
                        return false;
                    }

                    char openBracket = stack.Pop();
                    if ((ch == ')' && openBracket != '(') ||
                        (ch == ']' && openBracket != '['))
                    {
                        return false;
                    }
                }
            }
            return stack.Count == 0;
        }

        static double CalculateExpression(string expression)
        {
            var operandStack = new Stack<double>();
            var operatorStack = new Stack<char>();

            var operatorPriority = new Dictionary<char, int>
            {
                {'-', 1 },
                {'+', 1 },
                {'*', 2 },
                {'/', 2 }
            };

            int i = 0;
            while (i < expression.Length)
            {
                char ch = expression[i];

                if (char.IsDigit(ch))
                {
                    int j = i;
                    while (j < expression.Length && (char.IsDigit(expression[j]) || expression[j] == '.'))
                    {
                        j++;
                    }
                    operandStack.Push(double.Parse(expression.Substring(i, j - i)));
                    i = j;
                }
                else if (ch == '(')
                {
                    operatorStack.Push(ch);
                    i++;
                }
                else if (ch == ')')
                {
                    while (operatorStack.Peek() != '(')
                    {
                        double operand2 = operandStack.Pop();
                        double operand1 = operandStack.Pop();
                        char op = operatorStack.Pop();

                        double result = PerformOperation(operand1, operand2, op);
                        operandStack.Push(result);
                    }
                    operatorStack.Pop(); // Remove the '(' from the operator stack
                    i++;
                }
                else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                {
                    while (operatorStack.Count > 0 && operatorPriority.ContainsKey(operatorStack.Peek()) && operatorPriority[ch] <= operatorPriority[operatorStack.Peek()])
                    {
                        double operand2 = operandStack.Pop();
                        double operand1 = operandStack.Pop();
                        char op = operatorStack.Pop();

                        double result = PerformOperation(operand1, operand2, op);
                        operandStack.Push(result);
                    }
                    operatorStack.Push(ch);
                    i++;
                }
                else if (!char.IsWhiteSpace(ch))
                {
                    throw new ArgumentException($"Invalid character: {ch}");
                }
                else
                {
                    i++;
                }
            }

            while (operatorStack.Count > 0)
            {
                double operand2 = operandStack.Pop();
                double operand1 = operandStack.Pop();
                char op = operatorStack.Pop();

                double result = PerformOperation(operand1, operand2, op);
                operandStack.Push(result);
            }
            return operandStack.Peek();
        }

        static double PerformOperation(double operand1, double operand2, char op)
        {
            switch (op)
            {
                case '+':
                    return operand1 + operand2;
                case '-':
                    return operand1 - operand2;
                case '*':
                    return operand1 * operand2;
                case '/':
                    return operand1 / operand2;
                default:
                    throw new ArgumentException("Invalid operator");
            }
        }
    }
}