using System;
using System.Collections.Generic;

namespace Api.Config.DI
{
    /// <summary>
    /// Ioc选择项
    /// </summary>
    public class IocOption
    {
        /// <summary>
        /// Ioc类型
        /// </summary>
        public List<Type> Types { get; private set; } = new List<Type>();
        /// <summary>
        /// 需要排除的类型
        /// </summary>
        public List<Type> ExceptTypes { get; private set; } = new List<Type>();
        /// <summary>
        /// 扩展程序集
        /// </summary>
        public List<string> Assemblies { get; private set; } = new List<string>();

        /// <summary>
        /// 添加目标接口
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public IocOption Add(params Type[] types)
        {
            foreach (var type in types)
            {
                if (Types.Find(t => t.Name == type.Name
                && t.Namespace == type.Namespace) == null)
                {
                    Types.Add(type);
                }
            }
            return this;
        }
        /// <summary>
        /// 排除类型接口
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public IocOption Excepts(params Type[] types)
        {
            foreach(var type in types)
            {
                if (ExceptTypes.Find(t => t.Name == type.Name
                && t.Namespace == type.Namespace) == null)
                {
                    ExceptTypes.Add(type);
                }
            }            
            return this;
        }
        /// <summary>
        /// 实现扩展程序集
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public IocOption Assembly(params string[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (!Assemblies.Contains(assembly))
                {
                    Assemblies.Add(assembly);
                }
            }
            return this;
        }
    }
}