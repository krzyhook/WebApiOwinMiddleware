﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApiOwinMiddleware.Startup))]

namespace WebApiOwinMiddleware
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web.Http;

    using WebApiOwinMiddleware.App_Data;
    using WebApiOwinMiddleware.Configuration;
    using WebApiOwinMiddleware.Extensions;
    using WebApiOwinMiddleware.OwinMiddlewares;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpConfiguration = new HttpConfiguration();

            //app.UseIpFiltering(remoteAddress =>
            //{
            //    var bytes = remoteAddress.GetAddressBytes();
            //    bool addressToReject = bytes[0] != 192 && bytes[0] != 172 && bytes[0] != 10 && bytes[0] != 127 && bytes[0] != 0;
            //    return addressToReject;
            //});

            app.UseHeaderFiltering(headers =>
            {
                if (ConfigurationProvider.TokenHeaderFilteringEnabled)
                {
                    return this.ValidateTokenHeader(headers);
                }

                return true;
            });

            app.UseBasicAuthentication(this.LogOn);

            // Configure Web API Routes:
            // - Enable Attribute Mapping
            // - Enable Default routes at /api.
            httpConfiguration.MapHttpAttributeRoutes();
            httpConfiguration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(httpConfiguration);
            this.SetupDatabase();
        }

        private Task<IIdentity> LogOn(string userName, string password)
        {
            if (userName == ConfigurationProvider.ApiUserName && password == ConfigurationProvider.ApiPassword)
            {
                var claims = new[]
                {
                        new Claim(ClaimTypes.Name, userName),
                        new Claim(ClaimTypes.Email, "some-username@some-email.com")
                    };

                var identity = new ClaimsIdentity(claims, BasicAuthenticationMiddleware.AuthMode);
                return Task.FromResult<IIdentity>(identity);
            }

            return Task.FromResult<IIdentity>(null);
        }

        private bool ValidateTokenHeader(Microsoft.Owin.IHeaderDictionary headers)
        {
            string[] headerValues;
            bool hasHeader = headers.TryGetValue(ConfigurationProvider.TokenHeaderName, out headerValues);
            if (hasHeader && headerValues != null && headerValues.Any())
            {
                return string.Equals(headerValues.First(), ConfigurationProvider.TokenHeaderValue, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private void SetupDatabase()
        {
            DatabaseSetup.Initialize();
        }
    }
}
