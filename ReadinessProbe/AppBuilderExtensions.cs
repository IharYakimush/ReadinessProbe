﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace ReadinessProbe
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseReadinessProbe(this IApplicationBuilder app, PathString endpointPath)
        {
            if (app == null)
            {
                throw new System.ArgumentNullException(nameof(app));
            }            

            return app.Map(endpointPath, builder => builder.UseMiddleware<ReadinessProbeMiddleware>());
        }

        public static IApplicationBuilder UseReadinessProbe(this IApplicationBuilder app)
        {
            return app.UseReadinessProbe(new PathString("/readinessProbe"));
        }

        public static IServiceCollection AddReadinessProbe(this IServiceCollection services, Action<ReadinessProbeOptions> builder = null)
        {
            ReadinessProbeOptions options = new ReadinessProbeOptions();

            builder?.Invoke(options);

            services.TryAddSingleton<IOptions<ReadinessProbeOptions>>(options);

            if (!services.Any(d => d.ServiceType == typeof(IReadinessCheck)))
            {
                services.AddSingleton<IReadinessCheck>(DefaultReadinessCheck.Instance);
            }

            services.TryAddSingleton<ReadinessProbeMiddleware>();

            return services;
        }
    }
}