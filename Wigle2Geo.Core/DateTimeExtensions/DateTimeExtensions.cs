namespace Wigle2Geo.Core.DateTimeExtensions
{
    public static class DateTimeExtensions
    {
        // Convert datetime to UNIX time
        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();
        }
    }
}
