using WebAPIAutores.DTOs;

namespace WebAPIAutores.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Pagina - 1) * paginationDTO.RecordsByPage)
                .Take(paginationDTO.RecordsByPage);
        }
    }
}
