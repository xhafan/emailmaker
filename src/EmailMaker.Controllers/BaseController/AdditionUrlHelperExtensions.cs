using System;
using System.Linq.Expressions;
#if NETCOREAPP
using Microsoft.AspNetCore.Routing;
#endif
#if NETFRAMEWORK 
using System.Web.Routing;
#endif

namespace EmailMaker.Controllers.BaseController
{
    //https://stackoverflow.com/a/44187330/379279
    //a few helper methods
    public static class AdditionUrlHelperExtensions
    {
        public static string GetControllerName(this Type controllerType)
        {
            var controllerName = controllerType.Name.Replace("Controller", string.Empty);
            return controllerName;
        }

        public static string GetActionName<TController>(this Expression<Func<TController, object>> actionExpression)
        {
            var actionName = ((MethodCallExpression)actionExpression.Body).Method.Name;

            return actionName;
        }

        public static RouteValueDictionary GetRouteValues<TController>(this Expression<Func<TController, object>> actionExpression)
        {
            var result = new RouteValueDictionary();
            var expressionBody = (MethodCallExpression)actionExpression.Body;

            var parameters = expressionBody.Method.GetParameters();

            //expression tree cannot represent a call with optional params
            //so our method param count and should match the expression body arg count
            //but just the same, let's check...
            if (parameters.Length != expressionBody.Arguments.Count)
                throw new InvalidOperationException("Mismatched parameter/argument count");

            for (var i = 0; i < expressionBody.Arguments.Count; ++i)
            {
                var parameter = parameters[i];
                var argument = expressionBody.Arguments[i];

                var parameterName = parameter.Name;
                var argumentValue = argument.GetValue();

                result.Add(parameterName, argumentValue);
            }

            return result;
        }

        private static object GetValue(this Expression exp)
        {
            var objectMember = Expression.Convert(exp, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();

            return getter();
        }
    }
}