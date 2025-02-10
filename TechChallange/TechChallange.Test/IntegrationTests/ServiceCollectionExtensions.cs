using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechChallange.Infrastructure.Context;

namespace TechChallange.Test.IntegrationTests
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, string connectionString)
        {
            services.RemoveDbContext<TechChallangeContext>(); // Removemos o contexto existente
            services.AddDbContext<TechChallangeContext>(options =>
                options.UseSqlServer(connectionString)); // Configuramos com o SQL do TestContainers

            return services;
        }

        // Método auxiliar para remover um DbContext já registrado
        private static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }
    }
}
