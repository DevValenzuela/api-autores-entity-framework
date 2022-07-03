using Microsoft.EntityFrameworkCore;

namespace WebAPIAutores.Utilidades
{
    public static class HTTPContextExtencions
    {
        public async static Task InsertParemetrePageHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null) { throw new ArgumentException(nameof(httpContext)); }
            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegister", count.ToString());

        }
    }
}
