using System.Linq.Expressions;
using System.Text;

namespace OpenAppLab.Core.UI.Shared.GraphQL;
public class GraphQLQueryable<T>
{
    private readonly List<Expression<Func<T, bool>>> _filters = new();
    private int? _page;
    private int? _pageSize;

    public GraphQLQueryable<T> Where(Expression<Func<T, bool>> filter)
    {
        _filters.Add(filter);
        return this;
    }

    public GraphQLQueryable<T> Page(int page, int pageSize)
    {
        _page = page;
        _pageSize = pageSize;
        return this;
    }

    //public async Task<List<T>> ToListAsync()
    //{
    //    var gql = GraphQLQueryBuilder.Build<T>(_filters, _page, _pageSize);
    //    return await GraphQLHttpClientService.ExecuteQueryAsync<T>(gql);
    //}
}

public static class GraphQLQueryBuilder
{
    public static string Build<T>(
        List<Expression<Func<T, bool>>> filters, int? page, int? pageSize)
    {
        var filterBuilder = new StringBuilder();
        foreach (var filter in filters)
        {
            var lambda = filter.Body as BinaryExpression;
            if (lambda?.NodeType == ExpressionType.Equal)
            {
                var left = ((MemberExpression)lambda.Left).Member.Name;
                var right = ((ConstantExpression)lambda.Right).Value;
                filterBuilder.Append($"{left.ToCamelCase()}: {{ eq: \"{right}\" }}, ");
            }
        }

        var filterString = filterBuilder.Length > 0
            ? $"filter: {{ {filterBuilder.ToString().TrimEnd(',', ' ')} }}"
            : "";

        var paging = "";
        if (page.HasValue && pageSize.HasValue)
        {
            paging = $"page: {page.Value}, pageSize: {pageSize.Value}";
        }

        return @$"""
        query {{posts({filterString} {paging}) {{
                items {{
                    id
                    title
                    postType
                    jsonData
                }}
            }}
        }}
        """;
    }
}

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}