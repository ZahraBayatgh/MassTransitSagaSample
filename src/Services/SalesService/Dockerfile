#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Services/SaleService/SaleService.csproj", "src/Services/SaleService/"]
COPY ["src/Common/EventBus/EventBus.csproj", "src/Common/EventBus/"]
COPY ["src/Common/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "src/Common/EventBusRabbitMQ/"]
RUN dotnet restore "src/Services/SaleService/SaleService.csproj"
COPY . .
WORKDIR "/src/src/Services/SaleService"
RUN dotnet build "SaleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SaleService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SaleService.dll"]
