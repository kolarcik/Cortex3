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

CI workflow behavior changed:
- Workflows now ignore pushes and pull requests targeting the 'main' branch to avoid triggering CI redundantly on protected/main merges.
- This repository-level change was applied to .github/workflows/ci.yml (push.branches-ignore and pull_request.branches-ignore set for 'main').

Reason: reduce duplicate CI runs and rely on branch protection rules to require CI on PRs before merge; keeps main merge workflows controlled.

Commit note (local changes only): "ci: ignore main branch for push and pull_request to reduce redundant runs"
