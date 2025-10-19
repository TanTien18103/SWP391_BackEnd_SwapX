FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SWP391_BackEnd/SWP391_BackEnd.csproj", "SWP391_BackEnd/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["BusinessObjects/BusinessObjects.csproj", "BusinessObjects/"]
RUN dotnet restore "SWP391_BackEnd/SWP391_BackEnd.csproj"
COPY . .
WORKDIR "/src/SWP391_BackEnd"
RUN dotnet build "./SWP391_BackEnd.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SWP391_BackEnd.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SWP391_BackEnd.dll"]
