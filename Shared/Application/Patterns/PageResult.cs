namespace NovaScale.Platform.API.Shared.Application.Patterns;

public record PageResult<T>(IEnumerable<T> Items, int Total, int Page, int PageSize)
{
    public PageResult<TNext> Map<TNext>(Func<T, TNext> map) => new(Items.Select(map), Total, Page, PageSize);
}
