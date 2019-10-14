using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Altered.Shared.Extensions;

namespace Altered.Shared
{
    public struct StatusCode
    {
        private int value;

        public static implicit operator int(StatusCode c) => c.value;
        public static implicit operator StatusCode(int v) => new StatusCode { value = v };
        public static implicit operator HttpStatusCode(StatusCode c) => (HttpStatusCode)c.value;
        public static implicit operator StatusCode(HttpStatusCode v) => new StatusCode { value = (int)v };

        public override string ToString() => value.ToString();
        public override bool Equals(object obj) => value.Equals(obj);
        public override int GetHashCode() => value.GetHashCode();
    }

    public static class StatusCodeExtensions
    {
        public static bool Is2XX(this StatusCode code) => code >= 200 && code < 300;
        public static bool Is3XX(this StatusCode code) => code >= 300 && code < 400;
        public static bool Is4XX(this StatusCode code) => code >= 400 && code < 500;
        public static bool Is5XX(this StatusCode code) => code >= 500 && code < 600;

        public static bool ShouldRetry(this StatusCode code) => code.Is5XX();
    }
}
