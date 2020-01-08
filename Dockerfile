FROM  mcr.microsoft.com/dotnet/core/aspnet:2.2.6-alpine AS base
WORKDIR /app
EXPOSE 80

#COPY AKSIngressData/ /app

#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime

#New relic environment common variables
ENV APP_NAME="Ingress Version 2"

COPY ./AKSIngressData /app
#ENV ASPNETCORE_URLS=http://+:82
ENTRYPOINT ["dotnet", "/app/AKSIngress.API.dll"]

