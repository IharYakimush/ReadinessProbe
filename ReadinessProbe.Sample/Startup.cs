﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ReadinessProbe.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // registed readiness probe services
            services.AddReadinessProbe();

            // registed long running action as hosted service.
            services.AddHostedService<LongRunningAction>();

            // registed rediness indicator which will be used by LongRunningAction for completion notice.
            services.AddReadinessIndicatorFor<LongRunningAction>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseReadinessProbe();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
