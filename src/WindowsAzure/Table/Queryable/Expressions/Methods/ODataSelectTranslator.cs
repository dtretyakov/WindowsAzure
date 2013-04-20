using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Select expression translator.
    /// </summary>
    public sealed class ODataSelectTranslator : ExpressionVisitor, IMethodTranslator
    {
        private static readonly List<String> SupportedMethods = new List<string> {"Select"};
        private readonly IDictionary<string, string> _nameChanges;
        private ITranslationResult _result;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="nameChanges">Entity properties name changes.</param>
        public ODataSelectTranslator(IDictionary<string, string> nameChanges)
        {
            _nameChanges = nameChanges;
        }

        public void Translate(ITranslationResult result, MethodCallExpression method)
        {
            _result = result;

            Visit(method);

            AddPostProcessing(method);
        }

        private void AddPostProcessing(MethodCallExpression method)
        {
            Type type = method.Arguments[0].Type.GetGenericArguments()[0];
            ParameterExpression parameter = Expression.Parameter(typeof(IQueryable<>).MakeGenericType(type), null);
            var call = Expression.Call(method.Method, parameter, method.Arguments[1]);

            _result.AddPostProcesing(Expression.Lambda(call, parameter));
        }

        public IList<string> AcceptedMethods
        {
            get { return SupportedMethods; }
        }

        protected override Expression VisitMember(MemberExpression member)
        {
            if (member.Expression != null && member.Expression.NodeType == ExpressionType.Parameter)
            {
                AppendColumnName(member.Member.Name);
            }

            return base.VisitMember(member);
        }

        private void AppendColumnName(string name)
        {
            _result.AddColumn(_nameChanges.ContainsKey(name) ? _nameChanges[name] : name);
        }
    }
}