#Create IIS sites
$site = Get-Website -Name "Feeds"
if ($site -eq $null) {
    New-Website -Name "Feeds" -Port 58206 -PhysicalPath "$(Get-Location)\PersonalServiceBus.RSS"
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
