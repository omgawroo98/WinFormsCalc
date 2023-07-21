using System.Linq.Expressions;
using System.Text;

namespace Calculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            initializeEventHandlers_Buttons();
        }

        public void initializeEventHandlers_Buttons()
        {
            foreach (Control control in this.Controls)
            {
                // pattern matching (C#7.0): allows you to both check if an object is of a certain type and cast it to that type in a single operation.
                if (control is Button button)
                {
                    button.Click += handleButtonClick;
                }
            }
        }

        private void handleButtonClick(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string buttonText = clickedButton.Text;

            #region control flow block for button invocations
            if (buttonText.Equals("←") && !string.IsNullOrWhiteSpace(buttonText))
            {
                if (displayPanel.Text.Length > 0)
                    displayPanel.Text = displayPanel.Text.Remove(displayPanel.Text.Length - 1, 1);
            }
            else if (buttonText.Equals("C"))
            {
                displayPanel.Clear();
            }
            else if (buttonText.Equals("="))
            {
                List<string> tokens = parseExpression(displayPanel.Text);
                double result = EvalutateRPN(convertToRPN(tokens));
                displayPanel.Clear();
                displayPanel.Text = result.ToString();
            }
            else
            {
                addItem(displayPanel, buttonText);
            }
            #endregion
        }

        private double EvalutateRPN(Queue<string> rpnQueue)
        {
            Stack<double> stack = new Stack<double>();

            while (rpnQueue.Count > 0)
            {

                string token = rpnQueue.Dequeue();

                if (double.TryParse(token, out double number))
                {
                    stack.Push(number);
                }

                else if (checkOperator(token[0]))
                {
                    double operand2 = stack.Pop();
                    double operand1 = stack.Pop();
                    double result = performOperation(operand1, operand2, token);
                    stack.Push(result);
                }
            }
            return stack.Pop();
        }

        private static Queue<string> convertToRPN(List<string> infixTokens)
        {
            Queue<string> outputQueue = new Queue<string>();
            Stack<string> operatorStack = new Stack<string>();

            foreach (string token in infixTokens)
            {
                if (int.TryParse(token, out _))
                {
                    outputQueue.Enqueue(token);
                }
                else if (checkOperator(token[0]))
                {
                    while (operatorStack.Count > 0 && getPrecedence(operatorStack.Peek()) >= getPrecedence(token))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
                else if (token.Equals("("))
                {
                    operatorStack.Push(token);
                }
                else if (token.Equals(")"))
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Pop();
                }
            }

            while (operatorStack.Count > 0)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }
            return outputQueue;
        }

        private static int getPrecedence(string v)
        {
            switch (v)
            {
                case "+":
                    return 1;
                case "-":
                    return 1;
                case "*":
                    return 3;
                case "/":
                    return 3;
                case "^":
                    return 4;
                case "%":
                    return 2;
                default:
                    return 1;
            }
        }

        private static double performOperation(double operand1, double operand2, string operatorSymbol)
        {
            switch (operatorSymbol)
            {
                case "+":
                    return operand1 + operand2;
                case "-":
                    return operand1 - operand2;
                case "*":
                    return operand1 * operand2;
                case "/":
                    return operand1 / operand2;
                case "^":
                    return Math.Pow(operand1, operand2);
                case "%":
                    return operand1 % operand2;
                default:
                    throw new ArgumentException($"Unknown operator: {operatorSymbol}");
            }
        }

        private List<string> parseExpression(string expression)
        {

            List<string> tokens = new List<string>();
            StringBuilder currentNumber = new StringBuilder();

            foreach (char c in expression)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    currentNumber.Append(c);
                }
                else if (checkOperator(c))
                {
                    if (currentNumber.Length > 0)
                    {
                        tokens.Add(currentNumber.ToString());
                        currentNumber.Clear();
                    }
                    tokens.Add(c.ToString());
                }
            }
            if (currentNumber.Length > 0)
            {
                tokens.Add(currentNumber.ToString());
            }
            return tokens;
        }

        private void addItem(TextBox displayPanel, string itemToAdd)
        {

            if ((displayPanel.Text.Length == 0 && !checkOperator(itemToAdd[0])) || checkValidity(displayPanel.Text, itemToAdd))
            {
                if (itemToAdd == ")")
                {
                    if (displayPanel.Text.Contains("("))
                    {
                        displayPanel.Text += itemToAdd;
                    } else
                    {
                        System.Diagnostics.Debug.WriteLine("Can't add that there!");
                    }
                } else
                {
                    displayPanel.Text += itemToAdd;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Can't add that there!");
            }
        }

        private bool checkValidity(string current, string itemToAdd)
        {
            if (current.Length > 0 && checkOperator(current[current.Length - 1]))
            {
                return !checkOperator(itemToAdd[0]);
            }
            else
            {
                return true;
            }

        }

        private static bool checkOperator(char theChar)
        {
            return theChar.Equals('-') || theChar.Equals('+') || theChar.Equals('*') || theChar.Equals('/') || theChar.Equals('%') || theChar.Equals('^');
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}