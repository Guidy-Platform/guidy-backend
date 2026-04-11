namespace CoursePlatform.Application.Common.Models;

public class Pagination<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => PageIndex > 1;
    public bool HasNext => PageIndex < TotalPages;
    public IReadOnlyList<T> Data { get; set; } = [];

    public Pagination() { }

    public Pagination(int pageIndex, int pageSize, int totalCount, IReadOnlyList<T> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        Data = data;
    }
}