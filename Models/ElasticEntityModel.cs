/*
 *  Copyright © 2016, Russell Libby 
 */

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace AzureStorage.Models
{
    /// <summary>
    /// Class for handling dynamic azure table data.
    /// </summary>
    public class ElasticEntityModel : DynamicObject, ITableEntity
    {
        #region Private methods

        /// <summary>
        /// Gets the entity property.
        /// </summary>
        /// <param name="key">The name of the entity property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns></returns>
        private EntityProperty GetEntityProperty(string key, object value)
        {
            if (value == null) return new EntityProperty((string)null);
            if (value.GetType() == typeof(byte[])) return EntityProperty.GeneratePropertyForByteArray((byte[])value);
            if (value.GetType() == typeof(bool)) return EntityProperty.GeneratePropertyForBool((bool)value);
            if (value.GetType() == typeof(DateTimeOffset)) return EntityProperty.GeneratePropertyForDateTimeOffset((DateTimeOffset)value);
            if (value.GetType() == typeof(DateTime)) return EntityProperty.GeneratePropertyForDateTimeOffset((DateTime)value);
            if (value.GetType() == typeof(double)) return EntityProperty.GeneratePropertyForDouble((double)value);
            if (value.GetType() == typeof(Guid)) return EntityProperty.GeneratePropertyForGuid((Guid)value);
            if (value.GetType() == typeof(int)) return EntityProperty.GeneratePropertyForInt((int)value);
            if (value.GetType() == typeof(long)) return EntityProperty.GeneratePropertyForLong((long)value);
            if (value.GetType() == typeof(string)) return EntityProperty.GeneratePropertyForString((string)value);

            throw new Exception("not supported " + value.GetType() + " for " + key);
        }

        /// <summary>
        /// Gets the C# type for the edm type. 
        /// </summary>
        /// <param name="edmType">The edm type to get the C# type for.</param>
        /// <returns>The Type.</returns>
        private Type GetType(EdmType edmType)
        {
            switch (edmType)
            {
                case EdmType.Binary:
                    return typeof(byte[]);

                case EdmType.Boolean:
                    return typeof(bool);

                case EdmType.DateTime:
                    return typeof(DateTime);

                case EdmType.Double:
                    return typeof(double);

                case EdmType.Guid:
                    return typeof(Guid);

                case EdmType.Int32:
                    return typeof(int);

                case EdmType.Int64:
                    return typeof(long);

                case EdmType.String:
                    return typeof(string);
            }

            throw new Exception("not supported " + edmType);
        }

        /// <summary>
        /// Gets the value for the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The property value.</returns>
        private object GetValue(EntityProperty property)
        {
            switch (property.PropertyType)
            {
                case EdmType.Binary:
                    return property.BinaryValue;

                case EdmType.Boolean:
                    return property.BooleanValue;

                case EdmType.DateTime:
                    return property.DateTimeOffsetValue;

                case EdmType.Double:
                    return property.DoubleValue;

                case EdmType.Guid:
                    return property.GuidValue;

                case EdmType.Int32:
                    return property.Int32Value;

                case EdmType.Int64:
                    return property.Int64Value;

                case EdmType.String:
                    return property.StringValue;
            }

            throw new Exception("not supported " + property.PropertyType);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ElasticEntityModel()
        {
            Properties = new Dictionary<string, EntityProperty>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the properties from the collection.
        /// </summary>
        /// <param name="properties">The collection from deserialization.</param>
        /// <param name="operationContext">The context for the azure opertation.</param>
        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Properties = properties;
        }

        /// <summary>
        /// Gets the properties for the entity.
        /// </summary>
        /// <param name="operationContext">The context for the azure opertation.</param>
        /// <returns>The dictionaary of properties.</returns>
        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return Properties;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The partition key.
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// The row key.
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        /// The timestamp for the row.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The etag for the row.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// The dictionary containing the properties.
        /// </summary>
        public IDictionary<string, EntityProperty> Properties { get; set; }

        #endregion
    }
}
