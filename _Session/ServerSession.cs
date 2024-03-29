﻿using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Api.Config
{
    /// <summary>
    /// ServerSession
    /// </summary>
    public class ServerSession
    {
        private readonly ISessionService _sessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        internal static TimeSpan Span = TimeSpan.FromHours(2);
        
        /// <summary>
        /// ServerSession
        /// </summary>
        /// <param name="sessionService"></param>
        /// <param name="httpContextAccessor"></param>
        public ServerSession(ISessionService sessionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
        }

        public ISessionService GetService()
        {
            return _sessionService;
        }

        /// <summary>
        /// 获取session
        /// </summary>
        /// <returns></returns>
        public async Task<T> GetSessionAsync<T>()
        {
            var key = _httpContextAccessor.HttpContext.GetToken();            
            return await GetSessionAsync<T>(key);
        }

        /// <summary>
        /// 获取session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">关键Key</param>
        /// <returns></returns>
        public async Task<T> GetSessionAsync<T>(string key)
        {           
            if (key == null)
            {
                return default(T);
            }
            return await _sessionService.GetAsync<T>(key);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">关键Key</param>
        /// <returns></returns>
        public bool Contain(string key)
        {           
            return _sessionService.Contain(key);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">关键Key</param>
        /// <returns></returns>
        public async Task<bool> ContainAsync(string key)
        {
            return await _sessionService.ContainAsync(key);
        }

        /// <summary>
        /// 获取session
        /// </summary>
        /// <returns></returns>
        public T GetSession<T>()
        {
            var key = _httpContextAccessor.HttpContext.GetToken();
            return GetSession<T>(key);
        }

        /// <summary>
        /// 获取session
        /// </summary>
        /// <returns></returns>
        public T GetSession<T>(string key)
        {           
            if (key == null)
            {
                return default(T);
            }
            try
            {
                return _sessionService.Get<T>(key);
            }
            catch
            {
                return default(T);
            }            
        }

        /// <summary>
        /// 保存session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<bool> SetSessionAsync(Session session)
        {
            await _sessionService.SetAsync(session);
            SetToken(session);
            return true;
        }

        public void SetToken(Session session)
        {            
            if (session != null)
            {
                _httpContextAccessor.HttpContext.SetToken(session.Token);
            }
        }

        public bool ContainToken(Session session)
        {
            return _httpContextAccessor.HttpContext.GetToken() == session.Token;
        }

        public string GetToken()
        {
            return _httpContextAccessor.HttpContext.GetToken();
        }

        /// <summary>
        /// 保存session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool SetSession(Session session)
        {
            _sessionService.Set(session);
            if (session != null)
            {
                _httpContextAccessor.HttpContext.SetToken(session.Token);
            }
            return true;
        }

        public bool ClearSession(string token)
        {
            try
            {
                _httpContextAccessor.HttpContext.ClearToken();
                return _sessionService.Remove(new Session
                {
                    Token = token
                });
            }
            catch
            {
                return false;
            }            
            
        }

        public async Task<bool> ClearSessionAsync(string token)
        {
            try
            {
                _httpContextAccessor.HttpContext.ClearToken();
                return await _sessionService.RemoveAsync(new Session
                {
                    Token = token
                });
            }
            catch
            {
                return false;
            }            
        }
    }
}
