using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WindowsAzure.Properties;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Common;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Keeps an entity type data.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class EntityTypeData<T> : IEntityTypeData<T> where T : class, new()
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        private const string PartitionKey = "PartitionKey";
        private const string RowKey = "RowKey";

        private static readonly Dictionary<Type, Func<MemberInfo, object, IDictionary<string, string>, IProperty<T>>> PropertyFactories =
            new Dictionary<Type, Func<MemberInfo, object, IDictionary<string, string>, IProperty<T>>>
            {
                {typeof (ETagAttribute), CreateETagProperty},
                {typeof (IgnoreAttribute), (member, attribute, names) => null},
                {typeof (PartitionKeyAttribute), CreatePartitionKeyProperty},
                {typeof (PropertyAttribute), CreateNamedProperty},
                {typeof (RowKeyAttribute), CreateRowKeyProperty},
                {typeof (TimestampAttribute), CreateTimestampProperty},
                {typeof (SerializeAttribute), CreateSerializableProperty},
            };

        private readonly Dictionary<string, string> _nameChanges = new Dictionary<string, string>();
        private readonly IProperty<T>[] _properties;

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal EntityTypeData()
        {
            var entityType = typeof (T);
            // Retrieve class members
            var members = new List<MemberInfo>(entityType.GetFields(Flags));
            members.AddRange(entityType.GetProperties(Flags).Where(p => p.CanRead && p.CanWrite));

            // Create properties for entity members
            var properties = new List<IProperty<T>>(
                members.Select(member => GetMemberProperty(member, _nameChanges)).Where(result => result != null));

            // Check whether entity's composite key completely defined
            if (!_nameChanges.ContainsValue(PartitionKey) && !_nameChanges.ContainsValue(RowKey))
            {
                var message = string.Format(Resources.EntityTypeDataMissingKey, entityType);
                throw new InvalidOperationException(message);
            }

            _properties = properties.ToArray();
        }        

        /// <summary>
        ///     Gets a name changes for entity members.
        /// </summary>
        public IDictionary<string, string> NameChanges => _nameChanges;

        /// <summary>
        ///     Creates a new timestamp property.
        /// </summary>
        /// <param name="member">Entity member.</param>
        /// <param name="attribute">Timestamp attribute.</param>
        /// <param name="nameChanges">Name changes.</param>
        /// <returns>Timestamp property.</returns>
        private static TimestampProperty<T> CreateTimestampProperty(MemberInfo member, object attribute, IDictionary<string, string> nameChanges)
        {
            return new TimestampProperty<T>(member);
        }

        private static SerializableProperty<T> CreateSerializableProperty(MemberInfo member, object attribute, IDictionary<string, string> nameChanges)
        {
            if (!(attribute is SerializeAttribute serializableAttribute)) 
            {
                throw new ArgumentException(nameof(attribute));
            };

            var propertyName = serializableAttribute?.Name;

            if (string.IsNullOrEmpty(propertyName))
            {
                propertyName = member.Name;
            }
            else
            {
                nameChanges.Add(member.Name, propertyName);
            }

            return new SerializableProperty<T>(member, propertyName);
        }

        /// <summary>
        ///     Creates a new row key property.
        /// </summary>
        /// <param name="member">Entity member.</param>
        /// <param name="attribute">Row key attribute.</param>
        /// <param name="nameChanges">Name changes.</param>
        /// <returns>Row key property.</returns>
        private static RowKeyProperty<T> CreateRowKeyProperty(MemberInfo member, object attribute, IDictionary<string, string> nameChanges)
        {
            nameChanges.Add(member.Name, RowKey);
            return new RowKeyProperty<T>(member);
        }

        /// <summary>
        ///     Creates a new named property.
        /// </summary>
        /// <param name="member">Entity member.</param>
        /// <param name="attribute">Property attribute.</param>
        /// <param name="nameChanges">Name changes.</param>
        /// <returns>Property.</returns>
        private static RegularProperty<T> CreateNamedProperty(MemberInfo member, object attribute, IDictionary<string, string> nameChanges)
        {
            var propertyName = ((PropertyAttribute) attribute).Name;
            nameChanges.Add(member.Name, propertyName);
            return new RegularProperty<T>(member, propertyName);
        }

        /// <summary>
        ///     Creates a new partition key property.
        /// </summary>
        /// <param name="member">Entity member.</param>
        /// <param name="attribute">Partition key attribute.</param>
        /// <param name="nameChanges">Name changes.</param>
        /// <returns>Partition key property.</returns>
        private static PartitionKeyProperty<T> CreatePartitionKeyProperty(MemberInfo member, object attribute, IDictionary<string, string> nameChanges)
        {
            nameChanges.Add(member.Name, PartitionKey);
            return new PartitionKeyProperty<T>(member);
        }

        /// <summary>
        ///     Creates a new etag property.
        /// </summary>
        /// <param name="member">Entity member.</param>
        /// <param name="attribute">ETag attribute.</param>
        /// <param name="nameChanges">Name changes.</param>
        /// <returns>ETag property.</returns>
        private static ETagProperty<T> CreateETagProperty(MemberInfo member, object attribute, IDictionary<string, string> nameChanges)
        {
            return new ETagProperty<T>(member);
        }

        /// <summary>
        ///     Creates a new entity member property.
        /// </summary>
        /// <param name="member">Entity member.</param>
        /// <param name="nameChanges">Name changes.</param>
        /// <returns>Property.</returns>
        private static IProperty<T> GetMemberProperty(MemberInfo member, IDictionary<string, string> nameChanges)
        {
            var customAttributes = member.GetCustomAttributes(false).ToArray();
            var attributes = SelectMetadataAttributes(customAttributes);
            
            if (attributes.Count == 0)
            {
                if (SerializationSettings.Instance.SerializeComplexTypes && !member.GetMemberType().IsSupportedEntityPropertyType())
                {
                    return new SerializableProperty<T>(member);
                }

                return new RegularProperty<T>(member);
            }           

            if (attributes.Count == 1)
            {
                return PropertyFactories[attributes[0].GetType()](member, attributes[0], nameChanges);
            }

            var typeName = member.DeclaringType != null ? member.DeclaringType.Name : string.Empty;
            var message = string.Format(Resources.EntityTypeDataShouldBeOneAttribute, member.Name, typeName);

            throw new InvalidOperationException(message);
        }

        /// <summary>
        ///     Selects a metadata attributes.
        /// </summary>
        /// <param name="attributes">Entity attributes.</param>
        /// <returns>Metadata attributes.</returns>
        private static IList<object> SelectMetadataAttributes(object[] attributes)
        {
            var selectedAttributes = new List<object>();

            for (var i = 0; i < attributes.Length; i++)
            {
                var attributeType = attributes[i].GetType();

                if (!PropertyFactories.ContainsKey(attributeType))
                {
                    continue;
                }

                selectedAttributes.Add(attributes[i]);
            }

            return selectedAttributes;
        }

        // ReSharper disable ForCanBeConvertedToForeach

        /// <summary>
        ///     Converts <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> into POCO.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <returns>POCO.</returns>
        public T GetEntity(DynamicTableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException(nameof(tableEntity));
            }

            var result = new T();

            for (var i = 0; i < _properties.Length; i++)
            {
                _properties[i].SetMemberValue(tableEntity, result);
            }

            return result;
        }

        /// <summary>
        ///     Converts POCO into <see cref="T:Microsoft.WindowsAzure.Storage.Table.ITableEntity" />.
        /// </summary>
        /// <param name="entity">POCO entity.</param>
        /// <returns>Table entity.</returns>
        public ITableEntity GetEntity(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = new DynamicTableEntity(string.Empty, string.Empty) {ETag = "*"};

            for (var i = 0; i < _properties.Length; i++)
            {
                _properties[i].GetMemberValue(entity, result);
            }

            return result;
        }

        // ReSharper restore ForCanBeConvertedToForeach
    }
}