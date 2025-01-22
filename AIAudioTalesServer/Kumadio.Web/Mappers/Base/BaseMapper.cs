namespace Kumadio.Web.Mappers.Base
{
    public abstract class BaseMapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public TDestination? Map(TSource? source)
        {
            if (source == null) return default;

            return MapCore(source);
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
