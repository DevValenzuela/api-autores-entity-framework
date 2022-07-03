using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutores.Utilidades
{
    public class HATEOASFiltroAttribute : ResultFilterAttribute
    {
        protected bool DebeIncluirHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;
            if (!isAbsweSuccessFully(result))
            {
                return false;

            }

            var header = context.HttpContext.Request.Headers["incluirHATEOAS"];
            if (header.Count == 0)
            {
                return false;
            }

            var valor = header[0];

            if (!valor.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }

        private bool isAbsweSuccessFully(ObjectResult result)
        {
            if (result == null || result.Value == null)
            {
                return false;
            }

            if (result.StatusCode.HasValue  && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }
            return true;

        }
    }
}
