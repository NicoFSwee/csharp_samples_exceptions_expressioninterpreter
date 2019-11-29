using System;
using System.Text;

namespace ExpressionInterpreter.Logic
{
    public class Interpreter
    {
        private double _operandLeft;
        private double _operandRight;
        private char _op;  // Operator   

        /// <summary>
        /// Eingelesener Text
        /// </summary>
        public string ExpressionText { get; private set; }

        public double OperandLeft
        {
            get { return _operandLeft; }
        }

        public double OperandRight
        {
            get { return _operandRight; }
        }

        public char Op
        {
            get { return _op; }
        }


        public void Parse(string expressionText)
        {
            if(!String.IsNullOrEmpty(expressionText))
            {
                ExpressionText = expressionText;
                ParseExpressionStringToFields();
            }
            else
            {
                throw new Exception("Ausdruck ist null oder empty!");
            }
        }

        private void CheckStringForExceptions(int pos)
        {
            if(ExpressionText[pos] == '-')
            {
                pos++;
            }
            while(pos < ExpressionText.Length - 1 && ExpressionText[pos] != '+' && ExpressionText[pos] != '*' && ExpressionText[pos] != '/' && ExpressionText[pos] != '-')
            {
                pos++;
                if(!char.IsDigit(ExpressionText[pos]) && !char.IsWhiteSpace(ExpressionText[pos])  //Checking for symbols that are not allowed.
                    && ExpressionText[pos] != '+' && ExpressionText[pos] != '*' 
                    && ExpressionText[pos] != '/' && ExpressionText[pos] != '-' && ExpressionText[pos] != ',')
                {
                    throw new InvalidOperationException($"Operator {ExpressionText[pos]} ist fehlerhaft!");
                }
            }

            try
            {
                bool leftContainsDigit = false;
                int leftCommaPos = 0;
                bool leftHasComma = false;
                for (int j = 0; j < pos; j++)
                {
                    if (char.IsDigit(ExpressionText[j]))
                    {
                        leftContainsDigit = true;
                    }
                    if (ExpressionText[j] == ',')
                    {
                        leftCommaPos = j;
                        leftHasComma = true;
                    }
                }
                if (!leftContainsDigit)
                {
                    throw new ArgumentException("Zahl fehlt komplett");
                }

                //Now lets check if both sides of the comma have numbers.
                bool lDigitBeforeComma = false;
                bool lDigitAfterComma = false;
                if (leftHasComma)
                {
                    for (int k = 0; k < leftCommaPos; k++)
                    {
                        if (char.IsDigit(ExpressionText[k]))
                        {
                            lDigitBeforeComma = true;
                        }
                    }
                    for (int k = leftCommaPos; k < pos; k++)
                    {
                        if (char.IsDigit(ExpressionText[k]))
                        {
                            lDigitAfterComma = true;
                        }
                    }
                }
                if(!lDigitBeforeComma)
                {
                    throw new ArgumentException("Ganzzahlanteil ist fehlerhaft");
                }
                if (!lDigitAfterComma)
                {
                    throw new ArgumentException("Nachkommaanteil ist fehlerhaft");
                }
            }
            catch(ArgumentException e)
            {
                throw new ArgumentException("Linker Operand ist fehlerhaft", e);
            } 

            //Right side.
            try
            {

                bool rightContainsDigit = false;
                int rightCommaPos = 0;
                bool rightHasComma = false;
                
                for (int j = pos; j < ExpressionText.Length; j++)
                {
                    if (char.IsDigit(ExpressionText[j]))
                    {
                        rightContainsDigit = true;
                    }
                    if (ExpressionText[j] == ',')
                    {
                        rightCommaPos = j;
                        rightHasComma = true;
                    }
                }
                if(rightContainsDigit == false)
                {
                    throw new ArgumentException("Zahl fehlt komplett");
                }

                bool rDigitBeforeComma = false;
                bool rDigitAfterComma = false;

                if (rightHasComma)
                {
                    for (int k = pos; k < rightCommaPos; k++)
                    {
                        if (char.IsDigit(ExpressionText[k]))
                        {
                            rDigitBeforeComma = true;
                        }
                    }
                    for (int k = rightCommaPos; k < ExpressionText.Length; k++)
                    {
                        if (char.IsDigit(ExpressionText[k]))
                        {
                            rDigitAfterComma = true;
                        }
                    }
                }
                
                if(!rDigitBeforeComma)
                {
                    throw new ArgumentException("Ganzzahlanteil ist fehlerhaft");
                }
                if (!rDigitAfterComma)
                {
                    throw new ArgumentException("Nachkommaanteil ist fehlerhaft");
                }
            }
            catch(ArgumentException e)
            {
                throw new ArgumentException("Rechter Operand ist fehlerhaft", e);
            }
        }

        /// <summary>
        /// Wertet den Ausdruck aus und gibt das Ergebnis zurück.
        /// Fehlerhafte Operatoren und Division durch 0 werden über Exceptions zurückgemeldet
        /// </summary>
        public double Calculate()
        {
            double result;

            if (Op == '+')
            {
                result = OperandLeft + OperandRight;
            }
            else if (Op == '-')
            {
                result = OperandLeft - OperandRight;
            }
            else if (Op == '*')
            {
                result = OperandLeft * OperandRight;
            }
            else if (Op == '/')
            {
                if(OperandRight != 0)
                {
                    result = OperandLeft / OperandRight;
                }
                else
                {
                    throw new InvalidOperationException("Division durch 0 ist nicht erlaubt");
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// Expressionstring in seine Bestandteile zerlegen und in die Felder speichern.
        /// 
        ///     { }[-]{ }D{D}[,D{D}]{ }(+|-|*|/){ }[-]{ }D{D}[,D{D}]{ }
        ///     
        /// Syntax  OP = +-*/
        ///         Vorzeichen -
        ///         Zahlen double/int
        ///         Trennzeichen Leerzeichen zwischen OP, Vorzeichen und Zahlen
        /// </summary>
        public void ParseExpressionStringToFields()
        {
            for (int i = 0; i < ExpressionText.Length; i++)
            {
                SkipBlanks(ref i);
                CheckStringForExceptions(i);
                _operandLeft = ScanNumber(ref i);
                SkipBlanks(ref i);
                SkipOperator(ref i);
                SkipBlanks(ref i);
                _operandRight = ScanNumber(ref i);
                SkipBlanks(ref i);
            }
        }

        private void SkipOperator(ref int pos)
        {
            if(ExpressionText[pos] == _op)
            {
                pos++;
            }
        }

        /// <summary>
        /// Ein Double muss mit einer Ziffer beginnen. Gibt es Nachkommastellen,
        /// müssen auch diese mit einer Ziffer beginnen.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private double ScanNumber(ref int pos)
        {
            bool isNegative = false;
            bool containsComma = false;
            double result = 0;

            if(pos < ExpressionText.Length - 1 && ExpressionText[pos] == '-')
            {
                isNegative = true;
                pos++;
                SkipBlanks(ref pos);
            }

            //Now the pos is at the first digit that occurs.
            int posTmp = pos;
            while(pos < ExpressionText.Length - 1 &&  ExpressionText[pos] != '-'      //Loop until we find the operator or reach the end of the string.
                && ExpressionText[pos] != '*' && ExpressionText[pos] != '/' 
                && ExpressionText[pos] != '+' )
            {
                if(ExpressionText[pos] == ',')
                {
                    containsComma = true;
                }
                pos++;
            }

            //Now we got the Operator and reset the pos to the position of the first digit.
            if(_operandLeft == 0)
            {
                _op = ExpressionText[pos];
            }
            
            pos = posTmp;

            double faktor = 1; //Scales with the position of the digit it gets multiplied / divided with.
            
            if (!containsComma) 
            {
                //Do we have a comma?

                while ((pos < ExpressionText.Length - 1 && ExpressionText[pos] != _op) 
                    || (pos < ExpressionText.Length - 1 && !char.IsWhiteSpace(ExpressionText[pos])))    //Count the positions until the digit ends.
                {
                    pos++;  
                }

                //Now we are at the next pos after the last digit and we look at the digits beginning from the end, multiplying each digit with its corresponding faktor (1, 10, 100, ...)
                //and add it to the result
                for (int i = pos - 1; i >= posTmp; i--)
                {
                    int number = ScanInteger(ref i);
                    result += (double)number * faktor;
                    faktor *= 10;
                }
                //Now we got the number on the left side
            }
            else
            {
                //we got a comma, what now?

                while(ExpressionText[pos] != ',')
                {
                    pos++;
                }
                //Move to the pos of the comma.

                for (int i = pos - 1; i >= posTmp; i--) //Same as above for the left side of the comma.
                {
                    int number = ScanInteger(ref i);
                    result += (double)number * faktor;
                    faktor *= 10;
                }
                faktor = 1;

                //now we should have the number on the left side of the comma;
                pos++; //Move the pos on the next digit, away from the comma.
                posTmp = pos;
                while (ExpressionText.Length - 1 != pos && char.IsDigit(ExpressionText[pos]))
                {
                    pos++;
                }

                //Now we are at the last digit of the left side.

                for (int i = posTmp; i < pos; i++)
                {
                    int number = ScanInteger(ref i);
                    faktor *= 10;                           //First digit after comma is a tenth so we increase it before we do stuff.
                    result += (double)number / faktor;
                }

            }

            if(isNegative)
            {
                return result * -1;
            }
            else
            {
                return result;
            }
            
        }

        /// <summary>
        /// Eine Ganzzahl muss mit einer Ziffer beginnen.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int ScanInteger(ref int pos)
        {
            int number;

            number = ExpressionText[pos] - '0';

            return number;
        }

        /// <summary>
        /// Setzt die Position weiter, wenn Leerzeichen vorhanden sind
        /// </summary>
        /// <param name="pos"></param>
        private void SkipBlanks(ref int pos)
        {
            while(pos < ExpressionText.Length - 1 && char.IsWhiteSpace(ExpressionText[pos]))
            {
                pos++;
            }
        }

        /// <summary>
        /// Exceptionmessage samt Innerexception-Texten ausgeben
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetExceptionTextWithInnerExceptions(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Exceptionmessage: {ex.Message}\r\n");
            int i = 1;

            while (ex != null)
            {
                if(ex.InnerException != null)
                {
                    sb.Append($"Inner Exception {i}: {ex.InnerException.Message}\r\n");
                }
                i++;
                ex = ex.InnerException;
            }


            return sb.ToString();
        }
    }
}
