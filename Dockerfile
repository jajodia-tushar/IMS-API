FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app
COPY . /app

RUN dotnet restore IMS-API.sln --source https://api.nuget.org/v3/index.json && \
    dotnet build IMS-API.sln -c Release && \
    dotnet publish IMS-API/IMS-API.csproj -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 as runtime
WORKDIR /app
COPY --from=build /app/publish ./
COPY --from=build /app/IMS-API/IMS-API.xml ./
ENV ASPNETCORE_ENVIRONMENT=PRODUCTION
ENV TZ=Asia/Kolkata
CMD dotnet IMS-API.dll
