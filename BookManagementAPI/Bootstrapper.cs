using Data;
using Microsoft.Practices.Unity;
using Service;
using System.Web.Http;
using Unity.WebApi;

namespace BookManagementAPI
{
    public class Bootstrapper
    {
        public static void Initialise()
        {
            var container = new UnityContainer();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IPublisherServices, PublisherServices>();
            container.RegisterType<IBookServices, BookServices>();
            container.RegisterType<IAuthorServices, AuthorServices>();
            container.RegisterType<ICategoryServices, CategoryServices>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}