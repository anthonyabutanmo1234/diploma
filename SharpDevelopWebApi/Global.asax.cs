﻿/*
 * Created by SharpDevelop.
 * User: Gabs
 * Date: 19/07/2019
 * Time: 2:03 am
 */
 
using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.SessionState;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;
using Hangfire.MemoryStorage;

namespace SharpDevelopWebApi
{
	public class MvcApplication : HttpApplication
	{
        const string API_Route_Prefix = "api";
        Hangfire.BackgroundJobServer _backgroundJobServer;

        protected void Application_Start()
		{
        	AutoMapperConfig.Initialize();
        	
			var config = System.Web.Http.GlobalConfiguration.Configuration;
			
			config.EnableCors(new EnableCorsAttribute("*","*","*"));	

			config.MessageHandlers.Add(new JWTAuth.TokenValidationHandler());			
			
			config.MapHttpAttributeRoutes();
			
            // Redirect root to Swagger UI
            config.Routes.MapHttpRoute(
                name: "Swagger UI",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler(SwaggerDocsConfig.DefaultRootUrlResolver, "swagger"));
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: API_Route_Prefix + "/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }); 
                
            config.Formatters.Remove(config.Formatters.XmlFormatter);
		    config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		    config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;            
            config.EnsureInitialized();
            
			// Configure Hangfire www.hangfire.io            
			Hangfire.GlobalConfiguration.Configuration.UseMemoryStorage();
			_backgroundJobServer = new Hangfire.BackgroundJobServer();         

			SimpleLogger.Init();
        }
        
        protected void Application_End(object sender, EventArgs e)
        {
            _backgroundJobServer.Dispose();
        }        

    }
}
