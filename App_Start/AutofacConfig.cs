using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using Invitee.Repository;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;
using Serilog.Formatting.Json;
using Serilog;
using Invitee.Firebase;
using Invitee.Infrastructure.PaymentInfra;

namespace Invitee.App_Start
{
    public class AutofacConfig
    {
        public static Autofac.IContainer RegisterComponents()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(typeof(UserRepository).Assembly)
              .Where(x => x.Namespace.Contains("Repository"))
              .AsImplementedInterfaces();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();
            builder.RegisterControllers(typeof(Controllers.DefaultController).Assembly);
            builder.RegisterType<RepositoryContext>().AsSelf().InstancePerRequest();
            //builder.RegisterType<RepositoryWrapper>().As<IRepositoryWrapper>().InstancePerRequest().PreserveExistingDefaults();
            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerRequest();
            builder.Register<ILogger>((c, p) =>
            {
                return new LoggerConfiguration()
                  .WriteTo.File(new JsonFormatter(),
                    AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "/SeriLogs.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 5242880, rollOnFileSizeLimit: true, shared: true)
                  .CreateLogger();
            }).SingleInstance();
            builder.RegisterType<FireBaseAdmin>().As<IFireBaseAdmin>().InstancePerRequest();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            return container;
        }
    }
}
