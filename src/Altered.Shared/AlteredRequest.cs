namespace Altered.Shared
{
    // use for 'empty' requests, inherit if you must
    public class AlteredRequest : IAlteredRequest
    {
        public string RequestId { get; set; }
    }
}
