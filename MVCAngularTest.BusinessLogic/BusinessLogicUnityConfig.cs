﻿using MVCAngularTest.BusinessLogic;
using MVCAngularTest.BusinessLogic.Log;
using MVCAngularTest.Core;
using MVCAngularTest.DataAccess;
using ORP.BusinessLogic;
using Serilog;
using System;
using System.Configuration;
using Unity;
using Unity.Injection;

namespace MVCAngularTest.BusinessLogic
{
    public class BusinessLogicUnityConfig
    {
        public static UnityContainer RegisterInContainer(UnityContainer container)
        {
            CoreUnityConfig.RegisterInContainer(container);
            container.RegisterType<ILogger>(new InjectionFactory((ctr, type, name) =>
            {
                ILogger log = new LoggerConfiguration()
                  .MinimumLevel.Debug()
                   .WriteTo.RollingFile(ConfigurationManager.AppSettings["LogsDir"] + @"log.txt", retainedFileCountLimit: 7)                   
                  .CreateLogger();
                return log;
            }));
            string connectionStringName = "name=MVCAngularTestContext";
            container.RegisterType<IUnitOfWork, UnitOfWork>(new InjectionConstructor(connectionStringName));
            container.RegisterType(typeof(IRepository<>), typeof(GenericRepository<>));
            container.RegisterType<ILogManager, LogManager>();
            container.RegisterType<IPalindromeManager, PalindromeManager>();
         
            return container;

        }

    }
}