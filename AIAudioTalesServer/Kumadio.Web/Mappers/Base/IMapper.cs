namespace Kumadio.Web.Mappers.Base
{
    public interface IMapper<TSource, TDestination>
    {
        public TDestination? Map(TSource source);

        public TSource? ReverseMap(TDestination destination);
    }
}
