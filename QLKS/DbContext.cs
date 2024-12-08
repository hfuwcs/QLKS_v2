using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace QLKS
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }

        public ColumnAttribute(string name, bool isPrimaryKey = false, bool isIdentity = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            IsIdentity = isIdentity;
        }
    }

    public static class DbContextExtensions
    {
        public static IEnumerable<T> Parse<T>(this DataTable table)
        {
            List<T> result = new List<T>();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            foreach (DataRow row in table.Rows)
            {
                T curr = (T)Activator.CreateInstance(type);
                foreach (PropertyInfo property in properties)
                {
                    ColumnAttribute columnAttr = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                    if (columnAttr == null) continue;
                    string column = columnAttr.Name;
                    object pValue = row[column];
                    if (pValue.ToString().Length == 0)
                        continue;
                    try
                    {
                        property.SetValue(curr, pValue, null);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return Enumerable.Empty<T>();
                    }
                }
                result.Add(curr);
            }
            return result;
        }
    }

    public class DbContext
    {
        public enum ConnectionType
        {
            ConnectionString,
            ConfigurationManager
        }

        SqlConnection conn;
        public SqlConnection Connection
        {
            get => conn;
            set => conn = value;
        }

        public DbContext(ConnectionType type, string text)
        {
            conn = new SqlConnection();
            if (type == ConnectionType.ConnectionString)
                conn.ConnectionString = text;
            else
                conn.ConnectionString = ConfigurationManager.ConnectionStrings[text].ConnectionString;
        }

        private void Open()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }

        private void Close()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        public IEnumerable<T> GetTable<T>(Expression<Func<T, bool>> predicate = null, int page = 1, int size = 0)
        {
            List<T> result = new List<T>();
            if (page < 1 || size < 0)
                return result;
            Type type = typeof(T);
            TableAttribute tableAttr = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));
            if (tableAttr == null)
                return result;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {tableAttr.Name} ORDER BY (SELECT NULL) OFFSET {(page - 1) * size} ROWS";
            if (size > 0)
                cmd.CommandText += $" FETCH NEXT {size} ROWS ONLY";
            cmd.CommandType = CommandType.Text;
            PropertyInfo[] properties = type.GetProperties();
            Debug.WriteLine(cmd.CommandText);
            try
            {
                Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T curr = (T)Activator.CreateInstance(typeof(T));
                        foreach (PropertyInfo property in properties)
                        {
                            ColumnAttribute columnAttr = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                            if (columnAttr == null) continue;
                            string column = columnAttr.Name;
                            object pValue = reader[column];
                            if (pValue.ToString().Length == 0)
                                continue;
                            property.SetValue(curr, pValue, null);
                        }
                        if (predicate != null)
                        {
                            if (predicate.Compile()(curr))
                                result.Add(curr);
                        }
                        else
                            result.Add(curr);
                    }
                }
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Enumerable.Empty<T>();
            }
            return result;
        }

        string FromAddition(params Type[] fromAddition)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Type t in fromAddition)
            {
                TableAttribute tableAttr = (TableAttribute)t.GetCustomAttribute(typeof(TableAttribute));
                if (tableAttr == null) continue;
                builder.Append($", {tableAttr.Name}");
            }
            return builder.ToString();
        }

        public IEnumerable<T> GetTable<T>(string whereCondition, int page = 1, int size = 0, params Type[] fromAddition)
        {
            List<T> result = new List<T>();
            if (page < 1 || size < 0)
                return result;
            Type type = typeof(T);
            TableAttribute tableAttr = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));
            if (tableAttr == null)
                return result;
            SqlCommand cmd = conn.CreateCommand();
            if (whereCondition.Length != 0)
                whereCondition = $"WHERE {whereCondition}";
            cmd.CommandText = $"SELECT {tableAttr.Name}.* FROM {tableAttr.Name}{FromAddition(fromAddition)} {whereCondition} ORDER BY (SELECT NULL) OFFSET {(page - 1) * size} ROWS";
            if (size > 0)
                cmd.CommandText += $" FETCH NEXT {size} ROWS ONLY";
            cmd.CommandType = CommandType.Text;
            PropertyInfo[] properties = type.GetProperties();
            Debug.WriteLine(cmd.CommandText);
            try
            {
                Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T curr = (T)Activator.CreateInstance(type);
                        foreach (PropertyInfo property in properties)
                        {
                            ColumnAttribute columnAttr = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                            if (columnAttr == null) continue;
                            string column = columnAttr.Name;
                            object pValue = reader[column];
                            if (pValue.ToString().Length == 0)
                                continue;
                            property.SetValue(curr, pValue, null);
                        }
                        result.Add(curr);
                    }
                }
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Enumerable.Empty<T>();
            }
            return result;
        }

        public DataTable GetTable(string query)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        public object ExecuteScalar(string query)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandType = CommandType.Text;
            try
            {
                Open();
                object data = cmd.ExecuteScalar();
                Close();
                return data;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public int ExecuteNonQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandType = CommandType.Text;
            try
            {
                Open();
                int data = cmd.ExecuteNonQuery();
                Close();
                return data;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return 0;
            }
        }

        public T AddRow<T>(T data)
        {
            Type type = typeof(T);
            T result = (T)Activator.CreateInstance(type);
            if (data == null)
                return result;
            TableAttribute tableAttr = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));
            if (tableAttr == null)
                return result;
            List<string> keys = new List<string>();
            List<object> values = new List<object>();

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                ColumnAttribute columnAttr = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                if (columnAttr == null || columnAttr.IsIdentity)
                    continue;
                object value = property.GetValue(data, null);
                if (value == null || value.ToString().Length == 0)
                    continue;
                Type pType = property.PropertyType;
                if (pType == typeof(int))
                    values.Add(value);
                else if (pType == typeof(DateTime))
                {
                    DateTime dt = (DateTime)value;
                    if (dt.Year == 1)
                        continue;
                    else
                        values.Add($"'{(DateTime)value:yyyy-MM-dd HH:mm:ss}'");
                }
                else if (pType == typeof(bool))
                {
                    if ((bool)value)
                        values.Add(1);
                    else
                        values.Add(0);
                }
                else if (pType == typeof(decimal) || pType == typeof(float) || pType == typeof(double))
                    values.Add(((decimal)value).ToString(System.Globalization.CultureInfo.InvariantCulture));
                else
                    values.Add($"N'{value}'");
                keys.Add(columnAttr.Name);
            }
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $"INSERT INTO {tableAttr.Name} ({string.Join(", ", keys)}) OUTPUT inserted.* VALUES ({string.Join(", ", values)})";
            Debug.WriteLine(cmd.CommandText);
            try
            {
                Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreach (PropertyInfo property in properties)
                        {
                            ColumnAttribute columnAttr = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                            if (columnAttr == null) continue;
                            string column = columnAttr.Name;
                            object pValue = reader[column];
                            if (pValue.ToString().Length == 0)
                                continue;
                            property.SetValue(result, pValue, null);
                        }
                    }
                }
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }

        public bool UpdateRow<T>(T data)
        {
            bool result = false;
            if (data == null)
                return result;
            Type type = typeof(T);
            TableAttribute tableAttr = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));
            if (tableAttr == null)
                return result;

            List<string> sets = new List<string>();
            List<string> where = new List<string>();

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                ColumnAttribute columnAttr = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                if (columnAttr == null) continue;
                object value = property.GetValue(data, null);
                Type pType = property.PropertyType;
                object fValue = null;
                if (columnAttr.IsPrimaryKey)
                {
                    if (pType == typeof(int))
                        fValue = value;
                    else if (pType == typeof(DateTime))
                    {
                        DateTime dt = (DateTime)value;
                        if (dt == new DateTime())
                            continue;
                        else
                            fValue = $"'{(DateTime)value:yyyy-MM-dd HH:mm:ss}'";
                    }
                    else if (pType == typeof(bool))
                        fValue = (bool)value ? 1 : 0;
                    else if (pType == typeof(decimal) || pType == typeof(float) || pType == typeof(double))
                        fValue = ((decimal)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    else
                        fValue = $"N'{value}'";
                    where.Add($"{columnAttr.Name} = {fValue}");
                }
                else
                {
                    if (pType == typeof(int))
                        fValue = value;
                    else if (pType == typeof(DateTime))
                    {
                        DateTime dt = (DateTime)value;
                        if (dt.Year == 1)
                            continue;
                        else
                            fValue = $"'{(DateTime)value:yyyy-MM-dd HH:mm:ss}'";
                    }
                    else if (pType == typeof(bool))
                        fValue = (bool)value ? 1 : 0;
                    else if (pType == typeof(decimal) || pType == typeof(float) || pType == typeof(double))
                        fValue = ((decimal)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    else
                        fValue = $"N'{value}'";
                    sets.Add($"{columnAttr.Name} = {fValue}");
                }
            }
            if (where.Count == 0)
                return result;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $"UPDATE {tableAttr.Name} SET {string.Join(", ", sets)} WHERE {string.Join(" AND ", where)}";
            Debug.WriteLine(cmd.CommandText);
            try
            {
                Open();
                if (cmd.ExecuteNonQuery() != 0)
                    result = true;
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }

        public bool DeleteRow<T>(T obj)
        {
            if (obj == null)
                return false;
            Type type = typeof(T);
            TableAttribute tableAttr = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));
            if (tableAttr == null) return false;
            List<string> where = new List<string>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                ColumnAttribute columnAttr = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                if (columnAttr == null) continue;
                object value = property.GetValue(obj, null);
                Type pType = property.PropertyType;
                object fValue = null;
                if (columnAttr.IsPrimaryKey)
                {
                    if (pType == typeof(int) || pType == typeof(float) || pType == typeof(double))
                        fValue = value;
                    else if (pType == typeof(DateTime))
                    {
                        DateTime dt = (DateTime)value;
                        if (dt.Year == 1)
                            continue;
                        else
                            fValue = $"'{(DateTime)value:yyyy-MM-dd HH:mm:ss}'";
                    }
                    else if (pType == typeof(bool))
                        fValue = (bool)value ? 1 : 0;
                    else
                        fValue = $"N'{value}'";
                    where.Add($"{columnAttr.Name} = {fValue}");
                }
            }
            if (where.Count == 0) return false;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $"DELETE FROM {tableAttr.Name} WHERE {string.Join(" AND ", where)}";
            Debug.WriteLine(cmd.CommandText);
            try
            {
                Open();
                int result = cmd.ExecuteNonQuery();
                Close();
                return result != 0 ? true : false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public int DeleteRows<T>(string whereCondition)
        {
            int result = 0;
            Type type = typeof(T);
            TableAttribute tableAttr = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));
            if (tableAttr == null) return result;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            if (whereCondition.Length == 0)
                return result;
            else
                whereCondition = $" WHERE {whereCondition}";
            cmd.CommandText = $"DELETE FROM {tableAttr.Name}{whereCondition}";
            Debug.WriteLine(cmd.CommandText);
            try
            {
                Open();
                result = cmd.ExecuteNonQuery();
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }

        public static bool IsValidConnStr(string connStr)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}