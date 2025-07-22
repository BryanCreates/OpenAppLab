using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenAppLab.Core.UI.Shared.GraphQL;

public class GraphQLQueryable<T>
{
    private readonly string _queryName;
    private readonly List<Expression<Func<T, bool>>> _filters = new();
    private readonly Dictionary<string, LambdaExpression> _collectionFilters = new();
    private readonly Dictionary<string, LambdaExpression> _collectionSelectionFilters = new();
    private int? _first;
    private string _after = "";
    private Expression<Func<T, object>>? _selector;
    private string? _selectClause;

    public GraphQLQueryable(string queryName)
    {
        _queryName = queryName;
    }

    public GraphQLQueryable<T> Where(Expression<Func<T, bool>> filter)
    {
        _filters.Add(filter);
        return this;
    }

    public GraphQLQueryable<T> WhereOnCollection<TSub>(string property, Expression<Func<TSub, bool>> filter)
    {
        _collectionFilters[property] = filter;
        return this;
    }

    public GraphQLQueryable<T> SelectWhere<TSub>(string property, Expression<Func<TSub, bool>> filter)
    {
        _collectionSelectionFilters[property] = filter;
        return this;
    }

    public GraphQLQueryable<T> Page(int first, string after)
    {
        _first = first;
        _after = after;
        return this;
    }

    public GraphQLQueryable<T> Select<TContract>()
    {
        _selectClause = GraphQLQueryBuilder.BuildSelectionFromType(typeof(TContract), _collectionSelectionFilters);
        return this;
    }

    public GraphQLQueryable<T> Select(Expression<Func<T, object>> selector)
    {
        _selector = selector;
        return this;
    }

    public string BuildQuery()
    {
        return GraphQLQueryBuilder.Build(
            _queryName,
            _filters,
            _first,
            _after,
            _selector,
            _collectionFilters,
            _collectionSelectionFilters,
            _selectClause);
    }
}

public static class GraphQLQueryBuilder
{
    public static string Build<T>(
        string queryName,
        List<Expression<Func<T, bool>>> filters,
        int? first,
        string? after,
        Expression<Func<T, object>>? selector,
        Dictionary<string, LambdaExpression>? collectionFilters,
        Dictionary<string, LambdaExpression>? collectionSelections,
        string? selectClause)
    {
        var sb = new StringBuilder();
        sb.AppendLine("query {");
        sb.Append($"  {queryName}");

        var args = new List<string>();
        if (first.HasValue) args.Add($"first: {first.Value}");
        if (!string.IsNullOrEmpty(after)) args.Add($"after: \"{after}\"");

        var whereClause = BuildWhere(filters.Cast<LambdaExpression>().ToList(), collectionFilters);
        if (!string.IsNullOrEmpty(whereClause)) args.Add($"where: {whereClause}");

        if (args.Any())
            sb.Append($"({string.Join(", ", args)})");

        sb.AppendLine(" {");

        if (!string.IsNullOrWhiteSpace(selectClause))
        {
            sb.AppendLine(selectClause);
        }

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

    public static string BuildSelectionFromType(
    Type type,
    Dictionary<string, LambdaExpression>? collectionSelections = null,
    string parentPath = "")
    {
        var sb = new StringBuilder();
        sb.AppendLine("    edges {");
        sb.AppendLine("      node {");

        foreach (var prop in type.GetProperties())
        {
            var name = prop.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                           .Cast<JsonPropertyNameAttribute>()
                           .FirstOrDefault()?.Name ?? prop.Name.ToCamelCase();

            var fullPath = string.IsNullOrEmpty(parentPath) ? name : $"{parentPath}.{name}";
            var propType = prop.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;

            if (IsSimple(underlyingType))
            {
                sb.AppendLine($"        {name}");
            }
            else if (IsEnumerable(propType))
            {
                var itemType = GetEnumerableItemType(propType);
                if (itemType == null) continue;

                // Check if there's a filter expression for this collection
                if (collectionSelections != null && collectionSelections.TryGetValue(name, out var filterExpr))
                {
                    var cond = ParseExpression(filterExpr.Body);
                    sb.AppendLine($"        {name}(where:{{{cond}}}) {{");
                }
                else
                {
                    sb.AppendLine($"        {name} {{");
                }

                // Get child fields from itemType (skip extra edge/node recursion)
                var nestedFields = BuildSelectionFields(itemType, collectionSelections, fullPath);
                foreach (var line in nestedFields)
                    sb.AppendLine($"          {line}");

                sb.AppendLine("        }");
            }
            else
            {
                sb.AppendLine($"        {name} {{");

                var subFields = BuildSelectionFields(underlyingType, collectionSelections, fullPath);
                foreach (var line in subFields)
                    sb.AppendLine($"          {line}");

                sb.AppendLine("        }");
            }
        }

        sb.AppendLine("      }"); // end node
        sb.AppendLine("      cursor");
        sb.AppendLine("    }");   // end edges

        return sb.ToString();
    }
    
    private static IEnumerable<string> BuildSelectionFields(
    Type type,
    Dictionary<string, LambdaExpression>? collectionSelections,
    string parentPath)
    {
        foreach (var prop in type.GetProperties())
        {
            var name = prop.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                           .Cast<JsonPropertyNameAttribute>()
                           .FirstOrDefault()?.Name ?? prop.Name.ToCamelCase();

            var fullPath = string.IsNullOrEmpty(parentPath) ? name : $"{parentPath}.{name}";
            var propType = prop.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;

            if (IsSimple(underlyingType))
            {
                yield return name;
            }
            else if (IsEnumerable(propType))
            {
                var itemType = GetEnumerableItemType(propType);
                if (itemType == null) continue;

                if (collectionSelections != null && collectionSelections.TryGetValue(name, out var filterExpr))
                {
                    var cond = ParseExpression(filterExpr.Body);
                    yield return $"{name}(where:{{{cond}}}) {{";
                }
                else
                {
                    yield return $"{name} {{";
                }

                var nested = BuildSelectionFields(itemType, collectionSelections, fullPath);
                foreach (var line in nested)
                    yield return "  " + line;

                yield return "}";
            }
            else
            {
                yield return $"{name} {{";
                var sub = BuildSelectionFields(underlyingType, collectionSelections, fullPath);
                foreach (var line in sub)
                    yield return "  " + line;
                yield return "}";
            }
        }
    }

    private static bool IsSimple(Type type)
    {
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(Guid) || type == typeof(DateTime);
    }

    private static bool IsEnumerable(Type type)
    {
        return typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }
    private static Type? GetEnumerableItemType(Type type)
    {
        if (type.IsArray)
            return type.GetElementType();

        if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            return type.GetGenericArguments().FirstOrDefault();

        return type.GetInterfaces()
                   .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                   .Select(t => t.GetGenericArguments().FirstOrDefault())
                   .FirstOrDefault();
    }

    private static string BuildWhere(List<LambdaExpression> filters, Dictionary<string, LambdaExpression>? subFilters)
    {
        var combined = filters.Select(f => ParseExpression(f.Body)).ToList();

        if (subFilters != null)
        {
            foreach (var kv in subFilters)
            {
                var subCondition = ParseExpression(kv.Value.Body);
                combined.Add($"{kv.Key.ToCamelCase()}: {{ some: {{ {subCondition} }} }}");
            }
        }

        return combined.Count switch
        {
            0 => "",
            1 => $"{{ {combined[0]} }}",
            _ => $"{{ and: [ {string.Join(", ", combined.Select(c => $"{{ {c} }}"))} ] }}"
        };
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
        if (expression.NodeType is ExpressionType.OrElse or ExpressionType.AndAlso)
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
            _ => throw new NotSupportedException("Left side of binary expression must be a member access.")
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
