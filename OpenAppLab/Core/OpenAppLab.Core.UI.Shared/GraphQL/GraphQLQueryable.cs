using System.Linq.Expressions;
using System.Text;

namespace OpenAppLab.Core.UI.Shared.GraphQL;

public class GraphQLQueryable<T>
{
    private readonly string _queryName;
    private readonly List<Expression<Func<T, bool>>> _filters = new();
    private int? _first;
    private string _after = "";
    private Expression<Func<T, object>>? _selector;

    public GraphQLQueryable(string queryName)
    {
        _queryName = queryName;
    }

    public GraphQLQueryable<T> Where(Expression<Func<T, bool>> filter)
    {
        _filters.Add(filter);
        return this;
    }

    public GraphQLQueryable<T> Page(int first, string after)
    {
        _first = first;
        _after = after;
        return this;
    }

    public GraphQLQueryable<T> Select(Expression<Func<T, object>> selector)
    {
        _selector = selector;
        return this;
    }

    public string BuildQuery()
    {
        return GraphQLQueryBuilder.Build(_queryName, _filters, _first, _after, _selector);
    }
}

public static class GraphQLQueryBuilder
{
    public static string Build<T>(
        string queryName,
        List<Expression<Func<T, bool>>> filters,
        int? first,
        string? after,
        Expression<Func<T, object>>? selector)
    {
        var sb = new StringBuilder();
        sb.AppendLine("query {");
        sb.Append($"  {queryName}");

        var args = new List<string>();
        if (first.HasValue) args.Add($"first: {first.Value}");
        if (!string.IsNullOrEmpty(after)) args.Add($"after: \"{after}\"");

        var whereClause = BuildWhere(filters.Cast<LambdaExpression>().ToList());
        if (!string.IsNullOrEmpty(whereClause)) args.Add($"where: {whereClause}");

        if (args.Any())
            sb.Append($"({string.Join(", ", args)})");

        sb.AppendLine(" {");
        sb.AppendLine("    edges {");
        sb.AppendLine("      node {");

        var fields = selector != null
            ? ExtractSelectedFields(selector.Body)
            : new[] { "id", "title", "content" };

        foreach (var field in fields)
            sb.AppendLine($"        {field}");

        sb.AppendLine("      }");
        sb.AppendLine("      cursor");
        sb.AppendLine("    }");
        sb.AppendLine("    pageInfo {");
        sb.AppendLine("      startCursor");
        sb.AppendLine("      endCursor");
        sb.AppendLine("      hasNextPage");
        sb.AppendLine("      hasPreviousPage");
        sb.AppendLine("    }");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string BuildWhere(List<LambdaExpression> filters)
    {
        if (!filters.Any()) return "";

        var combined = filters
            .Select(f => ParseExpression(f.Body))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        return combined.Count == 1
            ? $"{{ {combined[0]} }}"
            : $"{{ or: [ {string.Join(", ", combined.Select(c => $"{{ {c} }}"))} ] }}";
    }

    private static string ParseExpression(Expression expr)
    {
        return expr switch
        {
            BinaryExpression b => ParseBinary(b),
            UnaryExpression u => ParseUnary(u),
            _ => throw new NotSupportedException($"Unsupported expression type: {expr.NodeType}")
        };
    }

    private static string ParseBinary(BinaryExpression expression)
    {
        if (expression.NodeType == ExpressionType.OrElse || expression.NodeType == ExpressionType.AndAlso)
        {
            string op = expression.NodeType == ExpressionType.OrElse ? "or" : "and";
            var left = ParseExpression(expression.Left);
            var right = ParseExpression(expression.Right);
            return $"{op}: [{{ {left} }}, {{ {right} }}]";
        }

        var memberExpr = expression.Left switch
        {
            MemberExpression m => m,
            UnaryExpression u when u.Operand is MemberExpression m => m,
            _ => throw new NotSupportedException($"Left side of binary expression must be a member access. Got: {expression.Left.NodeType}")
        };

        var memberName = memberExpr.Member.Name.ToCamelCase();
        var value = GetValue(expression.Right);
        var formattedValue = FormatValue(value);

        return $"{memberName}: {{ eq: {formattedValue} }}";
    }

    private static string ParseUnary(UnaryExpression expression)
    {
        if (expression.NodeType == ExpressionType.Not)
        {
            var inner = ParseExpression(expression.Operand);
            return $"not: {{ {inner} }}";
        }

        throw new NotSupportedException($"Unary operator '{expression.NodeType}' is not supported.");
    }

    private static object? GetValue(Expression expr)
    {
        return expr switch
        {
            ConstantExpression ce => ce.Value,
            MemberExpression me => Expression.Lambda(me).Compile().DynamicInvoke(),
            UnaryExpression ue => Expression.Lambda(ue).Compile().DynamicInvoke(),
            _ => Expression.Lambda(expr).Compile().DynamicInvoke()
        };
    }

    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "null",
            string s => $"\"{s}\"",
            bool b => b.ToString().ToLower(),
            _ => value?.ToString() ?? "null"
        };
    }

    private static IEnumerable<string> ExtractSelectedFields(Expression body)
    {
        return body switch
        {
            NewExpression newExpr when newExpr.Members != null => newExpr.Members.Select(m => m.Name.ToCamelCase()),
            MemberInitExpression memberInit => memberInit.Bindings.Select(b => b.Member.Name.ToCamelCase()),
            MemberExpression memberExpr => new[] { memberExpr.Member.Name.ToCamelCase() },
            UnaryExpression unary when unary.Operand is MemberExpression unaryMember => new[] { unaryMember.Member.Name.ToCamelCase() },
            _ => new[] { "id", "title" }
        };
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
