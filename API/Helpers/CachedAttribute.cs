using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;
        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        // Фильтры позволяют запускать код до или после определенных этапов конвейера обработки запроса
        // Мы можем запустить код до вызова метода действия и после того, как метод был выполнен.
        // Мы собираемся сделать и то, и другое, прежде чем проверим, есть ли что-то внутри кеша.
        // Если у нас его нет в нашем кеше, мы собираемся выполнить запрос, и результат этого мы помещаем в кеш.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

// Генерируем ключ для запроса
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);
            
            if(!string.IsNullOrEmpty(cachedResponse))
            {
                // Мы не разрешаем вызов контроллеру
                // Мы вернем кешированный элемент.
                var contentResult = new ContentResult
                {
                    Content =  cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;
                return;
            }

           // Если внутри кеша нет данных, мы разрешаем запрос.
            // После этого мы сохраняем результат de db внутри redis.
            var executedContext = await next(); // перейти к контроллеру

            if (executedContext.Result is OkObjectResult okObjectResult) 
            {
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }

        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
// Мы сортируем параметры строки запроса, чтобы они всегда были одинаковыми (ключ).
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");
            
            foreach (var (key,value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            };

            return keyBuilder.ToString();
        }
    }
}