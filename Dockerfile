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
ENV SSH_SERVER  =172.16.3.100
ENV SSH_USERNAME =uadmin
ENV SSH_PASSWORD =tavsrvtest123!@#
ENV SQL_SERVER   =127.0.0.1
ENV SQL_USERNAME =root
ENV SQL_PASSWORD =CODIMS
ENV NOTIFICATION_API_BASE_URL=http://172.16.3.99:5003
ENV JWT_KEY=kB4DPx5TAvFVxi9ZKDbndQQPWGixhrMjlpfsIARY
ENV TZ=Asia/Kolkata
CMD dotnet IMS-API.dll
