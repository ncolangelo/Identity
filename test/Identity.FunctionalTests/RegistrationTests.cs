﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public abstract class RegistrationTests<TStartup, TContext> : LoggedTest, IClassFixture<ServerFactory<TStartup, TContext>>
        where TStartup : class
        where TContext : DbContext
    {
        public RegistrationTests(ServerFactory<TStartup, TContext> serverFactory, ITestOutputHelper output) : base(output)
        {
            ServerFactory = serverFactory;
        }

        public ServerFactory<TStartup, TContext> ServerFactory { get; }

        [Fact]
        public async Task CanRegisterAUser()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                void ConfigureTestServices(IServiceCollection services) =>
                    services.AddSingleton(loggerFactory);

                var client = ServerFactory
                    .WithWebHostBuilder(whb => whb.ConfigureServices(ConfigureTestServices))
                    .CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";

                // Act & Assert
                await UserStories.RegisterNewUserAsync(client, userName, password);
            }
        }

        [Fact]
        public async Task CanRegisterWithASocialLoginProvider()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                void ConfigureTestServices(IServiceCollection services) =>
                    services
                        .SetupTestThirdPartyLogin()
                        .AddSingleton(loggerFactory);

                var client = ServerFactory
                    .WithWebHostBuilder(whb => whb.ConfigureServices(ConfigureTestServices))
                    .CreateClient();

                var guid = Guid.NewGuid();
                var userName = $"{guid}";
                var email = $"{guid}@example.com";

                // Act & Assert
                await UserStories.RegisterNewUserWithSocialLoginAsync(client, userName, email);
            }
        }
    }
}
