using System;
using System.Collections.Generic;

namespace PVDevelop.ReminderBot.Microservice.Port.Bus
{
    public class HeadersCollection
    {
        private readonly Dictionary<string, object> _headers = new Dictionary<string, object>();

        public bool TryGetHeader(string key, out object value)
        {
            return _headers.TryGetValue(key, out value);
        }

        public void AddHeader(string key, object value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            _headers.Add(key, value);
        }
    }
}
