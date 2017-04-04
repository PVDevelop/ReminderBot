rm -r "publish"

echo publishing DeliveryService
dotnet restore "./src/DeliveryService/DeliveryService.sln"
dotnet publish "./src/DeliveryService/src/DeliveryService/DeliveryService.csproj" -c Release -o "./../../../../publish/DeliveryService"

echo publishing Commander
dotnet restore "./src/Commander"
dotnet publish "./src/Commander/Src/Host/Host.csproj" -c Release -o "./../../../../publish/Commander"