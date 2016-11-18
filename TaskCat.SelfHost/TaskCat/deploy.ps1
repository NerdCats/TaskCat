$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$scriptPath = Join-Path $executingScriptDirectory "TaskCat.exe"

Invoke-Expression "$scriptPath --uninstall"
Invoke-Expression "$scriptPath --install"