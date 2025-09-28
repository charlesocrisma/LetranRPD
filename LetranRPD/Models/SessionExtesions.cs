namespace LetranRPD.Models
{
    public static class SessionExtesions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
        }
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }



    }
}
