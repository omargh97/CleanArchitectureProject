namespace Demo.WebUi.Static
{
    public static class EndPoints
    {
        public static string BaseUrl { get; set; } = "http://localhost:5000/";
        public static string LoginEndPoints { get; set; } = $"{BaseUrl}api/Authentication/Login";
        public static string SigninEndPoints { get; set; } = $"{BaseUrl}api/Authentication/Signin";
    }
}
