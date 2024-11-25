using System.Linq.Expressions;

namespace Wigle2Geo.ExpressionVisitors
{
    /// <summary>
    /// SqliteQueryReplacer<TEntity> is a custom ExpressionVisitor class used to rewrite certain LINQ query expressions.
    /// Specifically, it looks for calls to the `Any` method with two arguments: an IEnumerable<string> and a predicate.
    /// If found, it replaces the `Any` method call with a series of predicate invocations joined by logical OR expressions.
    /// This is typically used to convert collection-based expressions into a format more suitable for SQLite queries.
    /// </summary>
    public class SqliteExpressionOptimizer : ExpressionVisitor
    {
        /// <summary>
        /// This method visits method calls in the expression tree and checks if they represent
        /// the `Any` method call with specific argument types. If so, it rewrites the expression.
        /// </summary>
        /// <param name="node">The method call expression being visited.</param>
        /// <returns>The modified or original expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Check if the method is "Any", there are exactly two arguments and validate that the first argument is an IEnumerable<string>
            if (node.Method.Name == "Any" && node.Arguments.Count == 2 && node.Arguments[0].Type.IsAssignableTo(typeof(IEnumerable<string>)))
            {
                // Compile and execute the first argument to get the actual collection of strings
                var targetObject = Expression.Lambda<Func<IEnumerable<string>>>(node.Arguments[0]).Compile()();

                // The second argument should be a predicate (expression that takes a string and returns a bool)
                var predicate = node.Arguments[1];

                // If the collection is empty, the result of the query is always false
                if (!targetObject.Any())
                    return Expression.Constant(false);

                // Create a series of predicate invocations for each string in the collection.
                // For each string, the predicate will be invoked, and all results will be combined using OR.
                return targetObject
                  .Select(s => Expression.Invoke(predicate, Expression.Constant(s)))  // Invoke the predicate for each string
                  .Aggregate<Expression>(Expression.OrElse);                          // Combine expressions with OR logic (short-circuiting)
            }

            // For any other method calls, use the base class's default behavior (no modification)
            return base.VisitMethodCall(node);
        }

        public static Expression<Func<TEntity, bool>> Transform<TEntity>(Expression<Func<TEntity, bool>> inputExpression)
        {
            var body = (new SqliteExpressionOptimizer()).Visit(inputExpression.Body);
            return Expression.Lambda<Func<TEntity, bool>>(body, inputExpression.Parameters);
        }
    }
}