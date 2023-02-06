using System.Linq;
using System.Linq.Expressions;

namespace Adhoc.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> query, Action<T> method)
        {
            foreach (T item in query)
            {
                method(item);
            }
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> query, string columnName, object value)
        {
            var predicate = CreatePredicate<T>(columnName, value);

            return query.AsQueryable().Where(predicate).ToList();
        }

        public static Expression<Func<T, bool>> CreatePredicate<T>(string columnName, object searchValue)
        {
            var xType = typeof(T);
            var x = Expression.Parameter(xType, "x");
            var column = xType.GetProperties().FirstOrDefault(p => p.Name == columnName);

            var body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.Equal(
                    Expression.PropertyOrField(x, columnName),
                    Expression.Constant(searchValue));

            return Expression.Lambda<Func<T, bool>>(body, x);
        }
    }
}
