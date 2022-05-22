using Microsoft.AspNetCore.Mvc.Filters;

namespace PIA_BACKEND_MAGG.Filtros
{
    public class FiltroDeException : ExceptionFilterAttribute
    {

        private readonly ILogger<FiltroDeException> log;

        public FiltroDeException(ILogger<FiltroDeException> log)
        {
            this.log = log;
        }

        public override void OnException(ExceptionContext context)
        {
            log.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }

    }
}
