using Autofac;
using Autofac.Integration.WebApi;
using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Infrastructure.Services;
using Litskevich.Family.WebApi.App_Start;
using Newtonsoft.Json;
using SXCore.Common.Classes;
using SXCore.Common.Contracts;
using SXCore.Common.Interfaces;
using SXCore.Infrastructure.Services;
using SXCore.Infrastructure.Services.Cache;
using SXCore.Infrastructure.Services.Dependency;
using SXCore.WebApi.Services;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Litskevich.Family.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        static public Assembly[] Assemblies
        {
            get
            {
                List<Assembly> result = new List<Assembly>();

                result.Add(Assembly.GetExecutingAssembly());
                //result.Add(typeof(Terminal.Kernel.Api.KernelApiController).Assembly);

                return result.ToArray();
            }
        }

        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;

            // Setup Dependency Injection
            this.ConfigIoc(config);

            // Config Json Formatter
            this.ConfigFormatter(config);

            // Register all Areas
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Ensure that the Api was initialized
            GlobalConfiguration.Configuration.EnsureInitialized();
        }

        protected MediaTypeFormatter ConfigFormatter(HttpConfiguration config)
        {
            var json = config.Formatters.JsonFormatter;

            //json.SerializerSettings.Converters.Add(new JsonDemandDataConverter());
            //json.SerializerSettings.Converters.Add(new JsonOrganizationShareholderConverter());

            json.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            json.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            //json.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";

            //json.UseDataContractJsonSerializer = true;
            //json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
            json.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //json.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;

            return json;
        }

        protected IContainer ConfigIoc(HttpConfiguration config)
        {
            // Get Autofac builder
            var builder = new ContainerBuilder();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assemblies);

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);


            builder.RegisterType<MemoryCacheProvider>().As<ICacheProvider>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            var registrator = new AutofacRegistrator(builder);

            // Set dependencies for Common services
            //registrator.RegisterType<MemoryCacheProvider, ICacheProvider>(DependencyScope.Singletone);
            //registrator.RegisterInstance<ICacheProvider>(new MemoryCacheProvider());
            registrator.RegisterType<AppSettingsProvider, ISettingsProvider>();
            registrator.RegisterType<ApiSubscriberProvider, ISubscriber>();
            registrator.RegisterType<ApiSubscriberProvider, ITokenProvider>();
            registrator.RegisterType<FamilyInfrastructureProvider, IFamilyInfrastructureProvider>();

            // Set dependencies for Extra services
            registrator.RegisterType<AuthenticationService, IAuthenticationProvider>();
            registrator.RegisterType<ThumbnailProvider, IThumbnailProvider>(); 

            // Register Modules
            (new FamilyApiConfiguration()).Config(registrator);

            // build Container
            var container = registrator.Build();

            // resolve dependencies in WebAPI
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // resolve dependencies in Domain Events
            DomainEvents.Container = new AutofacResolver(container);

            return container;
        }
    }
}
