﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamSpeak3QueryApi.Net.Specialized.Responses;

namespace TeamspeakAnalytics.ts3provider
{
  public static class TS3ProviderExtentions
  {
    public static IServiceCollection AddTS3Provider<T>(
        this IServiceCollection services, 
        TS3ServerInfo serverInfo,
        Func<IServiceProvider, ITS3DataProvider> implementationFactory = null) 
      where T : class, ITS3DataProvider
    {
      if (string.IsNullOrWhiteSpace(serverInfo.Password))
        throw new ArgumentException($"{nameof(serverInfo.Password)} was not set!", nameof(serverInfo.Password));
      

      services.AddSingleton<TS3ServerInfo>(serverInfo);

      if (implementationFactory == null)
        services.AddSingleton<ITS3DataProvider, T>();
      else
        services.AddSingleton<ITS3DataProvider>(implementationFactory);

      return services;
    }
    
    internal static async Task ConnectAndInitConnection(this TeamSpeakClient teamSpeakClient, TS3ServerInfo serverInfo)
    {
      await teamSpeakClient.Connect()
            .ContinueWith(o => teamSpeakClient.Login(serverInfo.Username, serverInfo.Password))
            .ContinueWith(o => teamSpeakClient.UseServer(serverInfo.ServerIndex));
    }
  }
}
