using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SISTEMA_DE_INVENTARIO4.Attributes
{
    public class SimpleThrottleAttribute : ActionFilterAttribute
    {
        private static Dictionary<string, List<DateTime>> _requests = new();
        
        public int MaxRequests { get; set; } = 5;
        public int Seconds { get; set; } = 10;
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocido";
            
            if (!_requests.ContainsKey(ip))
                _requests[ip] = new List<DateTime>();
            
            _requests[ip].RemoveAll(f => f < DateTime.Now.AddSeconds(-Seconds));
            
            if (_requests[ip].Count >= MaxRequests)
            {
                var espera = (int)(_requests[ip].First().AddSeconds(Seconds) - DateTime.Now).TotalSeconds;
                
                context.Result = new ContentResult
                {
                    StatusCode = 429,
                    Content = $"⛔ Límite: {MaxRequests} peticiones cada {Seconds}s. Espera {espera}s."
                };
                return;
            }
            
            _requests[ip].Add(DateTime.Now);
        }
    }
}