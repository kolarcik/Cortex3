Score API

Endpoints:
- POST /scores  { name, score }
- GET /scores?limit=10&order=desc
- DELETE /scores/{id}  (requires X-Admin-Token header matching AdminToken in config)

Run:
cd src/ScoreApi
dotnet run --urls "http://localhost:5000"

Config:
- Connection string via appsettings or env SCORES_DB
- Admin token via appsettings AdminToken or env ADMIN_TOKEN

CORS:
- By default AllowAnyOrigin; modify in Program.cs for production to restrict frontend origin.
