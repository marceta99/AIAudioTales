namespace Kumadio.Core.Mappers.Base
{
    public interface IModelMapper<TSource, TDestination>
    {
        public TDestination? Map(TSource? source);
        public IList<TDestination?> Map(IEnumerable<TSource>? sources);
        public TSource? ReverseMap(TDestination? destination);
    }
}
