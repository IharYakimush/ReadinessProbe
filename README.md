# ReadinessProbe
Adds readiness probe endpoint which can be used by kubernetes. Allows to perform time consuming operations after startup and indicate that app not ready to receive traffic during this time.
# Sample
Considering the case when readines endpoint should return 200 status code only after finishing some long running action. 
```
	public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // registed readiness probe services
            services.AddReadinessProbe();

            // registed long running action as hosted service.
            services.AddHostedService<LongRunningAction>();

            // registed rediness indicator which will be used by LongRunningAction for completion notice.
			// multiple indicators can be registered if you want to wait for multiple actions
            services.AddReadinessIndicatorFor<LongRunningAction>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			// readiness probe endpoint
            app.UseReadinessProbe();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
```
```
	public class LongRunningAction : BackgroundService
    {
        public LongRunningAction(ReadinessIndicatorFor<LongRunningAction> readinessIndicator)
        {
            ReadinessIndicator = readinessIndicator ?? throw new ArgumentNullException(nameof(readinessIndicator));
        }

        public ReadinessIndicatorFor<LongRunningAction> ReadinessIndicator { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int l = 60;

            for (int i = 0; i < l; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Console.WriteLine($"{i + 1}/{l}");
            }

            // let readiness endpoint know that this action have finished
            this.ReadinessIndicator.Ready();
        }
    }
```
