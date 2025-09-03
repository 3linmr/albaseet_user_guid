using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Reflection;

namespace Shared.Helper.Startup
{
	public static class StartupExtensions
	{
		//public static void ConfigureIdentityServer(this IServiceCollection services, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
		//{
		//	var authority = configuration.GetSection("IdentityServerAuthority")?.Value;
		//	var authorityLocal = configuration.GetSection("IdentityServerAuthorityLocal")?.Value;
		//	var clientId = configuration.GetSection("IdentityServerClientId")?.Value;
		//	var clientIdLocal = configuration.GetSection("IdentityServerClientIdLocal")?.Value;
		//	if (webHostEnvironment.IsDevelopment())
		//	{
		//		services.AddAuthentication(options =>
		//		{
		//			options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		//			options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
		//		}).AddCookie().AddOpenIdConnect(configureOptions: options =>
		//		{
		//			options.Authority = authorityLocal;
		//			options.ClientId = clientIdLocal;
		//			options.RequireHttpsMetadata = false;
		//			options.Scope.Add("profile");
		//			options.Scope.Add("openid");
		//			options.Scope.Add("email");
		//			options.Scope.Add("roles");
		//			//options.Scope.Add("HubStoreApilocal");
		//			options.ResponseType = "code id_token";
		//			options.SaveTokens = true;
		//			options.ClientSecret = "0fceb88d-1d94-4f3c-9588-c976da2edd0a";
		//			options.GetClaimsFromUserInfoEndpoint = true;
		//			options.SaveTokens = true;
		//			options.MaxAge = TimeSpan.FromMinutes(600); // change SSoLifeTime in Database
		//			options.UseTokenLifetime = true;
		//		});
		//		//IdentityModelEventSource.ShowPII = true; //UncommentToShowWhatBehindPPI
		//	}
		//	else
		//	{
		//		services.AddAuthentication(options =>
		//		{
		//			options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		//			options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
		//		}).AddCookie().AddOpenIdConnect(configureOptions: options =>
		//		{
		//			options.Authority = authority;
		//			options.ClientId = clientId;
		//			options.RequireHttpsMetadata = false;

		//			options.Scope.Add("profile");
		//			options.Scope.Add("openid");
		//			options.Scope.Add("email");
		//			options.Scope.Add("roles");

		//			//options.Scope.Add("HubStoreApi");
		//			options.ResponseType = "code id_token";
		//			options.SaveTokens = true;
		//			options.ClientSecret = "0fceb88d-1d94-4f3c-9588-c976da2edd0a";
		//			options.GetClaimsFromUserInfoEndpoint = true;
		//			options.SaveTokens = true;
		//			options.MaxAge = TimeSpan.FromMinutes(600); // change SSoLifeTime in Database
		//			options.UseTokenLifetime = true;
		//		});
		//	}
		//}
		public static void CheckSameSite(HttpContext httpContext, CookieOptions options)
		{
			if (options.SameSite == SameSiteMode.None)
			{
				if (!httpContext.Request.IsHttps)
				{
					options.SameSite = SameSiteMode.Lax;
				}
			}
		}

		public static int GetIdleTimeout()
		{
			return 600;
		}
		public static void AddScopedFromAssembly(this IServiceCollection services, Assembly assembly)
		{
			var allServices = assembly.GetTypes().Where(p =>
				p.GetTypeInfo().IsClass &&
				!p.GetTypeInfo().IsAbstract);
			foreach (var type in allServices)
			{
				var allInterfaces = type.GetInterfaces();
				var mainInterfaces = allInterfaces.Except
					(allInterfaces.SelectMany(t => t.GetInterfaces()));
				foreach (var itype in mainInterfaces)
				{
					services.AddScoped(itype, type);
					// if you want you can pass lifetime as a parameter
				}
			}
		}
	}
}
