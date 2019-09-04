﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace VideoFilesLibrary.Helpers {
    public static class SessionExtensions {
        public static void Set<T>(this ISession session, string key, T value) {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key) {
            var value = session.GetString(key);

            return value == null ? default(T) :
                                  JsonConvert.DeserializeObject<T>(value);
        }

        public static void Remove(this ISession session, string key) {
            session.Remove(key);
        }
    }
}
