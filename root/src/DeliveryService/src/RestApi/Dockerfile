FROM microsoft/aspnetcore:1.1.0
ENTRYPOINT ["dotnet", "RestApi.dll"]
ARG source=.
WORKDIR /app
EXPOSE 80
COPY $source .
