namespace Kumadio.Core.Mappers.Base
{
    public abstract class ModelMapper<TSource, TDestination> : IModelMapper<TSource, TDestination>
    {
        public TDestination? Map(TSource? source)
        {
            if (source == null) return default;

            return MapCore(source);
        }

        public IList<TDestination?> Map(IEnumerable<TSource>? sources)
        {
            if (sources == null) return new List<TDestination?>();

            return sources
                .Select(item => Map(item))
                .ToList();
        }

        public TSource? ReverseMap(TDestination? destination)
        {
            if (destination == null) return default;

            return ReverseMapCore(destination);
        }

        public abstract TDestination MapCore(TSource source);

        public virtual TSource ReverseMapCore(TDestination destination)
        {
            throw new NotSupportedException($"{GetType().Name} does not support reverse mapping by default.");
        }
    }
}
