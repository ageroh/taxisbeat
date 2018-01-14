using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace TaxisBeat.Code
{

    //http://haacked.com/archive/2009/01/14/named-formats-redux.aspx/
    public static class HenriFormatter
    {
        private static string OutExpression(string expression, params object[] source)
        {
            if (source != null && source.Length > 0)
            {
                string format = "";

                int colonIndex = expression.IndexOf(':');
                if (colonIndex > 0)
                {
                    format = expression.Substring(colonIndex + 1);
                    expression = expression.Substring(0, colonIndex);
                }

                try
                {
                    int index;
                    bool numeric = false;
                    object indexItem = null;
                    if (int.TryParse(expression, out index))
                    {
                        if (index >= 0 && index < source.Length)
                        {
                            numeric = true;
                            indexItem = source[index];
                        }
                    }
                    var func = source[0] as Func<string, string, string>;
                    if (String.IsNullOrEmpty(format))
                    {
                        if (numeric)
                        {
                            return indexItem == null ? string.Empty : indexItem.ToString();
                        }
                        if (func != null)
                        {
                            return func(expression, null);
                        }
                        return (DataBinder.Eval(source[0], expression) ?? string.Empty).ToString();
                    }
                    if (numeric && func == null)
                    {
                        return string.Format("{0:" + format + "}", indexItem);
                    }
                    if (func != null)
                    {
                        return func(expression, format);
                    }
                    return DataBinder.Eval(source[0], expression, "{0:" + format + "}") ?? "";
                }
                catch (Exception)
                {

                }
            }
            return string.Format("{{{0}}}", expression);
        }

        internal static string HenriFormat(this string format, params object[] source)
        {
            if (format == null)
            {
                return null;
            }

            StringBuilder result = new StringBuilder(format.Length * 2);

            using (var reader = new StringReader(format))
            {
                StringBuilder expression = new StringBuilder();
                int @char = -1;

                State state = State.OutsideExpression;
                do
                {
                    switch (state)
                    {
                        case State.OutsideExpression:
                            @char = reader.Read();
                            switch (@char)
                            {
                                case -1:
                                    state = State.End;
                                    break;
                                case '{':
                                    state = State.OnOpenBracket;
                                    break;
                                case '}':
                                    state = State.OnCloseBracket;
                                    break;
                                default:
                                    result.Append((char)@char);
                                    break;
                            }
                            break;
                        case State.OnOpenBracket:
                            @char = reader.Read();
                            switch (@char)
                            {
                                case -1:
                                    result.Append(expression);
                                    state = State.End;
                                    break;
                                case '{':
                                    result.Append('{');
                                    state = State.OutsideExpression;
                                    break;
                                default:
                                    expression.Append((char)@char);
                                    state = State.InsideExpression;
                                    break;
                            }
                            break;
                        case State.InsideExpression:
                            @char = reader.Read();
                            switch (@char)
                            {
                                case -1:
                                    result.Append('{').Append(expression);
                                    state = State.End;
                                    break;
                                case '}':
                                    result.Append(OutExpression(expression.ToString(), source));
                                    expression.Length = 0;
                                    state = State.OutsideExpression;
                                    break;
                                default:
                                    expression.Append((char)@char);
                                    break;
                            }
                            break;
                        case State.OnCloseBracket:
                            @char = reader.Read();
                            switch (@char)
                            {
                                case '}':
                                    result.Append('}');
                                    state = State.OutsideExpression;
                                    break;
                                case -1:
                                    result.Append(expression).Append('}');
                                    state = State.End;
                                    break;
                                default:
                                    result.Append((char)@char);
                                    break;
                            }
                            break;
                        default:
                            throw new InvalidOperationException("Invalid state.");
                    }
                } while (state != State.End);
            }

            return result.ToString();
        }

        private enum State
        {
            OutsideExpression,
            OnOpenBracket,
            InsideExpression,
            OnCloseBracket,
            End
        }
    }
}
