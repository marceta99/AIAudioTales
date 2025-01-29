namespace Kumadio.Web.Mappers.Base
{
    public interface IMapper<TSource, TDestination>
    {
        public TDestination? Map(TSource? source);
        public IList<TDestination?> Map(IEnumerable<TSource>? sources);
        public TSource? ReverseMap(TDestination? destination);
    }
}
