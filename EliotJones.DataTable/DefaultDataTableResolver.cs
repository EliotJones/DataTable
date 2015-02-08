﻿namespace EliotJones.DataTable
{
    using EliotJones.DataTable.DataTypeConverter;
    using EliotJones.DataTable.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class DefaultDataTableResolver : IDataTableResolver
    {
        public virtual IList<T> ToObjects<T>(DataTable dataTable, IDataTypeConverter dataTypeConverter, IEnumerable<ExtendedPropertyInfo> mappings, DataTableParserSettings settings) where T : new()
        {
            Guard.ArgumentNotNull(dataTable);
            Guard.ArgumentNotNull(dataTypeConverter);
            Guard.ArgumentNotNull(mappings);
            Guard.ArgumentNotNull(settings);

            VerifyMappingIndexIntegrity<T>(dataTable.Columns, ref mappings);

            List<T> objectList = new List<T>(capacity: dataTable.Rows.Count);

            for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                T returnObject = new T();

                foreach (var mapping in mappings)
                {
                    object value = dataTypeConverter.FieldToObject(dataTable.Rows[rowIndex][mapping.ColumnIndex], mapping.PropertyInfo.PropertyType, settings);
                    mapping.PropertyInfo.SetValue(returnObject, value);
                }

                objectList.Add(returnObject);
            }

            return objectList;
        }

        protected virtual void VerifyMappingIndexIntegrity<T>(DataColumnCollection columns, ref IEnumerable<ExtendedPropertyInfo> mappings) where T : new()
        {
            int columnsCount = columns.Count;

            foreach (var mapping in mappings)
            {
                if (mapping == null)
                {
                    throw new InvalidMappingException<T>("Null mapping.");
                }

                if (mapping.ColumnIndex < 0 || mapping.ColumnIndex >= columnsCount)
                {
                    if (columns.Contains(mapping.FieldName))
                    {
                        mapping.ColumnIndex = columns.IndexOf(mapping.FieldName);
                    }
                    else
                    {
                        throw new InvalidMappingException<T>("Incorrectly mapped Field: " + mapping.FieldName);
                    }
                }
            }
        }
    }
}
