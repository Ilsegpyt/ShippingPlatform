
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence;

internal static class AdvancedOperationQueryBuilder
{
    private sealed record Field(string Path, Type Type, bool IsContainer = false);

    private static readonly IReadOnlyDictionary<string, Field> Fields =
        new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase)
        {
            ["Created order"] = new("CreatedAtUtc", typeof(DateTime)),
            ["CreatedAtUtc"] = new("CreatedAtUtc", typeof(DateTime)),
            ["Operation ref"] = new("Id", typeof(Guid)),
            ["Id"] = new("Id", typeof(Guid)),
            ["Operation created date"] = new("CreatedAtUtc", typeof(DateTime)),
            ["Shipment #"] = new("ShipmentNumber", typeof(string)),
            ["ShipmentNumber"] = new("ShipmentNumber", typeof(string)),
            ["CNTR Volume"] = new("NumberOfContainers", typeof(int)),
            ["NumberOfContainers"] = new("NumberOfContainers", typeof(int)),
            ["operation"] = new("OperationType", typeof(string)),
            ["OperationType"] = new("OperationType", typeof(string)),
            ["Certificate Number"] = new("CertificateNumber", typeof(string)),
            ["CertificateNumber"] = new("CertificateNumber", typeof(string)),
            ["CLIENT"] = new("ClientId", typeof(Guid)),
            ["ClientId"] = new("ClientId", typeof(Guid)),
            ["Shipping Line"] = new("ShippingLine.Name", typeof(string)),
            ["ShippingLine"] = new("ShippingLine.Name", typeof(string)),
            ["M/BL"] = new("ImportDetails.MBLNumber", typeof(string)),
            ["MBLNumber"] = new("ImportDetails.MBLNumber", typeof(string)),
            ["destination"] = new("Containers.Destination", typeof(string), true),
            ["Destination"] = new("Containers.Destination", typeof(string), true),
            ["POD"] = new("ImportDetails.POD.Name", typeof(string)),
            ["Required Offloading Date"] = new("ImportDetails.RequiredOffloadingDate", typeof(DateOnly)),
            ["Required Offloading Time"] = new("ImportDetails.RequiredOffloadingTime", typeof(TimeOnly)),
            ["free time till"] = new("Containers.FreeTimeTill", typeof(DateOnly), true),
            ["FreeTimeTill"] = new("Containers.FreeTimeTill", typeof(DateOnly), true),
            ["Actual Arrive Date"] = new("Containers.ActualArrivalDate", typeof(DateOnly), true),
            ["ActualArrivalDate"] = new("Containers.ActualArrivalDate", typeof(DateOnly), true),
            ["Actual Arrive Time"] = new("Containers.ActualArrivalTime", typeof(TimeOnly), true),
            ["ActualArrivalTime"] = new("Containers.ActualArrivalTime", typeof(TimeOnly), true),
            ["Gate out Date"] = new("Containers.GateOutDate", typeof(DateOnly), true),
            ["GateOutDate"] = new("Containers.GateOutDate", typeof(DateOnly), true),
            ["Gate Out Time"] = new("Containers.GateOutTime", typeof(TimeOnly), true),
            ["GateOutTime"] = new("Containers.GateOutTime", typeof(TimeOnly), true),
            ["Container Number"] = new("Containers.ContainerNumber", typeof(string), true),
            ["ContainerNumber"] = new("Containers.ContainerNumber", typeof(string), true),
            ["Container Type"] = new("Containers.ContainerType", typeof(string), true),
            ["ContainerType"] = new("Containers.ContainerType", typeof(string), true),
            ["Booking Number"] = new("ExportDetails.BookingNumber", typeof(string)),
            ["BookingNumber"] = new("ExportDetails.BookingNumber", typeof(string)),
            ["POL"] = new("ExportDetails.POL.Name", typeof(string)),
            ["POW"] = new("ExportDetails.POW.Name", typeof(string)),
            ["Cut Off Date"] = new("ExportDetails.CutOffDate", typeof(DateOnly)),
            ["Customs"] = new("ExportDetails.Customs", typeof(string)),
            ["Clearance Type"] = new("ExportDetails.ClearanceType", typeof(string)),
            ["Status"] = new("Status", typeof(string)),
            ["Source"] = new("Source", typeof(string)),
            ["Delivered date"] = new("DeliveredAtUtc", typeof(DateTime)),
            ["Created by"] = new("CreatedByName", typeof(string))
        };

    public static bool IsValidField(string name) => Fields.ContainsKey(name);

    public static IQueryable<Operation> Apply(
        IQueryable<Operation> query,
        IReadOnlyList<Operations.Application.Operations.SearchOperations.OperationSearchRule>? rules,
        string? sortBy,
        string? sortDirection)
    {
        if (rules is { Count: > 0 })
        {
            Expression<Func<Operation, bool>>? combined = null;

            foreach (var rule in rules)
            {
                if (!Fields.TryGetValue(rule.Field, out var field))
                    throw new ArgumentException($"Unsupported search field: {rule.Field}");

                var predicate = BuildPredicate(field, rule.Operator, rule.Value);

                if (combined is null)
                {
                    combined = predicate;
                    continue;
                }

                var parameter = Expression.Parameter(typeof(Operation), "operation");
                var left = new ReplaceParameterVisitor(
                    combined.Parameters[0], parameter).Visit(combined.Body)!;
                var right = new ReplaceParameterVisitor(
                    predicate.Parameters[0], parameter).Visit(predicate.Body)!;

                var logic = rule.Logic ?? "AND";

                Expression body = logic.Equals("OR", StringComparison.OrdinalIgnoreCase)
                    ? Expression.OrElse(left, right)
                    : logic.Equals("NOT", StringComparison.OrdinalIgnoreCase)
                        ? Expression.AndAlso(left, Expression.Not(right))
                        : Expression.AndAlso(left, right);

                combined = Expression.Lambda<Func<Operation, bool>>(body, parameter);
            }

            if (combined is not null)
                query = query.Where(combined);
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            if (!Fields.TryGetValue(sortBy, out var sortField) || sortField.IsContainer)
                throw new ArgumentException($"Unsupported sort field: {sortBy}");

            var parameter = Expression.Parameter(typeof(Operation), "operation");
            Expression body = BuildMemberPath(parameter, sortField.Path);
            var keyType = body.Type;
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(Operation), keyType);
            var selector = Expression.Lambda(delegateType, body, parameter);

            var method =
                string.Equals(sortDirection, "Descending", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase)
                    ? "OrderByDescending"
                    : "OrderBy";

            var call = Expression.Call(
                typeof(Queryable),
                method,
                new[] { typeof(Operation), keyType },
                query.Expression,
                Expression.Quote(selector));

            query = query.Provider.CreateQuery<Operation>(call);
        }

        return query;
    }

    private static Expression<Func<Operation, bool>> BuildPredicate(
        Field field,
        string operation,
        string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(operation))
            throw new ArgumentException("Search operator is required.");

        var parameter = Expression.Parameter(typeof(Operation), "operation");

        if (field.IsContainer)
        {
            var anyMethod = typeof(Enumerable).GetMethods()
                .Single(m =>
                    m.Name == nameof(Enumerable.Any) &&
                    m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(OperationContainer));

            var containerParameter =
                Expression.Parameter(typeof(OperationContainer), "container");

            var path = field.Path["Containers.".Length..];
            var member = BuildMemberPath(containerParameter, path);
            var comparison = BuildComparison(member, field.Type, operation, rawValue);
            var lambda = Expression.Lambda(comparison, containerParameter);

            var containers = Expression.Property(
                parameter, nameof(Operation.Containers));

            var any = Expression.Call(anyMethod, containers, lambda);

            return Expression.Lambda<Func<Operation, bool>>(any, parameter);
        }

        var memberExpression = BuildMemberPath(parameter, field.Path);

        return Expression.Lambda<Func<Operation, bool>>(
            BuildComparison(memberExpression, field.Type, operation, rawValue),
            parameter);
    }

    private static Expression BuildComparison(
        Expression member,
        Type fieldType,
        string operation,
        string? rawValue)
    {
        var op = operation.Trim().ToLowerInvariant();
        var nullableUnderlying = Nullable.GetUnderlyingType(member.Type);
        var targetType = nullableUnderlying ?? member.Type;

        if (op is "isnull" or "is null")
            return Expression.Equal(member, Expression.Constant(null, member.Type));

        if (op is "notnull" or "is not null")
            return Expression.NotEqual(member, Expression.Constant(null, member.Type));

        if (targetType == typeof(string))
        {
            var value = rawValue ?? string.Empty;

            Expression notNull = member.Type == typeof(string)
                ? Expression.NotEqual(
                    member,
                    Expression.Constant(null, typeof(string)))
                : Expression.Constant(true);

            Expression text = member.Type == typeof(string)
                ? member
                : Expression.Call(
                    member,
                    member.Type.GetMethod("ToString", Type.EmptyTypes)!);

            var constant = Expression.Constant(value);

            Expression comparison = op switch
            {
                "equals" or "eq" or "=" =>
                    Expression.Equal(text, constant),

                "notequals" or "neq" or "!=" =>
                    Expression.NotEqual(text, constant),

                "contains" =>
                    Expression.Call(
                        text, nameof(string.Contains), Type.EmptyTypes, constant),

                "startswith" =>
                    Expression.Call(
                        text, nameof(string.StartsWith), Type.EmptyTypes, constant),

                "endswith" =>
                    Expression.Call(
                        text, nameof(string.EndsWith), Type.EmptyTypes, constant),

                _ => throw new ArgumentException(
                    $"Unsupported operator '{operation}' for text field.")
            };

            return Expression.AndAlso(notNull, comparison);
        }

        if (string.IsNullOrWhiteSpace(rawValue))
            throw new ArgumentException(
                $"A value is required for operator '{operation}'.");

        object parsed;

        try
        {
            if (targetType.IsEnum)
                parsed = Enum.Parse(targetType, rawValue, true);
            else if (targetType == typeof(Guid))
                parsed = Guid.Parse(rawValue);
            else if (targetType == typeof(DateOnly))
                parsed = DateOnly.Parse(rawValue, CultureInfo.InvariantCulture);
            else if (targetType == typeof(TimeOnly))
                parsed = TimeOnly.Parse(rawValue, CultureInfo.InvariantCulture);
            else if (targetType == typeof(DateTime))
                parsed = DateTime.Parse(
                    rawValue,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal);
            else
                parsed = Convert.ChangeType(
                    rawValue, targetType, CultureInfo.InvariantCulture)!;
        }
        catch
        {
            throw new ArgumentException(
                $"Value '{rawValue}' is invalid for this field.");
        }

        var constantValue = Expression.Constant(parsed, targetType);

        Expression comparableMember = nullableUnderlying is null
            ? member
            : Expression.Property(member, "Value");

        Expression result = op switch
        {
            "equals" or "eq" or "=" =>
                Expression.Equal(comparableMember, constantValue),

            "notequals" or "neq" or "!=" =>
                Expression.NotEqual(comparableMember, constantValue),

            "greaterthan" or "gt" or ">" =>
                Expression.GreaterThan(comparableMember, constantValue),

            "greaterthanorequal" or "gte" or ">=" =>
                Expression.GreaterThanOrEqual(comparableMember, constantValue),

            "lessthan" or "lt" or "<" =>
                Expression.LessThan(comparableMember, constantValue),

            "lessthanorequal" or "lte" or "<=" =>
                Expression.LessThanOrEqual(comparableMember, constantValue),

            _ => throw new ArgumentException(
                $"Unsupported operator '{operation}' for this field.")
        };

        return nullableUnderlying is null
            ? result
            : Expression.AndAlso(
                Expression.Property(member, "HasValue"),
                result);
    }

    private static Expression BuildMemberPath(Expression root, string path)
    {
        Expression current = root;

        foreach (var segment in path.Split('.'))
        {
            if (current.Type == typeof(string))
                throw new ArgumentException($"Invalid field path '{path}'.");

            current = Expression.PropertyOrField(current, segment);
        }

        return current;
    }

    private sealed class ReplaceParameterVisitor(
        ParameterExpression source,
        ParameterExpression target)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == source ? target : base.VisitParameter(node);
    }
}
