using BookManagementAPI.ErrorHelper;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace BookManagementAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Bootstrapper.Initialise();

            HttpConfiguration config = GlobalConfiguration.Configuration;

            config.Formatters.JsonFormatter
                        .SerializerSettings
                        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            log4net.Config.XmlConfigurator.Configure();

            //ConfigurationLogging();
        }
        //public void ConfigurationLogging()
        //{
        //    string sLogFile = HttpRuntime.AppDomainAppPath + "log4net.config";
        //    if ((System.IO.File.Exists(sLogFile)))
        //    {
        //        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(sLogFile));
        //    }
        //}
    }
}
    