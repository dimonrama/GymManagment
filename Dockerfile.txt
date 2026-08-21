FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY GymManagment/GymManagment.csproj GymManagment/
RUN dotnet restore GymManagment/GymManagment.csproj
COPY GymManagment/. GymManagment/
WORKDIR /src/GymManagment
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GymManagment.dll"]