#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["micro-c-web/Server/micro-c-web.Server.csproj", "micro-c-web/Server/"]
COPY ["micro-c-web/Client/micro-c-web.Client.csproj", "micro-c-web/Client/"]
COPY ["micro-c-web/Shared/micro-c-web.Shared.csproj", "micro-c-web/Shared/"]
RUN dotnet restore "micro-c-web/Server/micro-c-web.Server.csproj"
COPY . .
WORKDIR "/src/micro-c-web/Server"
RUN dotnet build "micro-c-web.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "micro-c-web.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "micro-c-web.Server.dll"]