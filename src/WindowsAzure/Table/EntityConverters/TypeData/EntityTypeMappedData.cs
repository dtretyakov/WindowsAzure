using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WindowsAzure.Properties;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Maps an entity type data.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public sealed class EntityTypeMappedData<T> : IEntityTypeData<T> where T : class, new()
    {
        private const BindingFlags AutoMapFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        private const BindingFlags PropertyMapFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string PartitionKey = "PartitionKey";
        private const string RowKey = "RowKey";

        private readonly Dictionary<string, string> _nameChanges = new Dictionary<string, string>();
        private readonly Dictionary<string, IProperty<T>> _properties = new Dictionary<string, IProperty<T>> { };

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal EntityTypeMappedData()
        {
            Type entityType = typeof(T);

            // Retrieve class members
            var members = new List<MemberInfo>(entityType.GetFields(PropertyMapFlags));
            members.AddRange(entityType.GetProperties(PropertyMapFlags).Where(p => p.CanRead && p.CanWrite));

            // Create properties for entity members
            var properties = members.Select(member => new { Key = member.Name, Value = (IProperty<T>)new RegularProperty<T>(member) })
                                    .Where(result => result != null && result.Value != null);

            // Check whether entity's composite key completely defined
            if (!_nameChanges.ContainsValue(PartitionKey) && !_nameChanges.ContainsValue(RowKey))
            {
                string message = string.Format(Resources.EntityTypeDataMissingKey, entityType);
                throw new InvalidOperationException(message);
            }

            _properties = properties.ToDictionary(k => k.Key, e => e.Value);
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="classMapInitializer">The class map initializer.</param>
        public EntityTypeMappedData(Action<EntityTypeMappedData<T>> classMapInitializer)
        {
            classMapInitializer(this);
        }

        /// <summary>
        ///     Converts <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> into POCO.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <returns>POCO.</returns>
        public T GetEntity(DynamicTableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException("tableEntity");
            }

            var result = new T();

            foreach (var prop in _properties)
            {
                prop.Value.SetMemberValue(tableEntity, result);
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
                throw new ArgumentNullException("entity");
            }

            var result = new DynamicTableEntity(string.Empty, string.Empty) { ETag = "*" };

            foreach (var prop in _properties)
            {
                prop.Value.GetMemberValue(entity, result);
            }

            return result;
        }

        /// <summary>
        ///     Gets a name changes for entity members.
        /// </summary>
        public IDictionary<string, string> NameChanges
        {
            get { return _nameChanges; }
        }

        /// <summary>
        ///     Maps a property and overrides the property name.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMappedData<T> MapPropertyName<TMember>(string propertyName, Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _nameChanges.Add(member.Name, propertyName);
            _properties[member.Name] = new RegularProperty<T>(member, propertyName);
            return this;
        }

        /// <summary>
        ///     Maps a row key property.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMappedData<T> MapRowKey<TMember>(Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _nameChanges.Add(member.Name, RowKey);
            _properties[member.Name] = new RowKeyProperty<T>(member);
            return this;
        }

        /// <summary>
        ///     Maps a partition key property.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMappedData<T> MapPartitionKey<TMember>(Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _nameChanges.Add(member.Name, PartitionKey);
            _properties[member.Name] = new PartitionKeyProperty<T>(member);
            return this;
        }

        /// <summary>
        ///     Maps a etag property.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMappedData<T> MapETag<TMember>(Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _properties[member.Name] = new ETagProperty<T>(member);
            return this;
        }

        /// <summary>
        ///     Maps a timestamp property.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMappedData<T> MapTimestamp<TMember>(Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _properties[member.Name] = new TimestampProperty<T>(member);
            return this;
        }

        /// <summary>
        ///     Converts a member expression to a member metadata instance.
        /// </summary>
        /// <param name="memberLambda">Lambda expression for a class member.</param>
        /// <returns>Member metadata.</returns>
        private static MemberInfo GetMemberInfoFromLambda<TMember>(Expression<Func<T, TMember>> memberLambda)
        {
            var body = memberLambda.Body;
            MemberExpression memberExpression;
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    memberExpression = (MemberExpression)body;
                    break;
                case ExpressionType.Convert:
                    var convertExpression = (UnaryExpression)body;
                    memberExpression = (MemberExpression)convertExpression.Operand;
                    break;
                default:
                    throw new ArgumentException("Invalid lambda expression");
            }
            var memberInfo = memberExpression.Member;
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    break;
                case MemberTypes.Property:
                    if (memberInfo.DeclaringType.IsInterface)
                    {
                        memberInfo = FindPropertyImplementation((PropertyInfo)memberInfo, typeof(T));
                    }
                    break;
                default:
                    memberInfo = null;
                    break;
            }
            if (memberInfo == null)
            {
                throw new ArgumentException("Invalid lambda expression");
            }
            return memberInfo;
        }

        /// <summary>
        ///     Finds the property implementation on the type.
        /// </summary>
        /// <param name="interfacePropertyInfo">Property metadata instance.</param>
        /// <param name="actualType">Class type</param>
        /// <returns>Property metadata instance.</returns>
        private static PropertyInfo FindPropertyImplementation(PropertyInfo interfacePropertyInfo, Type actualType)
        {
            var interfaceType = interfacePropertyInfo.DeclaringType;

            // An interface map must be used because because there is no
            // other officially documented way to derive the explicitly
            // implemented property name.
            var interfaceMap = actualType.GetInterfaceMap(interfaceType);

            var interfacePropertyAccessors = interfacePropertyInfo.GetAccessors(true);

            var actualPropertyAccessors = interfacePropertyAccessors.Select(interfacePropertyAccessor =>
            {
                var index = Array.IndexOf<MethodInfo>(interfaceMap.InterfaceMethods, interfacePropertyAccessor);

                return interfaceMap.TargetMethods[index];
            });

            // Binding must be done by accessor methods because interface
            // maps only map accessor methods and do not map properties.
            return actualType.GetProperties(PropertyMapFlags)
                             .Single(propertyInfo =>
                             {
                                 // we are looking for a property that implements all the required accessors
                                 var propertyAccessors = propertyInfo.GetAccessors(true);
                                 return actualPropertyAccessors.All(x => propertyAccessors.Contains(x));
                             });
        }
    }
}