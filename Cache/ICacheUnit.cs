using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.Cache
{
    public interface ICacheUnit
    {
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        bool Contain(string key);
        
    }
}
