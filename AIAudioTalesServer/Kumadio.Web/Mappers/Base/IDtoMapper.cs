namespace Kumadio.Web.Mappers.Base
{
    public interface IDtoMapper<TSource, TDestination>
    {
        public TDestination? Map(TSource? source);
        public IList<TDestination?> Map(IEnumerable<TSource>? sources);
        public TSource? ReverseMap(TDestination? destination);
    }
}
