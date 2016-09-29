using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WindowsAzure.Properties;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Maps an entity type data.
    /// </summary>
    public class EntityTypeMap
    {
        /// <summary>
        ///     Performs registration of assemblies with type maps.
        /// </summary>
        /// <param name="assemblies">List of assemblies.</param>
        public static void RegisterAssembly(params Assembly[] assemblies)
        {
            EntityTypeDataFactory.RegisterMappingAssembly(assemblies);
        }

        /// <summary>
        ///     Auto map all properties
        /// </summary>
        internal virtual void AutoMap()
        { }
    }

    /// <summary>
    ///     Maps an entity type data.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class EntityTypeMap<T> : EntityTypeMap, IEntityTypeData<T> where T : class, new()
    {
        private const BindingFlags PropertyMapFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string PartitionKeyPropertyName = "PartitionKey";
        private const string RowKeyPropertyName = "RowKey";
        private readonly Type _entityType = typeof(T);
        private readonly Dictionary<string, string> _nameChanges = new Dictionary<string, string>();
        private readonly IDictionary<string, IProperty<T>> _properties = new Dictionary<string, IProperty<T>>();
        private readonly HashSet<string> _propertiesToIgnore = new HashSet<string>();

        /// <summary>
        ///     Constructor.
        /// </summary>
        public EntityTypeMap()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="classMapInitializer">The class map initializer.</param>
        public EntityTypeMap(Action<EntityTypeMap<T>> classMapInitializer)
        {
            classMapInitializer(this);
            AutoMap();
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
        /// Auto map all properties that can be read and write
        /// </summary>
        internal override void AutoMap()
        {
            // Retrieve class members
            var members = _entityType.GetProperties(PropertyMapFlags).Where(p => p.CanRead && p.CanWrite);

            // Create properties for entity members
            foreach (var member in members)
            {
                var name = member.Name;
                if (_propertiesToIgnore.Contains(name) == false
                    && _properties.ContainsKey(name) == false)
                {
                    _properties.Add(member.Name, new RegularProperty<T>(member));
                }
            }

            // Check whether entity's composite key completely defined
            if (!_nameChanges.ContainsValue(PartitionKeyPropertyName) 
                && !_nameChanges.ContainsValue(RowKeyPropertyName))
            {
                var message = string.Format(Resources.EntityTypeDataMissingKey, _entityType);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        ///     Maps a property and overrides the property name.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMap<T> PropertyName<TMember>(string propertyName, Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _nameChanges.Add(member.Name, propertyName);
            _properties[member.Name] = new RegularProperty<T>(member, propertyName);
            return this;
        }

        /// <summary>
        ///     Ignores a property map.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMap<T> Ignore<TMember>(Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _nameChanges.Remove(member.Name);
            _properties.Remove(member.Name);
            _propertiesToIgnore.Add(member.Name);
            return this;
        }

        /// <summary>
        ///     Maps a row key property.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMap<T> RowKey<TMember>(Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _nameChanges.Add(member.Name, RowKeyPropertyName);
            _properties[member.Name] = new RowKeyProperty<T>(member);
            return this;
        }

        /// <summary>
        ///     Maps a partition key property.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMap<T> PartitionKey<TMember>(Expression<Func<T, TMember>> propertyLambda)
        {
            var member = GetMemberInfoFromLambda(propertyLambda);
            _nameChanges.Add(member.Name, PartitionKeyPropertyName);
            _properties[member.Name] = new PartitionKeyProperty<T>(member);
            return this;
        }

        /// <summary>
        ///     Maps a etag property.
        /// </summary>
        /// <typeparam name="TMember">Entity member.</typeparam>
        /// <param name="propertyLambda">Property lambda expression.</param>
        /// <returns>Current instance of <see cref="T:WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap" />.</returns>
        public EntityTypeMap<T> ETag<TMember>(Expression<Func<T, TMember>> propertyLambda)
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
        public EntityTypeMap<T> Timestamp<TMember>(Expression<Func<T, TMember>> propertyLambda)
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
                    memberExpression = (MemberExpression) body;
                    break;
                case ExpressionType.Convert:
                    var convertExpression = (UnaryExpression) body;
                    memberExpression = (MemberExpression) convertExpression.Operand;
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
                    if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.IsInterface)
                    {
                        memberInfo = FindPropertyImplementation((PropertyInfo) memberInfo, typeof (T));
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
            if (interfaceType == null)
            {
                return null;
            }

            // An interface map must be used because because there is no
            // other officially documented way to derive the explicitly
            // implemented property name.
            var interfaceMap = actualType.GetInterfaceMap(interfaceType);

            var interfacePropertyAccessors = interfacePropertyInfo.GetAccessors(true);

            var actualPropertyAccessors = interfacePropertyAccessors.Select(interfacePropertyAccessor =>
            {
                var index = Array.IndexOf(interfaceMap.InterfaceMethods, interfacePropertyAccessor);

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