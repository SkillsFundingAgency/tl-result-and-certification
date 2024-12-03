﻿/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF ('$(environment)' = 'test')
BEGIN
/* Below seed script is applicable for test environment as testers are seeding their own data in test environment */
:r ".\SeedNotificationTemplates.sql"
:r ".\SeedAssessmentSeries.sql"
:r ".\SeedAcademicYear.sql"
:r ".\SeedTlLookup.sql"
:r ".\SeedQualificationType.sql"
:r ".\SeedQualificationGrade.sql"
:r ".\SeedQualification.sql"
:r ".\SeedIpLookup.sql"
:r ".\SeedOverallGradeLookup.sql"
:r ".\SeedTlDualSpecialism.sql"
:r ".\SeedTlDualSpecialismToSpecialism.sql"
:r ".\SeedOverallGradeLookupPostStartYear2020.sql"
:r ".\SeedDualSpecialismOverallGradeLookup.sql"
END
ELSE IF ('$(environment)' <> 'DevIntegration')
BEGIN
/*For DevIntegration we do not want to seed any data */
:r ".\SeedTlAwardingOrganisations.sql"
:r ".\SeedTlRoutes.sql"
:r ".\SeedTlPathways.sql"
:r ".\SeedTlSpecialisms.sql"
:r ".\SeedTlPathwaySpecialismCombinations.sql"
:r ".\SeedTqAwardingOrganisations.sql"
:r ".\SeedNotificationTemplates.sql"
:r ".\SeedAssessmentSeries.sql"
:r ".\SeedAcademicYear.sql"
:r ".\SeedTlLookup.sql"
:r ".\SeedQualificationType.sql"
:r ".\SeedQualificationGrade.sql"
:r ".\SeedQualification.sql"
:r ".\SeedIpLookup.sql"
:r ".\SeedIpModelTlevelCombination.sql"
:r ".\SeedIpTempFlexTlevelCombination.sql"
:r ".\SeedIpTempFlexNavigation.sql"
:r ".\SeedOverallGradeLookup.sql"
:r ".\SeedTlDualSpecialism.sql"
:r ".\SeedTlDualSpecialismToSpecialism.sql"
:r ".\SeedOverallGradeLookupPostStartYear2020.sql"
:r ".\SeedDualSpecialismOverallGradeLookup.sql"
END