﻿using DotNet.Testcontainers.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace TechChallange.Test.IntegrationTests
{
    public class MsSqlContainerFixture : IAsyncLifetime
    {
        public MsSqlContainer MsSqlContainer { get; private set; }
        public RedisContainer RedisContainer { get; private set; }

        public MsSqlContainerFixture()
        {
            MsSqlContainer = new  MsSqlBuilder()
        //    //.WithImage("mcr.microsoft.com/mssql/server:2022-latest") // Imagem do SQL Server
        //    //.WithPassword("yourStrong(!)Password") // Senha do usuário SA
          .Build();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                MsSqlContainer = new MsSqlBuilder()
                    .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                      .WithPassword("password(!)Strong")
                             .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                             .Build();
            }
            else
            {
                MsSqlContainer = new MsSqlBuilder().Build();
            }

            RedisContainer = new RedisBuilder()
                .Build();
        }


        //public readonly MsSqlContainer MsSqlContainer = new MsSqlBuilder()
        //    //.WithImage("mcr.microsoft.com/mssql/server:2022-latest") // Imagem do SQL Server
        //    //.WithPassword("yourStrong(!)Password") // Senha do usuário SA
        //    .Build();

     //   public readonly RedisContainer redis = new RedisBuilder().Build();

        public async Task InitializeAsync()
        {
            await MsSqlContainer.StartAsync();
            await RedisContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await MsSqlContainer.DisposeAsync();
            await RedisContainer.DisposeAsync();
        }
    }
}
