# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/Api/Api.csproj", "Src/Api/"]
COPY ["Src/BuildingBlocks/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "Src/BuildingBlocks/BuildingBlocks.Infrastructure/"]
COPY ["Src/BuildingBlocks/Application/BuildingBlocks.Application.csproj", "Src/BuildingBlocks/Application/"]
COPY ["Src/BuildingBlocks/Domain/BuildingBlocks.Domain.csproj", "Src/BuildingBlocks/Domain/"]
COPY ["Src/Modules/Content/Content.Infrastructure/Content.Infrastructure.csproj", "Src/Modules/Content/Content.Infrastructure/"]
COPY ["Src/Modules/Content/Content.Application/Content.Application.csproj", "Src/Modules/Content/Content.Application/"]
COPY ["Src/Modules/Content/Content.Domain/Content.Domain.csproj", "Src/Modules/Content/Content.Domain/"]
COPY ["Src/Modules/Customers/Customers.Application/Customers.Application.csproj", "Src/Modules/Customers/Customers.Application/"]
COPY ["Src/Modules/Identity/Identity.Contracts/Identity.Contracts.csproj", "Src/Modules/Identity/Identity.Contracts/"]
COPY ["Src/Modules/Customers/Customers.Contracts/Customers.Contracts.csproj", "Src/Modules/Customers/Customers.Contracts/"]
COPY ["Src/Modules/Customers/Customers.Domain/Customers.Domain.csproj", "Src/Modules/Customers/Customers.Domain/"]
COPY ["Src/Modules/Customers/Infrastructure/Customers.Infrastructure.csproj", "Src/Modules/Customers/Infrastructure/"]
COPY ["Src/Modules/Identity/Application/Identity.Application.csproj", "Src/Modules/Identity/Application/"]
COPY ["Src/BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "Src/BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["Src/Modules/Identity/Domain/Identity.Domain.csproj", "Src/Modules/Identity/Domain/"]
COPY ["Src/Modules/Identity/Infrastructure/Identity.Infrastructure.csproj", "Src/Modules/Identity/Infrastructure/"]
COPY ["Src/Modules/Notifications/Notifications.Application/Notifications.Application.csproj", "Src/Modules/Notifications/Notifications.Application/"]
COPY ["Src/Modules/Notifications/Notifications.Domain/Notifications.Domain.csproj", "Src/Modules/Notifications/Notifications.Domain/"]
COPY ["Src/Modules/Notifications/Notifications.Infrastructure/Notifications.Infrastructure.csproj", "Src/Modules/Notifications/Notifications.Infrastructure/"]
COPY ["Src/Modules/Operations/Operations.Application/Operations.Application.csproj", "Src/Modules/Operations/Operations.Application/"]
COPY ["Src/Modules/Operations/Operations.Domain/Operations.Domain.csproj", "Src/Modules/Operations/Operations.Domain/"]
COPY ["Src/Modules/Operations/Operations.Infrastructure/Operations.Infrastructure.csproj", "Src/Modules/Operations/Operations.Infrastructure/"]
COPY ["Src/Modules/Reports/Reports.Infrastructure/Reports.Infrastructure.csproj", "Src/Modules/Reports/Reports.Infrastructure/"]
COPY ["Src/Modules/Reports/Reports.Application/Reports.Application.csproj", "Src/Modules/Reports/Reports.Application/"]
COPY ["Src/Modules/Reports/Reports.Domain/Reports.Domain.csproj", "Src/Modules/Reports/Reports.Domain/"]
COPY ["Src/Modules/Website/Website.Application/Website.Application.csproj", "Src/Modules/Website/Website.Application/"]
COPY ["Src/Modules/Website/Website.Domain/Website.Domain.csproj", "Src/Modules/Website/Website.Domain/"]
COPY ["Src/Modules/Website/Website.Infrastructure/Website.Infrastructure.csproj", "Src/Modules/Website/Website.Infrastructure/"]
RUN dotnet restore "./Src/Api/Api.csproj"
COPY . .
WORKDIR "/src/Src/Api"
RUN dotnet build "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]