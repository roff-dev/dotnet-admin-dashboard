namespace AdminDashboard.Data.Seeding
{
    public static class DatabaseSeederExtension
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            // new scope for dependency
            // ensures disposal of services
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // get service provider from scope
                var services = scope.ServiceProvider;

                // get database context from service provider
                var context = services.GetRequiredService<ApplicationDbContext>();

                try
                {
                    // call seeding method
                    await DataSeeder.SeedData(context);
                }
                catch (Exception ex)
                {
                    //logger service and log any errors
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                    throw;
                }
            }
        }
    }
}
