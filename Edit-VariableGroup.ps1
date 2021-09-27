[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]    $OrganizationName,
    [Parameter(Mandatory = $true)]
    [string]    $ProjectName,
    [Parameter(Mandatory = $false)]
    [string[]] $Groups
)

if ($null -eq $Groups -or $Groups.Length -eq 0) {
    $g = Get-VSTeamVariableGroup -ProjectName $ProjectName
    $Groups = ($g).name
}

$Groups = $Groups | Sort-Object
$gdic = @{}

$VariableRow = @"
public class VariableRow
{
    public VariableRow(string name){
        this.Name = name;
    }
  public string Name {get; private set;}
  public bool IsSecret {get;set;}
"@

foreach ($g in $Groups) {
    $group = Get-VSTeamVariableGroup -ProjectName $ProjectName -Name $g -ErrorAction SilentlyContinue
    if ($null -eq $group) {
        Write-Warning "$g not found"
    }
    else {
        $gdic[$g] = $group
        $VariableRow = $VariableRow + @"

        public string $($group.name) {get;set;}

"@
    }
}

$VariableRow = $VariableRow + "}"
Add-Type -TypeDefinition $VariableRow

$res = @{}

foreach ($group in $gdic.Values) {
    $members = $group.variables | Get-Member -MemberType NoteProperty
    $dicVar = $null

    foreach ($m in $members) {
        if ($res.Contains($m.Name)) {
            $dicVar = $res[$m.Name]
        }
        else {
            # $dicVar = New-Object -TypeName PSCustomObject -Property @{Name = $m.Name }
            # Add-Member -InputObject $dicVar -MemberType NoteProperty -Name "IsSecret" -Value $false -TypeName boolean
            # foreach ($abcd in $Groups) {
            #     Add-Member -InputObject $dicVar -MemberType NoteProperty -Name $abcd -Value $null -TypeName string
            # }
            $dicVar = New-Object -TypeName VariableRow -ArgumentList $m.Name
            $res[$m.name] = $dicVar
        }

        $dicVar.$($group.name) = $group.variables.$($m.Name).value
        # Add-Member -InputObject $dicVar -MemberType NoteProperty -Name $group.name -Value $group.variables.$($m.Name).value -TypeName string
    }
    # $dicVar | Get-Member
}

# $res.Values | Select-Object | Out-GridView -Title "Edit variable groups" -OutputMode Multiple

# Select-Object | 


$filePath = Join-Path $PSScriptRoot "Window.xaml"
[xml]$XAMLMain = [XML](Get-Content $filePath)

Add-Type -AssemblyName PresentationFramework
$reader = (New-Object System.Xml.XmlNodeReader $XAMLMain)
$windowMain = [Windows.Markup.XamlReader]::Load( $reader )
$windowMain.DataContext = $res.Values
$windowMain.ShowDialog() | Out-Null

Write-Host "Hello"