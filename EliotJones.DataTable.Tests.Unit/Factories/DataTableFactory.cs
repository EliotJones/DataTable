﻿namespace EliotJones.DataTable.Tests.Unit.Factories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public static class DataTableFactory
    {
        public static DataTable GenerateEmptyDataTableWithStringColumns(params string[] columnNames)
        {
            if (columnNames == null) columnNames = new string[] { null };

            DataTable dt = new DataTable("Test Table");

            dt.Columns.AddRange(columnNames.Select(name => new DataColumn(name, typeof(string))).ToArray());

            return dt;
        }

        public static DataTable GenerateEmptyDataTableMatchingObjectProperties<T>()
        {
            DataTable dt = new DataTable("Test Table");

            foreach (var p in typeof(T).GetProperties())
            {
                dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
            }

            return dt;
        }

        public static DataTable GenerateDataTableFilledWithObjects<T>(IEnumerable<T> objects)
        {
            DataTable dt = GenerateEmptyDataTableMatchingObjectProperties<T>();

            foreach (var obj in objects)
            {
                DataRow dr = dt.NewRow();

                foreach (var p in typeof(T).GetProperties())
                {
                    dr[p.Name] = p.GetValue(obj);
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}