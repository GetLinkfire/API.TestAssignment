using System.Data.Entity;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json.Converters;
using Repository;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces.Commands;
using Service.Interfaces.Storage;
using Service.Link;
using Service.Models.Link;
using Service.Storage;
using WebApp.Mappings;

namespace WebApp
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			AutoMapperConfiguration.Configure();
			// Web API configuration and services
			var builder = new ContainerBuilder();

			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

			builder.RegisterType<Context>().InstancePerLifetimeScope();

			builder.RegisterType<LinkRepository>().As<ILinkRepository>().InstancePerLifetimeScope();
			builder.RegisterType<DomainRepository>().As<IDomainRepository>().InstancePerLifetimeScope();
			builder.RegisterType<MediaServiceRepository>().As<IMediaServiceRepository>().InstancePerLifetimeScope();

			builder.RegisterType<StorageService>().As<IStorage>().InstancePerLifetimeScope();

			builder.RegisterType<CreateLinkCommand>().As<ICommand<LinkModel, CreateLinkArgument>>().SingleInstance();
			builder.RegisterType<UpdateLinkCommand>().As<ICommand<ExtendedLinkModel, UpdateLinkArgument>>().SingleInstance();
			builder.RegisterType<GetLinkCommand>().As<ICommand<ExtendedLinkModel, GetLinkArgument>>().SingleInstance();
			builder.RegisterType<DeleteLinkCommand>().As<ICommand<DeleteLinkArgument>>().SingleInstance();
			
			var container = builder.Build();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
			// Web API routes
			config.MapHttpAttributeRoutes();

			Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, Repository.Entities.Migrations.Configuration>());

			config.Formatters.Clear();
			config.Formatters.Add(new JsonMediaTypeFormatter());
			config.Formatters.JsonFormatter.SupportedMediaTypes.Clear();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
			config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
			config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
			config.Formatters.JsonFormatter.Indent = true;

			config.EnsureInitialized();
		}
	}
}
