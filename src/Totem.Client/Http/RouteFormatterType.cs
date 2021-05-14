using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Totem.Http
{
    internal class RouteFormatterType
    {
        delegate object? Token(object message);

        readonly Dictionary<string, Token> _tokensByName = new();
        readonly string[] _parts;

        internal RouteFormatterType(string template, Type messageType)
        {
            _parts = template.Split('{', '}');

            for(var i = 1; i < _parts.Length; i += 2)
            {
                var token = _parts[i];

                _tokensByName[token] = CompileToken(messageType, token);
            }
        }

        internal IEnumerable<string> Tokens => _tokensByName.Keys;

        internal string Format(object message)
        {
            var formattedParts = _parts.ToArray();

            for(var i = 1; i < formattedParts.Length; i += 2)
            {
                var token = _tokensByName[formattedParts[i]];

                formattedParts[i] = token(message)?.ToString() ?? "";
            }

            return string.Join("", formattedParts);
        }

        static Token CompileToken(Type messageType, string token)
        {
            var property = messageType.GetProperty(token);

            if(property == null)
            {
                return _ => '{' + token + '}';
            }

            // message => ((TMessage) message).property

            var messageParameter = Expression.Parameter(typeof(object), "message");

            var lambda = Expression.Lambda<Token>(
                Expression.Property(Expression.Convert(messageParameter, messageType), property),
                messageParameter);

            return lambda.Compile();
        }
    }
}