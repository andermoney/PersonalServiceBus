.\packages\OpenCover.4.5.1604\OpenCover.Console.exe -register:user -target:.\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe -targetargs:"PersonalServiceBus.RSS.Test.Unit.dll /noshadow" "-filter:+[PersonalServiceBus.RSS.*]* -[PersonalServiceBus.*.Test*]*" -output:PersonalServiceBus.RSS.Test.output.xml -targetdir:.\PersonalServiceBus.RSS.Test.Unit\bin\Debug\
.\packages\ReportGenerator.1.8.1.0\ReportGenerator.exe -reports:PersonalServiceBus.RSS.Test.output.xml -reporttypes:Html -targetdir:.\TestReports
.\TestReports\index.htm