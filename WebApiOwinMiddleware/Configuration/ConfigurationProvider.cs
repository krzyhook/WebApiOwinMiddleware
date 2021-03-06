﻿namespace WebApiOwinMiddleware.Configuration
{
    public static class ConfigurationProvider
    {
        public static bool TokenHeaderFilteringEnabled => bool.Parse(System.Configuration.ConfigurationManager.AppSettings["TokenHeaderFilteringEnabled"]);

        public static string TokenHeaderName => System.Configuration.ConfigurationManager.AppSettings["TokenHeaderName"];

        public static string TokenHeaderValue => System.Configuration.ConfigurationManager.AppSettings["TokenHeaderValue"];

        public static string ApiUserName => System.Configuration.ConfigurationManager.AppSettings["ApiUserName"];

        public static string ApiPassword => System.Configuration.ConfigurationManager.AppSettings["ApiPassword"];
        public static string DBFileName => System.Configuration.ConfigurationManager.AppSettings["DBFileName"];
    }
}
