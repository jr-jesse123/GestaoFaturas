FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/GestaoFaturas.Api/GestaoFaturas.Api.csproj", "src/GestaoFaturas.Api/"]
RUN dotnet restore "src/GestaoFaturas.Api/GestaoFaturas.Api.csproj"
COPY . .
WORKDIR "/src/src/GestaoFaturas.Api"
RUN dotnet build "GestaoFaturas.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "GestaoFaturas.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestaoFaturas.Api.dll"]
