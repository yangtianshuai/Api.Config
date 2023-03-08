using Microsoft.AspNetCore.Mvc;

namespace Api.Config
{
    /// <summary>
    /// Response扩展
    /// </summary>
    public static class ResponseExtension
    {
        /// <summary>
        /// 返回Json
        /// </summary>
        /// <returns></returns>
        public static JsonResult ToJson(this ResponseResult _result)
        {            
            return new JsonResult(_result);
        }

        /// <summary>
        /// 返回Html
        /// </summary>
        /// <param name="_result"></param>
        /// <returns></returns>
        public static ContentResult ToHtml(this ResponseResult _result)
        {
            return ToContent(_result, "text/html");
        }

        /// <summary>
        /// 返回Content
        /// </summary>
        /// <param name="_result"></param>
        /// <param name="cotentType">内容类型</param>
        /// <returns></returns>
        private static ContentResult ToContent(this ResponseResult _result,string cotentType)
        {
            return new ContentResult
            {
                Content = _result.Data.ToString(),
                ContentType = cotentType
            };
        }
        /// <summary>
        /// 返回文件
        /// </summary>
        /// <param name="_result"></param>
        /// <returns></returns>
        public static RedirectResult ToRedirect(this ResponseResult _result, string url)
        {
            return new RedirectResult(url);
        }


        /// <summary>
        /// 返回文件
        /// </summary>
        /// <param name="_result"></param>
        /// <returns></returns>
        public static FileContentResult ToFile(this ResponseResult _result)
        {
            return ToFile(_result, null);
        }

        /// <summary>
        /// 返回文件
        /// </summary>
        /// <param name="_result"></param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static FileContentResult ToFile(this ResponseResult _result, string fileName)
        {
            return ToFile(_result,fileName, "application/zip");
        }

        /// <summary>
        /// 返回文件
        /// </summary>
        /// <param name="_result"></param>
        /// <returns></returns>
        public static FileContentResult ToFile(this ResponseResult _result, string fileName, string contentType)
        {
            if (!(_result.Data is byte[])) throw new System.Exception("Data必须是文件类型");
            return new FileContentResult((byte[])_result.Data, contentType)
            {
                FileDownloadName = fileName
            };
        }        
    }
}