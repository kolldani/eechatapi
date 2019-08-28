using System.Web.Http;
using System.Configuration;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;
using Unity.Injection;
using EEChatService.Business.Service;
using NLog;

namespace EEChatService
{
    public static class UnityConfig
    {
        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IChatService, ChatService>(new TransientLifetimeManager(), new InjectionConstructor(GetConnectionString()));

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }

        private static string GetConnectionString()
        {
            string connectionString = string.Empty;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["EEChatDbConnection"].ConnectionString;
            }
            catch (System.Exception e)
            {
                Logger.Error($"{nameof(GetConnectionString)} threw an exception: {e.Message}");
                throw e;
            }

            return connectionString;
        }
    }
}