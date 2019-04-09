using System;
using System.Configuration;
using System.Web.Mvc;
using Serilog;
using Serilog.Formatting.Json;

namespace DeployableWebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.CustomLabel = ConfigurationManager.AppSettings["CustomLabel"];

            return View();
        }


        public ActionResult CustomSuperAction(string query)
        {
            var logger = CreateLogger();

            logger.Information("The user asked for {query}", query);

            try
            {
                if (query == "break")
                    throw new ArgumentException($"I don't like the query {query}");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Query {query} is not supported.");
            }
            return Content("This is some content");
        }

        private ILogger CreateLogger()
        {
            var environment = ConfigurationManager.AppSettings["Environment"];
            var systemName = "AndrewsSystem";
            var instanceName = "AndrewsSystem.Api";
            var logFilePath = $@"C:\Logs\splunk\{environment}\{systemName}\{instanceName}\log.txt";

            var log = new LoggerConfiguration()
                .Enrich.WithProperty("Environment", environment)
                .Enrich.WithProperty("SystemName", systemName)
                .Enrich.WithProperty("InstanceName", instanceName)
                .Enrich.FromLogContext()
                .WriteTo.File(new JsonFormatter(renderMessage: true), logFilePath)
                .MinimumLevel.Debug()
                .CreateLogger();

            return log;
        }
    }
}
