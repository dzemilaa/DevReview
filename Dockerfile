FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY DevReview.Domain/DevReview.Domain.csproj DevReview.Domain/
COPY DevReview.Application/DevReview.Application.csproj DevReview.Application/
COPY DevReview.Infrastructure/DevReview.Infrastructure.csproj DevReview.Infrastructure/
COPY DevReview.API/DevReview.API.csproj DevReview.API/

RUN dotnet restore DevReview.API/DevReview.API.csproj

COPY DevReview.Domain/ DevReview.Domain/
COPY DevReview.Application/ DevReview.Application/
COPY DevReview.Infrastructure/ DevReview.Infrastructure/
COPY DevReview.API/ DevReview.API/

RUN dotnet publish DevReview.API/DevReview.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN useradd -m appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "DevReview.API.dll"]
