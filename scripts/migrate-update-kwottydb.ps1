$continueMigration = Read-Host "Do you want to execute a migration? (y/n)"

$StartupProjectPath = "..\src\Kwotty.Web\Kwotty.Web.csproj"
$ProjectPath = "..\src\Kwotty.Data\Kwotty.Data.csproj"
$DbContextMigration = "KwottyDbContext"
$DbContextUpdate = "KwottyDbContext"
$MigrationsPath = "Migrations"

if ($continueMigration -eq 'y') {
    while ($true) {
        $MigrationName = Read-Host "Enter the migration name"
        if (-not $MigrationName) {
            Write-Host "--- Migration name cannot be empty. Please try again."
            continue
        }
        break
    }

    Write-Host "--- Adding migration '$MigrationName' to '$MigrationsPath'..."

    # Run the EF Core migration command
    dotnet ef migrations add $MigrationName `
      --project $ProjectPath `
      --startup-project $StartupProjectPath `
      --context $DbContextMigration `
      --output-dir $MigrationsPath

    if (-not $?) {
        Write-Host "--- Migration failed. Please check the error messages."
        exit
    }

    Write-Host "--- Migration '$MigrationName' created successfully!"
}

$userResponse = Read-Host "Do you want to update the database? (y/n)"
if ($userResponse -eq 'y') {
    Write-Host "--- Updating database..."
    dotnet ef database update `
	  --project $ProjectPath `
      --startup-project $StartupProjectPath `
      --context $DbContextUpdate
    if ($?) {
        Write-Host "--- Database updated successfully!"
    } else {
        Write-Host "--- Database update failed. Please check the error messages."
    }
} else {
    Write-Host "--- Database update skipped. Exiting script."
}
