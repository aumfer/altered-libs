namespace Altered.Mvc
{
    public static class AlteredHeaderNames
    {
        //public const string CorrelationId = "X-Correlation-Id";

        // sharing this for now, should probably just use the one above, its pretty common
        public const string CorrelationId = "X-CoxAuto-Correlation-Id";

        public const string Duration = "X-Duration";
    }
}
