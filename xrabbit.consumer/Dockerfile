FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["xrabbit.consumer/xrabbit.consumer.csproj", "xrabbit.consumer/"]
RUN dotnet restore "xrabbit.consumer/xrabbit.consumer.csproj"
COPY . .
WORKDIR "/src/xrabbit.consumer"
RUN dotnet build "xrabbit.consumer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "xrabbit.consumer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "xrabbit.consumer.dll"]