FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Discount/DiscountGrpc/DiscountGrpc.csproj", "src/Discount/DiscountGrpc/"]
RUN dotnet restore "src/Discount/DiscountGrpc/DiscountGrpc.csproj"
COPY . .
WORKDIR "/src/src/Discount/DiscountGrpc"
RUN dotnet build "DiscountGrpc.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "DiscountGrpc.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/data && chown -R app:app /app/data

USER app

ENTRYPOINT ["dotnet", "DiscountGrpc.dll"]
