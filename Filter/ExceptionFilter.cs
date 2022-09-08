using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace Api.Config
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void OnException(ExceptionContext context)
        {
            var result = new ResponseResult();
            //写入错误日志
            _logger.Error(context.Exception);
            //返回客户端消息
            result.Message = context.Exception.Message;
            context.HttpContext.Response.StatusCode = 200;
            context.Result = result.ToJson();
            context.ExceptionHandled = true;
        }
    }
}