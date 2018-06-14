using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EmailMaker.WebsiteCore
{
	public static class WindsorRegistrationExtensions
	{
		// method taken from https://github.com/fir3pho3nixx/Windsor/blob/aspnet-core-windsor-final/src/Castle.Facilities.AspNetCore/WindsorRegistrationExtensions.cs
        // this version allows to pass parameters into the middleware
		public static void UseMiddlewareFromWindsor<T>(this IApplicationBuilder app, IWindsorContainer container, object argumentsAsAnonymousType)
			where T : class, IMiddleware
		{
			container.Register(Component.For<T>());
			app.Use(async (context, next) =>
			{
				var resolve = container.Resolve<T>(argumentsAsAnonymousType);
				try
				{
					await resolve.InvokeAsync(context, async (ctx) => await next());
				}
				finally
				{
					container.Release(resolve);
				}
			});
		}
	}
}