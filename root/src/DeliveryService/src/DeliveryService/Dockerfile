FROM microsoft/dotnet:1.1.0-core
ENTRYPOINT ["dotnet", "DeliveryService.dll"]
ARG source=.
WORKDIR /app
COPY $source .
