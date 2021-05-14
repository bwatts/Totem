using System;
using System.Text;

namespace Totem.Http
{
    public class BasicCredentials
    {
        public BasicCredentials(string name, string secret)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException(nameof(name));

            if(string.IsNullOrWhiteSpace(secret))
                throw new ArgumentOutOfRangeException(nameof(secret));

            Base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{name}:{secret}"));
        }

        public string Base64Data { get; }
    }
}