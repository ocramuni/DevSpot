using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DevSpot.AcceptanceTests.Support.Forms
{
    public class FormSession
    {
        public string Action { get; set; } = string.Empty;
        public string Method { get; set; } = "POST";
        public Dictionary<string, string> Fields { get; } = new(StringComparer.OrdinalIgnoreCase);

        public void SetField(string name, string value)
        {
            Fields[name] = value;
        }

        public FormUrlEncodedContent ToFormUrlEncodedContent()
        {
            return new FormUrlEncodedContent(Fields);
        }
    }
}
