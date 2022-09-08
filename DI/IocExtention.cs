using Api.Config.DI;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Api.Config
{
    /// <summary>
    /// Ioc扩展
    /// </summary>
    public static class IocExtention
    {        
        /// <summary>
        /// 配置DI
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="name">Json的Key</param>
        /// <param name="jsonFile">配置文件</param>
        public static void SetDI(this IServiceCollection services, string name, string jsonFile)
        {
            if (jsonFile == null)
            {
                return;
            }
            var jsonServices = JObject.Parse(File.ReadAllText(jsonFile))[name];
            SetDI(services, jsonServices);
        }
        /// <summary>
        /// 配置DI
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="directory">文件夹</param>
        public static void SetDI(this IServiceCollection services, string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
                var jsonServices = JObject.Parse(File.ReadAllText(file));

                SetDI(services, jsonServices[jsonServices.First.Path]);
            }
        }

        private static void SetDI(this IServiceCollection services, JToken token)
        {
            var requiredServices = JsonConvert.DeserializeObject<List<Services>>(token.ToString());

            foreach (var service in requiredServices)
            {
                try
                {
                    services.AddScoped(
                                   Assembly.Load(service.Type.Package).GetType(service.Type.Class),
                                   Assembly.Load(service.InstanceType.Package).GetType(service.InstanceType.Class));
                }
                catch (System.Exception ex)
                {
                    string aa = ex.ToString();
                    //throw ex;
                }
            }
        }

        /// <summary>
        /// 配置ScopedDI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static void SetDI(this IServiceCollection services, Action<IocOption> option)
        {
            if (option == null)
            {                
                return;
            }
            var iocOption = new IocOption();
            option(iocOption);//客户端类型注入     

            var definedTypes = Assembly
                        .GetEntryAssembly()//获取默认程序集
                        .GetReferencedAssemblies()//获取所有引用程序集
                        .Select(Assembly.Load)
                        .SelectMany(y => y.DefinedTypes).ToList();

            //加入本地程序集内容
            definedTypes.AddRange(Assembly.GetEntryAssembly().DefinedTypes);
            foreach(var assembly in iocOption.Assemblies)
            {
                try
                {
                    var _assembly = Assembly.Load(assembly);
                    if (_assembly != null)
                    {
                        definedTypes.AddRange(_assembly.DefinedTypes);
                    }
                }
                catch
                { }
            }
            
            foreach (var type in iocOption.Types)
            {
                var allTypes = definedTypes.Where(t => type.GetTypeInfo().IsAssignableFrom(t.AsType()));

                var abstractTypes = allTypes.Where(t => t.IsInterface || t.IsAbstract);
                var implTypes = allTypes.Where(t => t.IsInterface == false && t.IsAbstract == false).ToList();

                
                foreach (var abstractType in abstractTypes)
                {
                    if (abstractTypes.Count() > 1 
                        && abstractType.AssemblyQualifiedName == type.AssemblyQualifiedName)
                    {
                        continue;
                    }
                    var _implTypes = implTypes.Where(t => t.ImplementedInterfaces.Where(impl => impl.AssemblyQualifiedName == abstractType.AssemblyQualifiedName).Count() > 0
                                || t.BaseType.AssemblyQualifiedName == abstractType.AssemblyQualifiedName);
                    foreach (var implClass in _implTypes)
                    {
                        if (iocOption.ExceptTypes.Count > 0)
                        {
                            if (iocOption.ExceptTypes.Where(
                                t => t.AssemblyQualifiedName == abstractType.AssemblyQualifiedName
                                || t.AssemblyQualifiedName == implClass.AssemblyQualifiedName).Count() > 0)
                            {
                                continue;
                            }
                        }
                        services.AddScoped(abstractType, implClass);
                    }
                }                
            }           
        }        
    }
}