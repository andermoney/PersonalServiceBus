choco install MSMQ-Server -source windowsfeatures

#Set up IIS
choco install IIS-WebServerRole IIS-WebServer IIS-CommonHttpFeatures IIS-HttpErrors IIS-HttpRedirect IIS-ApplicationDevelopment IIS-NetFxExtensibility IIS-NetFxExtensibility45 IIS-HealthAndDiagnostics IIS-HttpLogging IIS-LoggingLibraries IIS-RequestMonitor IIS-HttpTracing IIS-Security IIS-URLAuthorization IIS-RequestFiltering IIS-IPSecurity IIS-Performance IIS-HttpCompressionDynamic IIS-WebServerManagementTools IIS-ManagementScriptingTools IIS-IIS6ManagementCompatibility IIS-Metabase IIS-HostableWebCore IIS-StaticContent IIS-DefaultDocument IIS-DirectoryBrowsing IIS-WebDAV IIS-WebSockets IIS-ApplicationInit IIS-ASPNET IIS-ASPNET45 IIS-ASP IIS-CGI IIS-ISAPIExtensions IIS-ISAPIFilter IIS-ServerSideIncludes IIS-CustomLogging IIS-BasicAuthentication IIS-HttpCompressionStatic IIS-ManagementConsole IIS-ManagementService IIS-WMICompatibility IIS-LegacyScripts IIS-LegacySnapIn IIS-FTPServer IIS-FTPSvc IIS-FTPExtensibility -source windowsfeatures


#Create IIS sites
$site = Get-Website -Name "Feeds"
if ($site -eq $null) {
    New-Website -Name "Feeds" -Port 58206 -PhysicalPath "$(Get-Location)\PersonalServiceBus.RSS" -ApplicationPool "FeedsAppPool"
} else {
    Write-Host "Feeds site already created" -ForegroundColor Yellow
}

#$site = Get-Website -Name "RavenDB"
#if ($site -eq $null) {
#    New-Website -name "RavenDB" -Port 8080 -PhysicalPath "c:\inetpub\wwwroot\RavenDB"
#} else {
#    Write-Host "RavenDB site already created" -ForegroundColor Yellow
#}

#Create Queues
$queueName = "personalservicebus.rss"
$queue = Get-MsmqQueue -Name $queueName
if ($queue -eq $null) {
    New-MsmqQueue -Name $queueName -Transactional
} else {
    Write-Host "$($queueName) Queue already created" -ForegroundColor Yellow
}

$queueName = "personalservicebus.rss.timeouts"
$queue = Get-MsmqQueue -Name $queueName
if ($queue -eq $null) {
    New-MsmqQueue -Name $queueName -Transactional
} else {
    Write-Host "$($queueName) Queue already created" -ForegroundColor Yellow
}

$queueName = "personalservicebus.rss.timeoutsdispatcher"
$queue = Get-MsmqQueue -Name $queueName
if ($queue -eq $null) {
    New-MsmqQueue -Name $queueName -Transactional
} else {
    Write-Host "$($queueName) Queue already created" -ForegroundColor Yellow
}
