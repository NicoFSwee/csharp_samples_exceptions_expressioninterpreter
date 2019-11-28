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
                    throw new InvalidOperationException();
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
                _operandRight = ScanNumber(ref i);
                SkipBlanks(ref i);
                _operandLeft = ScanNumber(ref i);
                SkipBlanks(ref i);

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

            if(ExpressionText[pos] == '-')
            {
                isNegative = true;
                pos++;
                SkipBlanks(ref pos);
            }

            //Now the pos is at the first digit that occurs.
            int posTmp = pos;
            while(ExpressionText[pos] != '+' && ExpressionText[pos] != '-' 
                && ExpressionText[pos] != '*' && ExpressionText[pos] != '/')
            {
                if(ExpressionText[pos] == ',')
                {
                    containsComma = true;
                }
                pos++;
            }
            _op = ExpressionText[pos];

            //Now we got the Operator and reset the pos to the position of the first digit.
            pos = posTmp;

            int faktor = 1;
            //Do we have a comma?
            if (!containsComma)
            {
                while(ExpressionText[pos] != _op || !char.IsWhiteSpace(ExpressionText[pos]))
                {
                    pos++;  //Count the positions until the digit ends.
                }

                //Now we are at the next pos after the last digit and we look at the digits beginning from the end
                for (int i = pos - 1; i >= posTmp; i++)
                {
                    int number = ScanInteger(ref i);
                    result += number * faktor;
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

                for (int i = pos - 1; i > posTmp; i++)
                {
                    int number = ScanInteger(ref i);
                    result += number * faktor;
                    faktor *= 10;
                }


                //now we should have the number on the left side of the comma;
                pos++; //Move the pos on the next digit, away from the comma.
                posTmp = pos;
                while (char.IsDigit(ExpressionText[pos]))
                {
                    pos++;
                }

                //Now we are behind the last digit of the left side.

                for (int i = posTmp; i < pos; i++)
                {
                    int number = ScanInteger(ref i);
                    result += number / faktor;
                    faktor *= 10;
                }

            }

            return result;
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
            while(char.IsWhiteSpace(ExpressionText[pos]))
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
            throw new NotImplementedException();
        }
    }
}
