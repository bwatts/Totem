namespace Totem.Map;

public interface IMapTypeKeyed : ITypeKeyed
{
    MapType MapTypeKey { get; }
}
