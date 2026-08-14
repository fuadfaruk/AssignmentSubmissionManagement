# Use Microsoft .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/AssignmentSubmissionManagement.Api/AssignmentSubmissionManagement.Api.csproj", "src/AssignmentSubmissionManagement.Api/"]
COPY ["src/AssignmentSubmissionManagement.Core/AssignmentSubmissionManagement.Core.csproj", "src/AssignmentSubmissionManagement.Core/"]
COPY ["src/AssignmentSubmissionManagement.Infrastructure/AssignmentSubmissionManagement.Infrastructure.csproj", "src/AssignmentSubmissionManagement.Infrastructure/"]

RUN dotnet restore "src/AssignmentSubmissionManagement.Api/AssignmentSubmissionManagement.Api.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/src/AssignmentSubmissionManagement.Api"
RUN dotnet publish "AssignmentSubmissionManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Production image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AssignmentSubmissionManagement.Api.dll"]
