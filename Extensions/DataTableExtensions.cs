using FastMember;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Adhoc.Common.Extensions
{
    public static class DataTableExtensions
    {
        private static Dictionary<string, Member> GetMemberMappings<T>(TypeAccessor typeAccessor, DataTable table)
        {
            var columnNames = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToList();
            var mappings = new Dictionary<string, Member>();

            typeAccessor.GetMembers().ForEach(member =>
            {
                if (member.IsDefined(typeof(ColumnAttribute)))
                {
                    var col = (ColumnAttribute)typeof(T).GetProperty(member.Name).GetCustomAttributes(typeof(ColumnAttribute), false).Single();
                    if (columnNames.Contains(col.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        mappings.Add(col.Name, member);
                    }
                    else
                    {
                        if (columnNames.Contains(member.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            mappings.Add(member.Name, member);
                        }
                    }
                }
                else
                {
                    if (columnNames.Contains(member.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        mappings.Add(member.Name, member);
                    }
                }
            });
            return mappings;
        }

        public static List<T> ToList<T>(this DataTable table)
        {
            var accessor = TypeAccessor.Create(typeof(T));
            var mappings = GetMemberMappings<T>(accessor, table);

            return table.AsEnumerable().Select(row =>
            {
                var t = (T)accessor.CreateNew();
                mappings.ForEach(mapping =>
                {
                    accessor[t, mapping.Value.Name] = GetConvertedValue(row[mapping.Key], mapping.Value.Type);
                });
                return t;
            }).ToList();
        }

        private static object GetConvertedValue(object value, Type type)
        {
            return value == DBNull.Value ? null : ConvertNonNullValueToTypeCompatibleValue(value, type);
        }

        private static object ConvertNonNullValueToTypeCompatibleValue(object o, Type type)
        {
            if (type == typeof(int) && o is string)
            {
                return (string.IsNullOrEmpty((string)o)) ? 0 : int.Parse((string)o);
            }

            if (type == typeof(decimal) && o is string)
            {
                return (string.IsNullOrEmpty((string)o)) ? 0 : decimal.Parse((string)o);
            }

            if (type == typeof(char) && o is string)
            {
                return char.Parse((string)o);
            }

            //Should be checked at last, to allow other type type casting to happen
            if (type == typeof(string) && !(o is string))
            {
                return Convert.ToString(o);
            }

            if ((type == typeof(bool) && o is int) || (type==typeof(System.Boolean)))
            {
                return (Convert.ToInt32(o) == 0 ? false : true);
            }
            if(type==typeof(System.DateTime?) && o.ToString().ToUpper() == "NULL")
            {
                return null;
            }
            return o;
        }
    }
}
