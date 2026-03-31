# MediTap

## Git Workflow

- Always branch off `main`

### Branch Naming Conventions

- Features: `feat/your-feature`
- Bug fixes: `fix/issue-fixed`

### Rules

1. Create a branch from `main`
2. Implement your changes
3. Submit for review
4. Ensure tests pass
5. Features or fixes are merged into the main branch after review and tests

### Folder Structure

- /src
  - /client (UI, Forms, etc.)
  - /server (API, DB manager, etc.)
- /tests (Automated Tests, etc.)
- /docs (Documentation, API call information, etc.)
- Solution.sln

### Git Commands

- When starting to work on a new feature or branch:
  - Update local main: `git checkout main`
  - Pull the main branch: `git pull origin main`
  - Create the feat or fix branch: `git checkout -b type/description`
- Save your work:
  - Check what files were changed: `git status`
  - Stage changes: `git add .`
  - Commit to the branch: `git commit -m 'message'`
  - Push to the branch: `git push origin {name of branch}`
