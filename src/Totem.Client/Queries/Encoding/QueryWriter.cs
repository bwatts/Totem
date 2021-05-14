using System;
using System.Text;

namespace Totem.Queries.Encoding
{
    internal class QueryWriter
    {
        readonly StringBuilder _builder = new();

        internal void Write(string key, object value)
        {
            if(_builder.Length > 0)
            {
                _builder.Append('&');
            }

            _builder.Append(Encode(key)).Append('=').Append(Encode(value.ToString()));
        }

        public override string ToString() =>
            _builder.ToString();

        static string Encode(string? value) =>
            string.IsNullOrWhiteSpace(value) ? "" : Uri.EscapeDataString(value).Replace("%20", "+");
    }
}